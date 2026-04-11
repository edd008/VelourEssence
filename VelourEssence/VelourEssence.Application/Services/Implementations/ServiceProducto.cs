using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VelourEssence.Application.DTOs;
using VelourEssence.Application.Services.Interfaces;
using VelourEssence.Infraestructure.Models;
using VelourEssence.Infraestructure.Repository.Interfaces;

namespace VelourEssence.Application.Services.Implementations
{
    public class ServiceProducto : IServiceProducto
    {
        // Repositorio para acceder a la data de productos
        private readonly IRepositoryProductos _repositoryProductos;
        // AutoMapper para convertir entidades en DTOs
        private readonly IMapper _mapper;

        // Constructor que recibe el repositorio y el mapper por inyección de dependencias
        public ServiceProducto(IRepositoryProductos repositoryProductos, IMapper mapper)
        {
            _repositoryProductos = repositoryProductos;
            _mapper = mapper;
        }

        // Lista de los productos mapeados a ProductoDto
        public async Task<ICollection<ProductoDto>> ListAsync()
        {
            var productos = await _repositoryProductos.ListAsync();
            return _mapper.Map<ICollection<ProductoDto>>(productos);
        }

        // Producto por Id, mapeado a ProductoDto CON PROMOCIONES APLICADAS
        public async Task<ProductoDto?> GetByIdAsync(int id)
        {
            var producto = await _repositoryProductos.GetByIdAsync(id);
            if (producto == null) return null;

            var productoDto = _mapper.Map<ProductoDto>(producto);
            
            // APLICAR PROMOCIONES - igual que en el carrito
            Console.WriteLine($"[DEBUG PRODUCTO] Verificando promoción para producto ID: {id}");
            
            var promocion = await _repositoryProductos.GetPromocionActivaAsync(id);
            if (promocion?.PorcentajeDescuento.HasValue == true)
            {
                var descuento = promocion.PorcentajeDescuento.Value;
                var precioOriginal = producto.Precio ?? 0;
                var precioConDescuento = precioOriginal * (1 - (descuento / 100));
                
                productoDto.TienePromocion = true;
                productoDto.PrecioConDescuento = precioConDescuento;
                
                Console.WriteLine($"[DEBUG PRODUCTO] ✅ Promoción aplicada:");
                Console.WriteLine($"[DEBUG PRODUCTO]    - Promoción: {promocion.Nombre}");
                Console.WriteLine($"[DEBUG PRODUCTO]    - Precio original: ₡{precioOriginal:N2}");
                Console.WriteLine($"[DEBUG PRODUCTO]    - Descuento: {descuento}%");
                Console.WriteLine($"[DEBUG PRODUCTO]    - Precio final: ₡{precioConDescuento:N2}");
            }
            else
            {
                productoDto.TienePromocion = false;
                productoDto.PrecioConDescuento = null;
                Console.WriteLine($"[DEBUG PRODUCTO] ❌ Sin promoción para producto ID: {id}");
            }
            
            return productoDto;
        }

        // Producto con detalle extendido (incluye etiquetas, imágenes, reseñas, etc.)
        // Mapeado a ProductoDetalleDto
        public async Task<ProductoDetalleDto?> GetDetalleByIdAsync(int id)
        {
            var producto = await _repositoryProductos.GetByIdAsync(id);
            return producto != null ? _mapper.Map<ProductoDetalleDto>(producto) : null;
        }

        // Crea un nuevo producto
        public async Task<ProductoDto> CreateAsync(CrearProductoDto crearProductoDto)
        {
            var producto = _mapper.Map<Producto>(crearProductoDto);
            
            // Cargar las etiquetas por IDs
            if (crearProductoDto.EtiquetasIds?.Any() == true)
            {
                var etiquetas = await _repositoryProductos.GetEtiquetasAsync();
                producto.IdEtiqueta = etiquetas.Where(e => crearProductoDto.EtiquetasIds.Contains(e.IdEtiqueta)).ToList();
            }

            // Crear imágenes si se proporcionaron - ahora guardamos como archivos físicos
            if (crearProductoDto.ImagenesArchivos?.Any() == true)
            {
                producto.ImagenProducto = crearProductoDto.ImagenesArchivos.Select(img => new ImagenProducto
                {
                    UrlImagen = GuardarImagenEnArchivo(img.ContenidoArchivo, img.NombreArchivo)
                }).ToList();
            }

            var productoCreado = await _repositoryProductos.CreateAsync(producto);
            return _mapper.Map<ProductoDto>(productoCreado);
        }

        // Actualiza un producto existente
        public async Task<ProductoDto> UpdateAsync(EditarProductoDto editarProductoDto)
        {
            var producto = await _repositoryProductos.GetByIdAsync(editarProductoDto.IdProducto);
            if (producto == null)
                throw new ArgumentException("Producto no encontrado");

            // Actualizar propiedades básicas
            // Actualizar propiedades escalares manualmente (evitamos sobrescribir colecciones)
            producto.Nombre = editarProductoDto.Nombre;
            producto.Descripcion = editarProductoDto.Descripcion;
            producto.Precio = editarProductoDto.Precio;
            producto.Stock = editarProductoDto.Stock;
            producto.Marca = editarProductoDto.Marca;
            producto.Concentracion = editarProductoDto.Concentracion;
            producto.Genero = editarProductoDto.Genero;
            producto.IdCategoria = editarProductoDto.IdCategoria;


            // Actualizar etiquetas
            if (editarProductoDto.EtiquetasIds?.Any() == true)
            {
                var etiquetas = await _repositoryProductos.GetEtiquetasAsync();
                producto.IdEtiqueta = etiquetas.Where(e => editarProductoDto.EtiquetasIds.Contains(e.IdEtiqueta)).ToList();
            }
            else
            {
                producto.IdEtiqueta.Clear();
            }

            // Eliminar imágenes marcadas para eliminación
            if (editarProductoDto.ImagenesAEliminar?.Any() == true)
            {

                // para probar
                if (editarProductoDto.ImagenesAEliminar?.Any() == true)
                {
                    Console.WriteLine("➡ Imagenes a eliminar:");
                    foreach (var id in editarProductoDto.ImagenesAEliminar)
                        Console.WriteLine($"- ID imagen: {id}");
                }




                var imagenesAEliminar = producto.ImagenProducto
                    .Where(img => editarProductoDto.ImagenesAEliminar.Contains(img.IdImagen))
                    .ToList();

                foreach (var imagen in imagenesAEliminar)
                {
                    // Eliminar el archivo físico si existe
                    EliminarImagenArchivo(imagen.UrlImagen);
                    producto.ImagenProducto.Remove(imagen);

                    //
                    await _repositoryProductos.EliminarImagenAsync(imagen);

                }
            }

            // Agregar nuevas imágenes
            if (editarProductoDto.ImagenesNuevasArchivos?.Any() == true)
            {
                var nuevasImagenes = editarProductoDto.ImagenesNuevasArchivos.Select(img => new ImagenProducto
                {
                    UrlImagen = GuardarImagenEnArchivo(img.ContenidoArchivo, img.NombreArchivo),
                    IdProducto = producto.IdProducto
                }).ToList();

                foreach (var imagen in nuevasImagenes)
                {
                    producto.ImagenProducto.Add(imagen);
                }
            }

            var productoActualizado = await _repositoryProductos.UpdateAsync(producto);
            return _mapper.Map<ProductoDto>(productoActualizado);
        }


        // Elimina un producto por ID
        public async Task<bool> DeleteAsync(int id)
        {
            var producto = await _repositoryProductos.GetByIdAsync(id);


            if (producto != null)
            {
                // Eliminar archivos de imágenes físicas antes de eliminar el producto
                foreach (var imagen in producto.ImagenProducto)
                {
                    EliminarImagenArchivo(imagen.UrlImagen);
                }
            }
            
            return await _repositoryProductos.DeleteAsync(producto);
        }

        // Obtiene los datos necesarios para el mantenimiento
        public async Task<ProductoMantenimientoDto> GetMantenimientoDatosAsync()
        {
            var categorias = await _repositoryProductos.GetCategoriasAsync();
            var etiquetas = await _repositoryProductos.GetEtiquetasAsync();

            return new ProductoMantenimientoDto
            {
                Categorias = _mapper.Map<List<CategoriaDto>>(categorias),
                Etiquetas = _mapper.Map<List<EtiquetaDto>>(etiquetas),
                CrearProducto = new CrearProductoDto()
            };
        }

        // Obtiene los datos de un producto para edición
        public async Task<ProductoMantenimientoDto> GetForEditAsync(int id)
        {
            var producto = await _repositoryProductos.GetByIdAsync(id);
            if (producto == null)
                throw new ArgumentException("Producto no encontrado");



            // Antes del mapeo
            foreach (var img in producto.ImagenProducto)
            {
                Console.WriteLine($"🧩 EF => Id: {img.IdImagen}, Url: {img.UrlImagen}");
            }




            // DEBUG antes del mapeo
            Console.WriteLine("🔍 Imagenes crudas desde entidad:");
            foreach (var img in producto.ImagenProducto)
            {
                Console.WriteLine($"➡️ IdImagen: {img.IdImagen}, Url: {img.UrlImagen}");
            }


            var categorias = await _repositoryProductos.GetCategoriasAsync();
            var etiquetas = await _repositoryProductos.GetEtiquetasAsync();

            var editarProductoDto = _mapper.Map<EditarProductoDto>(producto);
            editarProductoDto.EtiquetasIds = producto.IdEtiqueta.Select(e => e.IdEtiqueta).ToList();
            editarProductoDto.ImagenesExistentes = _mapper.Map<List<ImagenProductoDto>>(producto.ImagenProducto);


            // Después del mapeo
            foreach (var img in editarProductoDto.ImagenesExistentes)
            {
                Console.WriteLine($"✔ DTO => Id: {img.IdImagenProducto}, Url: {img.Url}");
            }


            // DEBUG después del mapeo
            Console.WriteLine("✔ Imágenes mapeadas:");
            foreach (var dto in editarProductoDto.ImagenesExistentes)
            {
                Console.WriteLine($"✅ IdImagenProducto: {dto.IdImagenProducto}, Url: {dto.Url}");
            }

            return new ProductoMantenimientoDto
            {
                Categorias = _mapper.Map<List<CategoriaDto>>(categorias),
                Etiquetas = _mapper.Map<List<EtiquetaDto>>(etiquetas),
                EditarProducto = editarProductoDto,
                PromedioValoracion = producto.Reseña.Any() ? producto.Reseña.Average(r => r.Valoracion ?? 0) : 0,
                TotalReseñas = producto.Reseña.Count
            };
        }

        // Verifica si existe un producto con el nombre especificado
        public async Task<bool> ExistsAsync(string nombre, int? excludeId = null)
        {
            return await _repositoryProductos.ExistsAsync(nombre, excludeId);
        }

        // Método privado para guardar una imagen como archivo físico
        private string GuardarImagenEnArchivo(byte[] contenidoArchivo, string nombreArchivo)
        {
            try
            {
                // Obtener el directorio raíz de la aplicación web
                var directorioRaiz = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var directorioImagenes = Path.Combine(directorioRaiz, "img", "productos");
                
                // Crear directorio de imágenes si no existe
                if (!Directory.Exists(directorioImagenes))
                {
                    Directory.CreateDirectory(directorioImagenes);
                }

                // Generar nombre único para evitar colisiones
                var extension = Path.GetExtension(nombreArchivo) ?? ".jpg";
                var nombreUnico = $"{Guid.NewGuid()}{extension}";
                var rutaCompleta = Path.Combine(directorioImagenes, nombreUnico);

                // Guardar el archivo
                File.WriteAllBytes(rutaCompleta, contenidoArchivo);

                // Retornar la ruta relativa para almacenar en la BD
                return $"img/productos/{nombreUnico}";
            }
            catch (Exception ex)
            {
                // En caso de error, log y retornar null o manejar según sea necesario
                throw new Exception($"Error al guardar imagen: {ex.Message}");
            }
        }

        // Método privado para eliminar un archivo de imagen
        private void EliminarImagenArchivo(string rutaImagen)
        {
            try
            {
                // Solo eliminar si es una ruta de archivo (no base64)
                if (!string.IsNullOrEmpty(rutaImagen) && rutaImagen.StartsWith("img/"))
                {
                    var directorioRaiz = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    var rutaCompleta = Path.Combine(directorioRaiz, rutaImagen);
                    if (File.Exists(rutaCompleta))
                    {
                        File.Delete(rutaCompleta);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log el error pero no fallar el proceso principal
                // En producción, usar un logger apropiado
                Console.WriteLine($"Error al eliminar imagen: {ex.Message}");
            }
        }
    }
}
