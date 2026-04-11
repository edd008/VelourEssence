using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VelourEssence.Infraestructure.Models;

namespace VelourEssence.Infraestructure.Repository.Interfaces
{
    // Interfaz para operaciones de repositorio de promociones
    public interface IRepositoryPromocion
    {
        // Obtiene todas las promociones disponibles
        Task<ICollection<Promocion>> ListAsync();

        // Busca una promoción por ID, retorna null si no existe
        Task<Promocion?> GetByIdAsync(int id);

        // Crea una nueva promoción
        Task<bool> CreateAsync(Promocion promocion);

        // Actualiza una promoción existente
        Task<bool> UpdateAsync(Promocion promocion);

        // Elimina una promoción
        Task<bool> DeleteAsync(int id);

        // Obtiene todas las categorías
        Task<ICollection<Categoria>> GetCategoriasAsync();
    }
}