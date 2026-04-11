
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VelourEssence.Infraestructure.Data;
using VelourEssence.Infraestructure.Models;
using VelourEssence.Infraestructure.Repository.Interfaces;

namespace VelourEssence.Infraestructure.Repository.Implementations
{
    // Repositorio para manejar las etiquetas de productos
        public class RepositoryEtiqueta : IRepositoryEtiqueta
    {
        private readonly VelourEssenceContext _context;

        // Constructor con inyección del contexto de BD
    public RepositoryEtiqueta(VelourEssenceContext context)
        {
            _context = context;
        }

        // Obtiene todas las etiquetas
        public async Task<ICollection<Etiqueta>> ListAsync()
        {
            return await _context.Etiqueta.ToListAsync();
        }

        // Busca una etiqueta por ID
        public async Task<Etiqueta?> GetByIdAsync(int id)
        {
            return await _context.Etiqueta.FirstOrDefaultAsync(e => e.IdEtiqueta == id);
        }

        // Crea una nueva etiqueta
        public async Task<Etiqueta> AddAsync(Etiqueta etiqueta)
        {
            _context.Etiqueta.Add(etiqueta);
            await _context.SaveChangesAsync();
            return etiqueta;
        }

        // Actualiza una etiqueta existente
        public async Task<bool> UpdateAsync(Etiqueta etiqueta)
        {
            var existing = await _context.Etiqueta.FindAsync(etiqueta.IdEtiqueta);
            if (existing == null)
                return false;
            existing.Nombre = etiqueta.Nombre;
            await _context.SaveChangesAsync();
            return true;
        }
        // Elimina una etiqueta por ID
        public async Task<bool> DeleteAsync(int id)
        {
            var etiqueta = await _context.Etiqueta.FindAsync(id);
            if (etiqueta == null)
                return false;
            _context.Etiqueta.Remove(etiqueta);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}