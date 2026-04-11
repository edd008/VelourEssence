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
    // Repositorio para manejar reseñas de productos
    public class RepositoryReseña : IRepositoryReseña
    {
        private readonly VelourEssenceContext _context;

        // Constructor con inyección del contexto de BD
        public RepositoryReseña(VelourEssenceContext context)
        {
            _context = context;
        }

        // Obtiene todas las reseñas con producto y usuario relacionados
        public async Task<ICollection<Reseña>> ListAsync()
        {
            return await _context.Reseña
                .Include(r => r.IdProductoNavigation)
                .Include(r => r.IdUsuarioNavigation)
                .ToListAsync();
        }

        // Busca una reseña por ID con producto y usuario relacionados
        public async Task<Reseña?> GetByIdAsync(int id)
        {
            return await _context.Reseña
                .Include(r => r.IdProductoNavigation)
                .Include(r => r.IdUsuarioNavigation)
                .FirstOrDefaultAsync(r => r.IdReseña == id);
        }

        // Crea una nueva reseña en la base de datos
        public async Task<Reseña> CreateAsync(Reseña reseña)
        {
            _context.Reseña.Add(reseña);
            await _context.SaveChangesAsync();

            // Cargar las navegaciones después de guardar
            await _context.Entry(reseña)
                .Reference(r => r.IdProductoNavigation)
                .LoadAsync();
            await _context.Entry(reseña)
                .Reference(r => r.IdUsuarioNavigation)
                .LoadAsync();

            return reseña;
        }

        // Obtiene reseñas por ID de producto
        public async Task<ICollection<Reseña>> GetByProductIdAsync(int productId)
        {
            return await _context.Reseña
                .Include(r => r.IdUsuarioNavigation)
                .Where(r => r.IdProducto == productId)
                .OrderByDescending(r => r.Fecha)
                .ToListAsync();
        }

        // Calcula el promedio de valoraciones de un producto
        public async Task<double> GetAverageRatingByProductIdAsync(int productId)
        {
            var ratings = await _context.Reseña
                .Where(r => r.IdProducto == productId && r.Valoracion.HasValue)
                .Select(r => r.Valoracion!.Value)
                .ToListAsync();

            return ratings.Any() ? ratings.Average() : 0;
        }


        public async Task<Reseña?> GetByIdWithIncludesAsync(int id)
        {
            return await _context.Reseña
                .Include(r => r.IdUsuarioNavigation)
                .Include(r => r.IdProductoNavigation)
                .FirstOrDefaultAsync(r => r.IdReseña == id);
        }

    }
}