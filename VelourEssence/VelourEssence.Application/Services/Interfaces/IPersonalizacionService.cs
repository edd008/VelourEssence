using VelourEssence.Application.DTOs;

namespace VelourEssence.Application.Services.Interfaces
{
    public interface IPersonalizacionService
    {
        Task<List<CriterioPersonalizacionDto>> GetCriteriosPorProductoAsync(int idProducto);
        Task<CriterioPersonalizacionDto?> GetCriterioConOpcionesAsync(int idCriterio);
    }
}
