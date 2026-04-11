using VelourEssence.Infraestructure.Data;
using VelourEssence.Infraestructure.Models;
using VelourEssence.Infraestructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace VelourEssence.Infraestructure.Repository.Implementations
{
    public class RepositoryCriterioPersonalizacion : IRepositoryCriterioPersonalizacion
    {
        private readonly VelourEssenceContext _context;

        public RepositoryCriterioPersonalizacion(VelourEssenceContext context)
        {
            _context = context;
        }

        public async Task<CriterioPersonalizacion?> GetAsync(int id)
        {
            return await _context.CriteriosPersonalizacion.FindAsync(id);
        }

        public async Task<List<CriterioPersonalizacion>> GetAllAsync()
        {
            return await _context.CriteriosPersonalizacion.ToListAsync();
        }

        public async Task<CriterioPersonalizacion> CreateAsync(CriterioPersonalizacion entity)
        {
            _context.CriteriosPersonalizacion.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<CriterioPersonalizacion> UpdateAsync(CriterioPersonalizacion entity)
        {
            _context.CriteriosPersonalizacion.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetAsync(id);
            if (entity == null) return false;
            
            _context.CriteriosPersonalizacion.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<CriterioPersonalizacion>> GetCriteriosActivosAsync()
        {
            return await _context.CriteriosPersonalizacion
                .Where(c => c.Activo)
                .Include(c => c.Opciones.Where(o => o.Activo))
                .OrderBy(c => c.Orden)
                .ToListAsync();
        }

        public async Task<List<CriterioPersonalizacion>> GetCriteriosPorProductoAsync(int idProducto)
        {
            return await _context.CriteriosPersonalizacion
                .Where(c => c.Activo && c.ProductoId == idProducto)
                .Include(c => c.Opciones.Where(o => o.Activo))
                .OrderBy(c => c.Orden)
                .ToListAsync();
        }

        public async Task<CriterioPersonalizacion?> GetConOpcionesAsync(int id)
        {
            return await _context.CriteriosPersonalizacion
                .Include(c => c.Opciones.Where(o => o.Activo))
                .FirstOrDefaultAsync(c => c.IdCriterio == id);
        }
    }
}
