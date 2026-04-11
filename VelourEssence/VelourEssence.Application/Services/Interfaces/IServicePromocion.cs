using VelourEssence.Application.DTOs;

namespace VelourEssence.Application.Services.Interfaces
{
    // Interfaz para servicios de gestión de promociones
    public interface IServicePromocion
    {
        // Obtiene todas las promociones disponibles
        Task<ICollection<PromocionDto>> ListAsync();

        // Busca una promoción por ID, retorna null si no existe
        Task<PromocionDto?> GetByIdAsync(int id);

        // Obtiene una promoción para editar
        Task<CrearPromocionDto?> GetForEditAsync(int id);

        // Obtiene solo las promociones vigentes en la fecha actual
        Task<ICollection<PromocionDto>> ListVigentesAsync();

        // Crea una nueva promoción
        Task<bool> CreateAsync(CrearPromocionDto promocionDto);

        // Actualiza una promoción existente
        Task<bool> UpdateAsync(CrearPromocionDto promocionDto);

        // Elimina una promoción
        Task<bool> DeleteAsync(int id);

        // Obtiene todas las categorías para dropdowns
        Task<ICollection<CategoriaDto>> GetCategoriasAsync();

        // Obtiene todos los productos para dropdowns
        Task<ICollection<ProductoDto>> GetProductosAsync();
    }
}