using Microsoft.EntityFrameworkCore;
using VelourEssence.Infraestructure.Data;
using VelourEssence.Infraestructure.Models;
using VelourEssence.Infraestructure.Repository.Interfaces;

namespace VelourEssence.Infraestructure.Repository.Implementations
{
    // Repositorio para manejar promociones del sistema
    public class RepositoryPromocion : IRepositoryPromocion
    {
        private readonly VelourEssenceContext _context;

        // Constructor con inyección del contexto de BD
        public RepositoryPromocion(VelourEssenceContext context)
        {
            _context = context;
        }

        // Obtiene todas las promociones con sus productos y categorías relacionados
        public async Task<ICollection<Promocion>> ListAsync()
        {
            return await _context.Promocion
                .Include(p => p.IdProductoNavigation)
                .Include(p => p.IdCategoriaNavigation)
                .ToListAsync();
        }

        // Busca una promoción por ID con el producto y categoría relacionados
        public async Task<Promocion?> GetByIdAsync(int id)
        {
            return await _context.Promocion
                .Include(p => p.IdProductoNavigation)
                .Include(p => p.IdCategoriaNavigation)
                .FirstOrDefaultAsync(p => p.IdPromocion == id);
        }

        // Crea una nueva promoción
        public async Task<bool> CreateAsync(Promocion promocion)
        {
            try
            {
                _context.Promocion.Add(promocion);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Actualiza una promoción existente
        public async Task<bool> UpdateAsync(Promocion promocion)
        {
            try
            {
                _context.Promocion.Update(promocion);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Elimina una promoción
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var promocion = await _context.Promocion.FindAsync(id);
                if (promocion != null)
                {
                    _context.Promocion.Remove(promocion);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // Obtiene todas las categorías
        public async Task<ICollection<Categoria>> GetCategoriasAsync()
        {
            return await _context.Categoria.ToListAsync();
        }
    }
}