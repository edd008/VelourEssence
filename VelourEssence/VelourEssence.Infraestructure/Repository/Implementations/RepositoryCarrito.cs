using Microsoft.EntityFrameworkCore;
using VelourEssence.Infraestructure.Data;
using VelourEssence.Infraestructure.Models;
using VelourEssence.Infraestructure.Repository.Interfaces;

namespace VelourEssence.Infraestructure.Repository.Implementations
{
    public class RepositoryCarrito : IRepositoryCarrito
    {
        private readonly VelourEssenceContext _context;

        public RepositoryCarrito(VelourEssenceContext context)
        {
            _context = context;
        }

        public async Task<Carrito?> GetByUsuarioIdAsync(int idUsuario)
        {
            try
            {
                Console.WriteLine($"[DEBUG REPO] Buscando carrito para usuario: {idUsuario}");
                
                // Consulta simplificada sin Include por ahora
                var carrito = await _context.Carrito
                    .FirstOrDefaultAsync(c => c.IdUsuario == idUsuario && (c.Activo == true || c.Activo == null));
                
                Console.WriteLine($"[DEBUG REPO] Carrito encontrado: {carrito?.IdCarrito ?? 0}");
                return carrito;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR REPO] Error en GetByUsuarioIdAsync: {ex.Message}");
                Console.WriteLine($"[ERROR REPO] Stack: {ex.StackTrace}");
                return null;
            }
        }

        public async Task<Carrito?> GetByIdAsync(int idCarrito)
        {
            try
            {
                Console.WriteLine($"[DEBUG REPO] Buscando carrito por ID: {idCarrito}");
                
                var carrito = await _context.Carrito
                    .FirstOrDefaultAsync(c => c.IdCarrito == idCarrito);
                
                Console.WriteLine($"[DEBUG REPO] Carrito encontrado: {carrito?.IdCarrito ?? 0}");
                return carrito;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR REPO] Error en GetByIdAsync: {ex.Message}");
                Console.WriteLine($"[ERROR REPO] Stack: {ex.StackTrace}");
                return null;
            }
        }

        public async Task<Carrito> CreateAsync(Carrito carrito)
        {
            try
            {
                Console.WriteLine($"[DEBUG REPO] Creando carrito para usuario: {carrito.IdUsuario}");
                
                _context.Carrito.Add(carrito);
                await _context.SaveChangesAsync();
                
                Console.WriteLine($"[DEBUG REPO] Carrito creado con ID: {carrito.IdCarrito}");
                return carrito;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR REPO] Error en CreateAsync: {ex.Message}");
                Console.WriteLine($"[ERROR REPO] Stack: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<Carrito> UpdateAsync(Carrito carrito)
        {
            _context.Carrito.Update(carrito);
            await _context.SaveChangesAsync();
            return carrito;
        }

        public async Task<bool> DeleteAsync(Carrito carrito)
        {
            try
            {
                _context.Carrito.Remove(carrito);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<CarritoProducto?> GetCarritoProductoAsync(int idCarrito, int idProducto)
        {
            try
            {
                Console.WriteLine($"[DEBUG REPO] Buscando producto {idProducto} en carrito {idCarrito}");
                
                var item = await _context.CarritoProducto
                    .FirstOrDefaultAsync(cp => cp.IdCarrito == idCarrito && cp.IdProducto == idProducto);
                
                Console.WriteLine($"[DEBUG REPO] Item encontrado: {item != null}");
                return item;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR REPO] Error en GetCarritoProductoAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<CarritoProducto> AddProductoAsync(CarritoProducto carritoProducto)
        {
            try
            {
                Console.WriteLine($"[DEBUG REPO] Agregando producto al carrito - Carrito: {carritoProducto.IdCarrito}, Producto: {carritoProducto.IdProducto}, Cantidad: {carritoProducto.Cantidad}");
                
                _context.CarritoProducto.Add(carritoProducto);
                await _context.SaveChangesAsync();
                
                Console.WriteLine($"[DEBUG REPO] Producto agregado exitosamente");
                return carritoProducto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR REPO] Error en AddProductoAsync: {ex.Message}");
                Console.WriteLine($"[ERROR REPO] Stack: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<CarritoProducto> UpdateProductoAsync(CarritoProducto carritoProducto)
        {
            _context.CarritoProducto.Update(carritoProducto);
            await _context.SaveChangesAsync();
            return carritoProducto;
        }

        public async Task<bool> RemoveProductoAsync(CarritoProducto carritoProducto)
        {
            try
            {
                _context.CarritoProducto.Remove(carritoProducto);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<CarritoProductoPersonalizado?> GetCarritoProductoPersonalizadoAsync(int idCarrito, int idProductoPersonalizado)
        {
            return await _context.CarritoProductoPersonalizado
                .FirstOrDefaultAsync(cpp => cpp.IdCarrito == idCarrito && cpp.IdProductoPersonalizado == idProductoPersonalizado);
        }

        public async Task<CarritoProductoPersonalizado> AddProductoPersonalizadoAsync(CarritoProductoPersonalizado carritoProductoPersonalizado)
        {
            _context.CarritoProductoPersonalizado.Add(carritoProductoPersonalizado);
            await _context.SaveChangesAsync();
            return carritoProductoPersonalizado;
        }

        public async Task<CarritoProductoPersonalizado> UpdateProductoPersonalizadoAsync(CarritoProductoPersonalizado carritoProductoPersonalizado)
        {
            _context.CarritoProductoPersonalizado.Update(carritoProductoPersonalizado);
            await _context.SaveChangesAsync();
            return carritoProductoPersonalizado;
        }

        public async Task<bool> RemoveProductoPersonalizadoAsync(CarritoProductoPersonalizado carritoProductoPersonalizado)
        {
            try
            {
                _context.CarritoProductoPersonalizado.Remove(carritoProductoPersonalizado);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<CarritoProducto>> GetProductosCarritoAsync(int idCarrito)
        {
            try
            {
                Console.WriteLine($"[DEBUG REPO] GetProductosCarritoAsync para carrito: {idCarrito}");
                
                var productos = await _context.CarritoProducto
                    .Where(cp => cp.IdCarrito == idCarrito)
                    .ToListAsync();
                
                Console.WriteLine($"[DEBUG REPO] Productos encontrados: {productos.Count}");
                return productos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR REPO] Error en GetProductosCarritoAsync: {ex.Message}");
                return new List<CarritoProducto>();
            }
        }

        public async Task<List<CarritoProductoPersonalizado>> GetProductosPersonalizadosCarritoAsync(int idCarrito)
        {
            try
            {
                Console.WriteLine($"[DEBUG REPO] GetProductosPersonalizadosCarritoAsync para carrito: {idCarrito}");
                
                var productos = await _context.CarritoProductoPersonalizado
                    .Where(cpp => cpp.IdCarrito == idCarrito)
                    .ToListAsync();
                
                Console.WriteLine($"[DEBUG REPO] Productos personalizados encontrados: {productos.Count}");
                return productos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR REPO] Error en GetProductosPersonalizadosCarritoAsync: {ex.Message}");
                return new List<CarritoProductoPersonalizado>();
            }
        }

        public async Task<bool> LimpiarCarritoAsync(int idCarrito)
        {
            try
            {
                var productos = await _context.CarritoProducto
                    .Where(cp => cp.IdCarrito == idCarrito)
                    .ToListAsync();

                var productosPersonalizados = await _context.CarritoProductoPersonalizado
                    .Where(cpp => cpp.IdCarrito == idCarrito)
                    .ToListAsync();

                _context.CarritoProducto.RemoveRange(productos);
                _context.CarritoProductoPersonalizado.RemoveRange(productosPersonalizados);
                
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Producto?> GetProductoByIdAsync(int idProducto)
        {
            try
            {
                Console.WriteLine($"[DEBUG REPO] GetProductoByIdAsync para producto: {idProducto}");
                
                var producto = await _context.Producto.FindAsync(idProducto);
                
                if (producto != null)
                {
                    Console.WriteLine($"[DEBUG REPO] Producto encontrado: {producto.Nombre}, Precio: {producto.Precio}");
                }
                else
                {
                    Console.WriteLine($"[DEBUG REPO] Producto no encontrado ID: {idProducto}");
                }
                
                return producto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR REPO] Error en GetProductoByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<string> GetPrimeraImagenProductoAsync(int idProducto)
        {
            try
            {
                Console.WriteLine($"[DEBUG REPO] GetPrimeraImagenProductoAsync para producto: {idProducto}");
                
                var imagen = await _context.ImagenProducto
                    .Where(img => img.IdProducto == idProducto)
                    .FirstOrDefaultAsync();
                
                if (imagen != null && !string.IsNullOrEmpty(imagen.UrlImagen))
                {
                    // Asegurar que la URL empiece con "/"
                    var url = imagen.UrlImagen;
                    if (!url.StartsWith("/"))
                    {
                        url = "/" + url;
                    }
                    Console.WriteLine($"[DEBUG REPO] Imagen encontrada: {url}");
                    return url;
                }
                else
                {
                    Console.WriteLine($"[DEBUG REPO] No hay imágenes para producto ID: {idProducto}");
                    return "/images/no-image.jpg";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR REPO] Error en GetPrimeraImagenProductoAsync: {ex.Message}");
                return "/images/no-image.jpg";
            }
        }

        public async Task<Promocion?> GetPromocionActivaProductoAsync(int idProducto)
        {
            try
            {
                Console.WriteLine($"[DEBUG REPO] GetPromocionActivaProductoAsync para producto: {idProducto}");
                
                var fechaActual = DateOnly.FromDateTime(DateTime.Now);
                
                // Primero buscar el producto para obtener su categoría
                var producto = await _context.Producto.FindAsync(idProducto);
                if (producto == null)
                {
                    Console.WriteLine($"[DEBUG REPO] ❌ Producto no encontrado: {idProducto}");
                    return null;
                }
                
                Console.WriteLine($"[DEBUG REPO] Producto encontrado: {producto.Nombre}, Categoría: {producto.IdCategoria}");
                
                // Buscar promoción específica del producto primero (mayor prioridad)
                var promocionProducto = await _context.Promocion
                    .Where(p => p.IdProducto == idProducto && 
                               p.FechaInicio <= fechaActual && 
                               p.FechaFin >= fechaActual)
                    .FirstOrDefaultAsync();
                
                if (promocionProducto != null)
                {
                    Console.WriteLine($"[DEBUG REPO] ✅ Promoción específica de producto encontrada: {promocionProducto.Nombre}, Descuento: {promocionProducto.PorcentajeDescuento}%");
                    return promocionProducto;
                }
                
                // Si no hay promoción específica del producto, buscar por categoría
                var promocionCategoria = await _context.Promocion
                    .Where(p => p.IdCategoria == producto.IdCategoria && 
                               p.FechaInicio <= fechaActual && 
                               p.FechaFin >= fechaActual)
                    .FirstOrDefaultAsync();
                
                if (promocionCategoria != null)
                {
                    Console.WriteLine($"[DEBUG REPO] ✅ Promoción de categoría encontrada: {promocionCategoria.Nombre}, Descuento: {promocionCategoria.PorcentajeDescuento}%");
                    return promocionCategoria;
                }
                
                Console.WriteLine($"[DEBUG REPO] ❌ No hay promoción activa para producto ID: {idProducto}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR REPO] Error en GetPromocionActivaProductoAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<ProductoPersonalizado?> GetProductoPersonalizadoByIdAsync(int idProductoPersonalizado)
        {
            try
            {
                Console.WriteLine($"[DEBUG REPO] GetProductoPersonalizadoByIdAsync para producto personalizado: {idProductoPersonalizado}");
                
                var productoPersonalizado = await _context.ProductoPersonalizado
                    .Include(pp => pp.BaseProducto)
                    .Where(pp => pp.IdProductoPersonalizado == idProductoPersonalizado)
                    .FirstOrDefaultAsync();
                
                if (productoPersonalizado != null)
                {
                    Console.WriteLine($"[DEBUG REPO] Producto personalizado encontrado: {productoPersonalizado.Descripcion}, Precio: {productoPersonalizado.PrecioFinal}");
                }
                else
                {
                    Console.WriteLine($"[DEBUG REPO] Producto personalizado no encontrado ID: {idProductoPersonalizado}");
                }
                
                return productoPersonalizado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR REPO] Error en GetProductoPersonalizadoByIdAsync: {ex.Message}");
                return null;
            }
        }
    }
}
