using VelourEssence.Infraestructure.Models;

namespace VelourEssence.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryOpcionPersonalizacion
    {
        Task<OpcionPersonalizacion?> GetAsync(int id);
        Task<List<OpcionPersonalizacion>> GetAllAsync();
        Task<OpcionPersonalizacion> CreateAsync(OpcionPersonalizacion entity);
        Task<OpcionPersonalizacion> UpdateAsync(OpcionPersonalizacion entity);
        Task<bool> DeleteAsync(int id);
        
        Task<List<OpcionPersonalizacion>> GetOpcionesPorCriterioAsync(int idCriterio);
        Task<List<OpcionPersonalizacion>> GetOpcionesActivasAsync();
        Task<OpcionPersonalizacion?> GetPorCriterioYNombreAsync(int idCriterio, string nombre);
    }
}
