using Microsoft.AspNetCore.Mvc;
using VelourEssence.Application.Services.Interfaces;
using VelourEssence.Application.DTOs;
using System.Diagnostics;

namespace VelourEssence.Web.Controllers
{
    // Controlador encargado de manejar las vistas relacionadas con productos
    public class ProductoController : Controller
    {
        // Servicio que permite acceder a los datos de productos
        private readonly IServiceProducto _serviceProducto;

        // Constructor que recibe el servicio de productos por inyección de dependencias
        public ProductoController(IServiceProducto serviceProducto)
        {
            _serviceProducto = serviceProducto;
        }

        // Acción que muestra la lista de todos los productos
        // Llama al servicio para obtener los datos y los pasa a la vista
        public async Task<IActionResult> Index()
        {
            var productos = await _serviceProducto.ListAsync();
            return View(productos); // Muestra la vista con la lista de productos
        }

        // Acción que muestra los detalles completos de un producto específico por su ID
        public async Task<IActionResult> Details(int id)
        {
            var producto = await _serviceProducto.GetDetalleByIdAsync(id);

            // Si no se encuentra el producto, devuelve un error 404
            if (producto == null)
                return NotFound();

            // Si existe, se pasa a la vista para mostrar sus detalles
            return View(producto);
        }

        // === ACCIONES DE MANTENIMIENTO ===

        // Muestra la lista de productos para administración
        public async Task<IActionResult> Manage()
        {
            var productos = await _serviceProducto.ListAsync();
            return View(productos);
        }

        // Muestra el formulario para crear un nuevo producto
        public async Task<IActionResult> Create()
        {
            var mantenimientoDto = await _serviceProducto.GetMantenimientoDatosAsync();
            return View(mantenimientoDto);
        }

        // Procesa la creación de un nuevo producto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoMantenimientoDto model, List<IFormFile> imagenesArchivos)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si ya existe un producto con el mismo nombre
                    if (await _serviceProducto.ExistsAsync(model.CrearProducto!.Nombre))
                    {
                        ModelState.AddModelError("CrearProducto.Nombre", "Ya existe un producto con este nombre");
                        return View(await RefreshMantenimientoDto(model));
                    }

                    // Convertir archivos de imagen a ImagenUploadDto
                    if (imagenesArchivos?.Any() == true)
                    {
                        model.CrearProducto!.ImagenesArchivos = await ConvertirArchivosADto(imagenesArchivos);
                    }

                    var producto = await _serviceProducto.CreateAsync(model.CrearProducto!);
                    TempData["SuccessMessage"] = "Producto creado exitosamente";
                    return RedirectToAction(nameof(Manage));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al crear el producto: " + ex.Message);
                }
            }

            return View(await RefreshMantenimientoDto(model));
        }

        // Muestra el formulario para editar un producto existente
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var mantenimientoDto = await _serviceProducto.GetForEditAsync(id);
            if (mantenimientoDto?.EditarProducto == null)
                return NotFound();

            return View(mantenimientoDto);
        }

        // Procesa la edición de un producto existente

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductoMantenimientoDto model, List<IFormFile> imagenesNuevas, string imagenesAEliminar)
        {
            //pruebas
            Debug.WriteLine("📥 Archivos recibidos en Request.Form.Files:");
            foreach (var archivo in Request.Form.Files)
            {
                Debug.WriteLine($"- Nombre: {archivo.FileName}, Tamaño: {archivo.Length}, Tipo: {archivo.ContentType}");
            }





            Debug.WriteLine("▶ FORM KEYS:");
            foreach (var key in Request.Form.Keys)
            {
                Debug.WriteLine($"{key} = {Request.Form[key]}");
            }

            Debug.WriteLine("▶ LLEGO imagenesAEliminar: " + imagenesAEliminar);



            if (!string.IsNullOrEmpty(imagenesAEliminar))
            {
                var idsAEliminar = imagenesAEliminar
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(cadena => int.Parse(cadena.Trim()))
                    .ToList();

                model.EditarProducto.ImagenesAEliminar = idsAEliminar;

                Console.WriteLine("✅ ImagenesAEliminar parseadas:");
                foreach (var idImagen in idsAEliminar)
                {
                    Console.WriteLine("- " + idImagen);
                }
            }





            else
            {
                Console.WriteLine("⚠️ imagenesAEliminar llegó vacío o null.");
            }


            if (id != model.EditarProducto!.IdProducto)
                return NotFound();
            //prueba
            

            if (!ModelState.IsValid)
            {
                Console.WriteLine("❌ ModelState no es válido. Errores:");
                foreach (var kvp in ModelState)
                {
                    foreach (var error in kvp.Value.Errors)
                    {
                        Console.WriteLine($"🛑 Campo: {kvp.Key}, Error: {error.ErrorMessage}");
                    }
                }
            }


            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar duplicado
                    if (await _serviceProducto.ExistsAsync(model.EditarProducto.Nombre, id))
                    {
                        ModelState.AddModelError("EditarProducto.Nombre", "Ya existe otro producto con este nombre");
                        return View(await RefreshMantenimientoDto(model, id));
                    }

                    //pruebas
                    Console.WriteLine("🧾 imagenesNuevas.Count = " + (imagenesNuevas?.Count ?? 0));
                    foreach (var file in imagenesNuevas ?? new List<IFormFile>())
                    {
                        Console.WriteLine($"📸 Archivo: {file.FileName}, Tamaño: {file.Length}");
                    }






                    // Procesar nuevas imágenes
                   if (imagenesNuevas?.Any() == true)
                    {

                       model.EditarProducto.ImagenesNuevasArchivos = await ConvertirArchivosADto(imagenesNuevas);

                       
                    }

                    // ❗ YA NO SE NECESITA CARGAR ImagenesAEliminar MANUALMENTE
                    // Ya se llena automáticamente gracias al binding del input hidden

                    await _serviceProducto.UpdateAsync(model.EditarProducto);

                    TempData["SuccessMessage"] = "Producto actualizado exitosamente";
                    return RedirectToAction(nameof(Manage));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar el producto: " + ex.Message);
                }
            }

            return View(await RefreshMantenimientoDto(model, id));
        }

        //


        // Muestra la confirmación para eliminar un producto
        public async Task<IActionResult> Delete(int id)
        {
            var producto = await _serviceProducto.GetDetalleByIdAsync(id);
            if (producto == null)
                return NotFound();

            return View(producto);
        }

        // Procesa la eliminación de un producto
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _serviceProducto.DeleteAsync(id);
                if (result)
                {
                    TempData["SuccessMessage"] = "Producto eliminado exitosamente";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se pudo eliminar el producto";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al eliminar el producto: " + ex.Message;
            }

            return RedirectToAction(nameof(Manage));
        }

        // === MÉTODOS AUXILIARES ===

        // Convierte los archivos IFormFile a ImagenUploadDto
        private async Task<List<ImagenUploadDto>> ConvertirArchivosADto(List<IFormFile> archivos)
        {
            var imagenesDto = new List<ImagenUploadDto>();

            foreach (var archivo in archivos)
            {
                if (archivo.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await archivo.CopyToAsync(memoryStream);
                    
                    imagenesDto.Add(new ImagenUploadDto
                    {
                        NombreArchivo = archivo.FileName,
                        TipoContenido = archivo.ContentType,
                        ContenidoArchivo = memoryStream.ToArray()
                    });
                }
            }

            return imagenesDto;
        }

        // Actualiza los datos del mantenimiento (categorías, etiquetas) para cuando hay errores
        private async Task<ProductoMantenimientoDto> RefreshMantenimientoDto(ProductoMantenimientoDto model, int? idProducto = null)
        {
            var datosMantenimiento = idProducto.HasValue
                ? await _serviceProducto.GetForEditAsync(idProducto.Value)
                : await _serviceProducto.GetMantenimientoDatosAsync();

            model.Categorias = datosMantenimiento.Categorias;
            model.Etiquetas = datosMantenimiento.Etiquetas;
            
            if (idProducto.HasValue)
            {
                model.PromedioValoracion = datosMantenimiento.PromedioValoracion;
                model.TotalReseñas = datosMantenimiento.TotalReseñas;
            }

            return model;
        }

        // Acción que muestra la vista de personalización de productos
        public async Task<IActionResult> Personalizar(int id)
        {
            var producto = await _serviceProducto.GetByIdAsync(id);

            // Si no se encuentra el producto, devuelve un error 404
            if (producto == null)
                return NotFound();

            // Si existe, se pasa a la vista para mostrar el personalizador
            return View(producto);
        }
    }
}
