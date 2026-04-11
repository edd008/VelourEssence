using VelourEssence.Infraestructure.Models;

namespace VelourEssence.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryCriterioPersonalizacion
    {
        Task<CriterioPersonalizacion?> GetAsync(int id);
        Task<List<CriterioPersonalizacion>> GetAllAsync();
        Task<CriterioPersonalizacion> CreateAsync(CriterioPersonalizacion entity);
        Task<CriterioPersonalizacion> UpdateAsync(CriterioPersonalizacion entity);
        Task<bool> DeleteAsync(int id);
        
        Task<List<CriterioPersonalizacion>> GetCriteriosActivosAsync();
        Task<List<CriterioPersonalizacion>> GetCriteriosPorProductoAsync(int idProducto);
        Task<CriterioPersonalizacion?> GetConOpcionesAsync(int id);
    }
}
