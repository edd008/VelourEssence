using AutoMapper;
using VelourEssence.Application.DTOs;
using VelourEssence.Application.Services.Interfaces;
using VelourEssence.Infraestructure.Repository.Interfaces;

namespace VelourEssence.Application.Services.Implementations
{
    public class PersonalizacionService : IPersonalizacionService
    {
        private readonly IRepositoryCriterioPersonalizacion _repositoryCriterio;
        private readonly IMapper _mapper;

        public PersonalizacionService(
            IRepositoryCriterioPersonalizacion repositoryCriterio,
            IMapper mapper)
        {
            _repositoryCriterio = repositoryCriterio;
            _mapper = mapper;
        }

        public async Task<List<CriterioPersonalizacionDto>> GetCriteriosPorProductoAsync(int idProducto)
        {
            var criterios = await _repositoryCriterio.GetCriteriosPorProductoAsync(idProducto);
            return _mapper.Map<List<CriterioPersonalizacionDto>>(criterios);
        }

        public async Task<CriterioPersonalizacionDto?> GetCriterioConOpcionesAsync(int idCriterio)
        {
            var criterio = await _repositoryCriterio.GetConOpcionesAsync(idCriterio);
            return criterio != null ? _mapper.Map<CriterioPersonalizacionDto>(criterio) : null;
        }
    }
}
