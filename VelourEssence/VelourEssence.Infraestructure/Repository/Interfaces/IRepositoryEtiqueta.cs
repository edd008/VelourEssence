using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VelourEssence.Infraestructure.Models;

namespace VelourEssence.Infraestructure.Repository.Interfaces
{
    // Interfaz para operaciones de repositorio de etiquetas
    public interface IRepositoryEtiqueta
    {
        // Obtiene todas las etiquetas disponibles
        Task<ICollection<Etiqueta>> ListAsync();

        // Busca una etiqueta por ID, retorna null si no existe
        Task<Etiqueta?> GetByIdAsync(int id);

        // Crea una nueva etiqueta
        Task<Etiqueta> AddAsync(Etiqueta etiqueta);

        // Actualiza una etiqueta existente
        Task<bool> UpdateAsync(Etiqueta etiqueta);

        // Elimina una etiqueta por ID
        Task<bool> DeleteAsync(int id);
    }
}