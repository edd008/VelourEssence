using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using VelourEssence.Application.DTOs;
using VelourEssence.Application.Services.Interfaces;
using VelourEssence.Infraestructure.Repository.Interfaces;
using VelourEssence.Infraestructure.Models;

namespace VelourEssence.Application.Services.Implementations
{
    public class ServiceReseña : IServiceReseña
    {
        // Repositorio de reseñas y AutoMapper para transformar datos
        private readonly IRepositoryReseña _repositoryReseña;
        private readonly IMapper _mapper;

        // Constructor que inyecta el repositorio y el mapper
        public ServiceReseña(IRepositoryReseña repositoryReseña, IMapper mapper)
        {
            _repositoryReseña = repositoryReseña;
            _mapper = mapper;
        }

        // Lista de todas las reseñas desde el repositorio
        public async Task<ICollection<ReseñaDto>> ListAsync()
        {
            var reseñas = await _repositoryReseña.ListAsync();
            return _mapper.Map<ICollection<ReseñaDto>>(reseñas);
        }

        // Reseña por su ID
        public async Task<ReseñaDto?> GetByIdAsync(int id)
        {
            var reseña = await _repositoryReseña.GetByIdAsync(id);
            return reseña != null ? _mapper.Map<ReseñaDto>(reseña) : null;
        }

        // Crea una nueva reseña
        public async Task<ReseñaDto> CreateAsync(CrearReseñaDto crearReseñaDto)
        {
            var reseña = _mapper.Map<Reseña>(crearReseñaDto);
            var reseñaCreada = await _repositoryReseña.CreateAsync(reseña);

            // 🛠️ Aquí recuperas la reseña con navegación incluida
            var reseñaConUsuario = await _repositoryReseña.GetByIdWithIncludesAsync(reseñaCreada.IdReseña);

            return _mapper.Map<ReseñaDto>(reseñaConUsuario!);
        }

        // Obtiene reseñas por ID de producto
        public async Task<ICollection<ReseñaDto>> GetByProductIdAsync(int productId)
        {
            var reseñas = await _repositoryReseña.GetByProductIdAsync(productId);
            return _mapper.Map<ICollection<ReseñaDto>>(reseñas);
        }

        // Calcula el promedio de valoraciones de un producto
        public async Task<double> GetAverageRatingByProductIdAsync(int productId)
        {
            return await _repositoryReseña.GetAverageRatingByProductIdAsync(productId);
        }
    }
}
