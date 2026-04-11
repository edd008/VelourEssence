using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using VelourEssence.Application.Services.Interfaces;
using VelourEssence.Application.DTOs;

namespace VelourEssence.Web.Controllers
{
    // Controlador que maneja las vistas relacionadas con promociones
    public class PromocionController : Controller
    {
        // Servicio para acceder a datos de promociones
        private readonly IServicePromocion _servicePromocion;
        // Servicio para acceder a datos de productos
        private readonly IServiceProducto _serviceProducto;

        // Constructor que recibe los servicios mediante inyección de dependencias
        public PromocionController(IServicePromocion servicePromocion, IServiceProducto serviceProducto)
        {
            _servicePromocion = servicePromocion;
            _serviceProducto = serviceProducto;
        }

        // Acción que muestra la lista de todas las promociones - Solo administradores
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index()
        {
            var promociones = await _servicePromocion.ListAsync();
            return View(promociones); 
        }

        // Acción que muestra detalles de una promoción específica por su ID - Solo administradores
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Details(int id)
        {
            var promocion = await _servicePromocion.GetByIdAsync(id);

            // Si no se encuentra la promoción, devuelve error 404
            if (promocion == null)
                return NotFound();

            // Si existe, pasa la promoción a la vista para mostrar detalles
            return View(promocion);
        }

        // Muestra el formulario para crear una nueva promoción
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create()
        {
            await CargarDropdowns();
            return View(new CrearPromocionDto 
            { 
                FechaInicio = DateOnly.FromDateTime(DateTime.Today),
                FechaFin = DateOnly.FromDateTime(DateTime.Today.AddDays(30))
            });
        }

        // Procesa el formulario para crear una nueva promoción
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CrearPromocionDto promocionDto)
        {
            if (ModelState.IsValid)
            {
                // Validaciones personalizadas
                if (!ValidarFechas(promocionDto))
                {
                    await CargarDropdowns();
                    return View(promocionDto);
                }

                if (!ValidarTipoYAplicacion(promocionDto))
                {
                    await CargarDropdowns();
                    return View(promocionDto);
                }

                var result = await _servicePromocion.CreateAsync(promocionDto);
                if (result)
                {
                    TempData["Success"] = "Promoción creada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Error al crear la promoción.");
                }
            }

            await CargarDropdowns();
            return View(promocionDto);
        }

        // Muestra el formulario para editar una promoción existente
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id)
        {
            var promocion = await _servicePromocion.GetForEditAsync(id);
            if (promocion == null)
                return NotFound();

            // Validar si la promoción ya se aplicó (fecha fin anterior a hoy)
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            if (promocion.FechaFin < hoy)
            {
                TempData["Error"] = "No se pueden modificar promociones que ya se aplicaron.";
                return RedirectToAction(nameof(Index));
            }

            await CargarDropdowns();
            return View(promocion);
        }

        // Procesa el formulario para editar una promoción
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CrearPromocionDto promocionDto)
        {
            if (id != promocionDto.IdPromocion)
                return NotFound();

            if (ModelState.IsValid)
            {
                // Validaciones personalizadas
                if (!ValidarFechas(promocionDto))
                {
                    await CargarDropdowns();
                    return View(promocionDto);
                }

                if (!ValidarTipoYAplicacion(promocionDto))
                {
                    await CargarDropdowns();
                    return View(promocionDto);
                }

                var result = await _servicePromocion.UpdateAsync(promocionDto);
                if (result)
                {
                    TempData["Success"] = "Promoción actualizada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Error al actualizar la promoción.");
                }
            }

            await CargarDropdowns();
            return View(promocionDto);
        }

        // Muestra la confirmación para eliminar una promoción
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            var promocion = await _servicePromocion.GetByIdAsync(id);
            if (promocion == null)
                return NotFound();

            return View(promocion);
        }

        // Procesa la eliminación de una promoción
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _servicePromocion.DeleteAsync(id);
            if (result)
            {
                TempData["Success"] = "Promoción eliminada exitosamente.";
            }
            else
            {
                TempData["Error"] = "Error al eliminar la promoción.";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> PromoDisponible()
        {
            // Obtiene todos los productos
            var productos = await _serviceProducto.ListAsync();
            
            // Filtra solo los productos que tienen promociones vigentes
            var productosConPromocion = productos.Where(p => p.TienePromocion).ToList();
            
            return View(productosConPromocion);
        }

        // Métodos privados de validación y utilidad
        private bool ValidarFechas(CrearPromocionDto promocionDto)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            
            if (promocionDto.FechaInicio < hoy)
            {
                ModelState.AddModelError("FechaInicio", "La fecha de inicio no puede ser anterior a la fecha actual.");
                return false;
            }

            if (promocionDto.FechaFin <= promocionDto.FechaInicio)
            {
                ModelState.AddModelError("FechaFin", "La fecha de fin debe ser posterior a la fecha de inicio.");
                return false;
            }

            return true;
        }

        private bool ValidarTipoYAplicacion(CrearPromocionDto promocionDto)
        {
            if (promocionDto.Tipo == "Producto")
            {
                if (!promocionDto.IdProducto.HasValue || promocionDto.IdProducto <= 0)
                {
                    ModelState.AddModelError("IdProducto", "Debe seleccionar un producto cuando el tipo es 'Producto'.");
                    return false;
                }
                promocionDto.IdCategoria = null;
            }
            else if (promocionDto.Tipo == "Categoria")
            {
                if (!promocionDto.IdCategoria.HasValue || promocionDto.IdCategoria <= 0)
                {
                    ModelState.AddModelError("IdCategoria", "Debe seleccionar una categoría cuando el tipo es 'Categoría'.");
                    return false;
                }
                promocionDto.IdProducto = null;
            }
            else
            {
                ModelState.AddModelError("Tipo", "Debe seleccionar un tipo de promoción válido.");
                return false;
            }

            return true;
        }

        private async Task CargarDropdowns()
        {
            var categorias = await _servicePromocion.GetCategoriasAsync();
            var productos = await _servicePromocion.GetProductosAsync();

            ViewBag.Categorias = new SelectList(categorias, "IdCategoria", "Nombre");
            ViewBag.Productos = new SelectList(productos, "IdProducto", "Nombre");
            ViewBag.TiposPromocion = new SelectList(new[] 
            {
                new { Value = "Producto", Text = "Producto específico" },
                new { Value = "Categoria", Text = "Categoría de productos" }
            }, "Value", "Text");
        }
    }
}
