using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VelourEssence.Infraestructure.Models;
using Microsoft.EntityFrameworkCore;


namespace VelourEssence.Infraestructure.Repository.Interfaces
{
    // Interfaz para operaciones de repositorio de reseñas
    public interface IRepositoryReseña
    {
        // Obtiene todas las reseñas disponibles
        Task<ICollection<Reseña>> ListAsync();

        // Busca una reseña por ID, retorna null si no existe
        Task<Reseña?> GetByIdAsync(int id);

        // Crea una nueva reseña
        Task<Reseña> CreateAsync(Reseña reseña);

        // Obtiene reseñas por ID de producto
        Task<ICollection<Reseña>> GetByProductIdAsync(int productId);

        // Calcula el promedio de valoraciones de un producto
        Task<double> GetAverageRatingByProductIdAsync(int productId);

        Task<Reseña?> GetByIdWithIncludesAsync(int id);

    }
}