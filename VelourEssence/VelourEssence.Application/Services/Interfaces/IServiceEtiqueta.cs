using VelourEssence.Application.DTOs;

namespace VelourEssence.Application.Services.Interfaces
{
    // Interfaz para servicios de gestión de etiquetas
    public interface IServiceEtiqueta
    {
        // Obtiene todas las etiquetas disponibles
        Task<ICollection<EtiquetaDto>> ListAsync();

        // Busca una etiqueta por ID, retorna null si no existe
        Task<EtiquetaDto?> GetByIdAsync(int id);

        // Crea una nueva etiqueta
        Task<EtiquetaDto> CreateAsync(EtiquetaDto dto);

        // Actualiza una etiqueta existente
        Task<bool> UpdateAsync(EtiquetaDto dto);

        // Elimina una etiqueta por ID
        Task<bool> DeleteAsync(int id);
    }
}