using VelourEssence.Infraestructure.Data;
using VelourEssence.Infraestructure.Models;
using VelourEssence.Infraestructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace VelourEssence.Infraestructure.Repository.Implementations
{
    public class RepositoryProductoPersonalizado : IRepositoryProductoPersonalizado
    {
        private readonly VelourEssenceContext _context;

        public RepositoryProductoPersonalizado(VelourEssenceContext context)
        {
            _context = context;
        }

        public async Task<ProductoPersonalizado?> GetAsync(int id)
        {
            return await _context.ProductoPersonalizado.FindAsync(id);
        }

        public async Task<List<ProductoPersonalizado>> GetAllAsync()
        {
            return await _context.ProductoPersonalizado.ToListAsync();
        }

        public async Task<ProductoPersonalizado> CreateAsync(ProductoPersonalizado entity)
        {
            _context.ProductoPersonalizado.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<ProductoPersonalizado> UpdateAsync(ProductoPersonalizado entity)
        {
            _context.ProductoPersonalizado.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetAsync(id);
            if (entity == null) return false;
            
            _context.ProductoPersonalizado.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ProductoPersonalizado>> GetPorUsuarioAsync(int idUsuario)
        {
            return await _context.ProductoPersonalizado
                .Where(p => p.IdUsuario == idUsuario && p.Activo)
                .Include(p => p.BaseProducto)
                .Include(p => p.IdUsuarioNavigation)
                .Include(p => p.ProductoPersonalizadoDetalles)
                    .ThenInclude(d => d.IdCriterioNavigation)
                .Include(p => p.ProductoPersonalizadoDetalles)
                    .ThenInclude(d => d.IdOpcionNavigation)
                .OrderByDescending(p => p.FechaCreacion)
                .ToListAsync();
        }

        public async Task<ProductoPersonalizado?> GetConDetallesAsync(int id)
        {
            return await _context.ProductoPersonalizado
                .Include(p => p.BaseProducto)
                    .ThenInclude(pb => pb!.ImagenProducto)
                .Include(p => p.IdUsuarioNavigation)
                .Include(p => p.ProductoPersonalizadoDetalles)
                    .ThenInclude(d => d.IdCriterioNavigation)
                .Include(p => p.ProductoPersonalizadoDetalles)
                    .ThenInclude(d => d.IdOpcionNavigation)
                .FirstOrDefaultAsync(p => p.IdProductoPersonalizado == id);
        }

        public async Task<List<ProductoPersonalizado>> GetActivosAsync()
        {
            return await _context.ProductoPersonalizado
                .Where(p => p.Activo)
                .Include(p => p.BaseProducto)
                .Include(p => p.IdUsuarioNavigation)
                .OrderByDescending(p => p.FechaCreacion)
                .ToListAsync();
        }

        public async Task CrearDetallesAsync(List<ProductoPersonalizadoDetalle> detalles)
        {
            _context.ProductoPersonalizadoDetalles.AddRange(detalles);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductoPersonalizadoDetalle>> GetDetallesPorProductoAsync(int idProductoPersonalizado)
        {
            return await _context.ProductoPersonalizadoDetalles
                .Where(d => d.IdProductoPersonalizado == idProductoPersonalizado)
                .Include(d => d.IdCriterioNavigation)
                .Include(d => d.IdOpcionNavigation)
                .ToListAsync();
        }
    }
}
