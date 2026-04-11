using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VelourEssence.Infraestructure.Data;
using VelourEssence.Infraestructure.Models;
using VelourEssence.Infraestructure.Repository.Interfaces;

namespace VelourEssence.Infraestructure.Repository.Implementations
{
    // Repositorio para manejar productos del sistema
    public class RepositoryProducto : IRepositoryProductos
    {
        private readonly VelourEssenceContext _context;

        // Constructor con inyección del contexto de BD
        public RepositoryProducto(VelourEssenceContext context)
        {
            _context = context;
        }

        // Obtiene todos los productos con imágenes y promociones
        public async Task<ICollection<Producto>> ListAsync()
        {
            return await _context.Producto
                .Include(p => p.ImagenProducto)
                .Include(p => p.Promocion)
                .Include(p => p.IdCategoriaNavigation!)
                    .ThenInclude(c => c.Promocion)
                .ToListAsync();
        }

        // Busca un producto por ID con todas sus relaciones
        public async Task<Producto?> GetByIdAsync(int id)
        {
            return await _context.Producto
                .Include(p => p.ImagenProducto)
                .Include(p => p.Promocion)
                .Include(p => p.IdCategoriaNavigation!)
                    .ThenInclude(c => c.Promocion)
                .Include(p => p.IdEtiqueta)
                .Include(p => p.Reseña)
                    .ThenInclude(r => r.IdUsuarioNavigation)
                .FirstOrDefaultAsync(p => p.IdProducto == id);
        }

        // Crea un nuevo producto
        public async Task<Producto> CreateAsync(Producto producto)
        {
            _context.Producto.Add(producto);
            await _context.SaveChangesAsync();
            return producto;
        }

        // Actualiza un producto existente 19072025
        public async Task<Producto> UpdateAsync(Producto producto)
        {
            var productoExistente = await _context.Producto
                .Include(p => p.ImagenProducto)
                .FirstOrDefaultAsync(p => p.IdProducto == producto.IdProducto);

            if (productoExistente == null)
                throw new ArgumentException("Producto no encontrado en la base de datos");

            // 1. Actualiza los campos básicos del producto
            _context.Entry(productoExistente).CurrentValues.SetValues(producto);

            // 2. Agrega nuevas imágenes (solo las que no tienen ID)
            foreach (var nuevaImagen in producto.ImagenProducto)
            {
                if (nuevaImagen.IdImagen == 0)
                {
                    productoExistente.ImagenProducto.Add(nuevaImagen);
                }
            }

            await _context.SaveChangesAsync();
            return productoExistente;
        }


        //

        // Elimina un producto por ID
        public async Task<bool> DeleteAsync(Producto producto)
        {
            _context.Producto.Remove(producto);
            await _context.SaveChangesAsync();
            return true;
        }


        // Verifica si existe un producto con el nombre especificado
        public async Task<bool> ExistsAsync(string nombre, int? excludeId = null)
        {
            var query = _context.Producto.Where(p => p.Nombre == nombre);
            if (excludeId.HasValue)
                query = query.Where(p => p.IdProducto != excludeId.Value);
            
            return await query.AnyAsync();
        }

        // Obtiene las categorías disponibles
        public async Task<ICollection<Categoria>> GetCategoriasAsync()
        {
            return await _context.Categoria.ToListAsync();
        }

        // Obtiene las etiquetas disponibles
        public async Task<ICollection<Etiqueta>> GetEtiquetasAsync()
        {
            return await _context.Etiqueta.ToListAsync();
        }

        // Guarda los cambios en la base de datos
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();

        }


        public async Task EliminarImagenAsync(ImagenProducto imagen)
        {
            _context.ImagenProducto.Remove(imagen);
            await _context.SaveChangesAsync();
        }

        // Obtiene la promoción activa para un producto específico
        public async Task<Promocion?> GetPromocionActivaAsync(int idProducto)
        {
            try
            {
                Console.WriteLine($"[DEBUG REPO PRODUCTO] Buscando promoción activa para producto: {idProducto}");
                
                var fechaActual = DateOnly.FromDateTime(DateTime.Now);
                
                // Primero buscar el producto para obtener su categoría
                var producto = await _context.Producto.FindAsync(idProducto);
                if (producto == null)
                {
                    Console.WriteLine($"[DEBUG REPO PRODUCTO] ❌ Producto no encontrado: {idProducto}");
                    return null;
                }
                
                Console.WriteLine($"[DEBUG REPO PRODUCTO] Producto encontrado: {producto.Nombre}, Categoría: {producto.IdCategoria}");
                
                // Buscar promoción específica del producto primero (mayor prioridad)
                var promocionProducto = await _context.Promocion
                    .Where(p => p.IdProducto == idProducto && 
                               p.FechaInicio <= fechaActual && 
                               p.FechaFin >= fechaActual)
                    .FirstOrDefaultAsync();
                
                if (promocionProducto != null)
                {
                    Console.WriteLine($"[DEBUG REPO PRODUCTO] ✅ Promoción específica de producto encontrada: {promocionProducto.Nombre}, Descuento: {promocionProducto.PorcentajeDescuento}%");
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
                    Console.WriteLine($"[DEBUG REPO PRODUCTO] ✅ Promoción de categoría encontrada: {promocionCategoria.Nombre}, Descuento: {promocionCategoria.PorcentajeDescuento}%");
                    return promocionCategoria;
                }
                
                Console.WriteLine($"[DEBUG REPO PRODUCTO] ❌ No hay promoción activa para producto ID: {idProducto}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR REPO PRODUCTO] Error en GetPromocionActivaAsync: {ex.Message}");
                return null;
            }
        }

    }
}