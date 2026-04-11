using VelourEssence.Infraestructure.Models;

namespace VelourEssence.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryProductoPersonalizado
    {
        Task<ProductoPersonalizado?> GetAsync(int id);
        Task<List<ProductoPersonalizado>> GetAllAsync();
        Task<ProductoPersonalizado> CreateAsync(ProductoPersonalizado entity);
        Task<ProductoPersonalizado> UpdateAsync(ProductoPersonalizado entity);
        Task<bool> DeleteAsync(int id);
        
        Task<List<ProductoPersonalizado>> GetPorUsuarioAsync(int idUsuario);
        Task<ProductoPersonalizado?> GetConDetallesAsync(int id);
        Task<List<ProductoPersonalizado>> GetActivosAsync();
        Task CrearDetallesAsync(List<ProductoPersonalizadoDetalle> detalles);
        Task<List<ProductoPersonalizadoDetalle>> GetDetallesPorProductoAsync(int idProductoPersonalizado);
    }
}
