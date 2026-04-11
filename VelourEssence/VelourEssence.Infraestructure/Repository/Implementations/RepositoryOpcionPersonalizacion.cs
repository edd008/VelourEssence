using VelourEssence.Infraestructure.Data;
using VelourEssence.Infraestructure.Models;
using VelourEssence.Infraestructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace VelourEssence.Infraestructure.Repository.Implementations
{
    public class RepositoryOpcionPersonalizacion : IRepositoryOpcionPersonalizacion
    {
        private readonly VelourEssenceContext _context;

        public RepositoryOpcionPersonalizacion(VelourEssenceContext context)
        {
            _context = context;
        }

        public async Task<OpcionPersonalizacion?> GetAsync(int id)
        {
            return await _context.OpcionesPersonalizacion.FindAsync(id);
        }

        public async Task<List<OpcionPersonalizacion>> GetAllAsync()
        {
            return await _context.OpcionesPersonalizacion.ToListAsync();
        }

        public async Task<OpcionPersonalizacion> CreateAsync(OpcionPersonalizacion entity)
        {
            _context.OpcionesPersonalizacion.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<OpcionPersonalizacion> UpdateAsync(OpcionPersonalizacion entity)
        {
            _context.OpcionesPersonalizacion.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetAsync(id);
            if (entity == null) return false;
            
            _context.OpcionesPersonalizacion.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<OpcionPersonalizacion>> GetOpcionesPorCriterioAsync(int idCriterio)
        {
            return await _context.OpcionesPersonalizacion
                .Where(o => o.IdCriterio == idCriterio && o.Activo)
                .OrderBy(o => o.Orden)
                .ToListAsync();
        }

        public async Task<List<OpcionPersonalizacion>> GetOpcionesActivasAsync()
        {
            return await _context.OpcionesPersonalizacion
                .Where(o => o.Activo)
                .Include(o => o.IdCriterioNavigation)
                .OrderBy(o => o.IdCriterio)
                .ThenBy(o => o.Orden)
                .ToListAsync();
        }

        public async Task<OpcionPersonalizacion?> GetPorCriterioYNombreAsync(int idCriterio, string nombre)
        {
            return await _context.OpcionesPersonalizacion
                .FirstOrDefaultAsync(o => o.IdCriterio == idCriterio && 
                                         (o.Valor == nombre || o.Etiqueta == nombre) && 
                                         o.Activo);
        }
    }
}
