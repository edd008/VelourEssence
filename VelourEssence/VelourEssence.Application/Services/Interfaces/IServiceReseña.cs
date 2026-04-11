using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VelourEssence.Application.DTOs;

namespace VelourEssence.Application.Services.Interfaces
{
    // Interfaz para servicios de gestión de reseñas
    public interface IServiceReseña
    {
        // Obtiene todas las reseñas disponibles
        Task<ICollection<ReseñaDto>> ListAsync();

        // Busca una reseña por ID, retorna null si no existe
        Task<ReseñaDto?> GetByIdAsync(int id);

        // Crea una nueva reseña
        Task<ReseñaDto> CreateAsync(CrearReseñaDto crearReseñaDto);

        // Obtiene reseñas por ID de producto
        Task<ICollection<ReseñaDto>> GetByProductIdAsync(int productId);

        // Calcula el promedio de valoraciones de un producto
        Task<double> GetAverageRatingByProductIdAsync(int productId);
    }
}