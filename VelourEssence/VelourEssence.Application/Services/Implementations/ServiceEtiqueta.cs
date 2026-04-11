       
using AutoMapper;
using VelourEssence.Application.DTOs;
using VelourEssence.Application.Services.Interfaces;
using VelourEssence.Infraestructure.Repository.Interfaces;

namespace VelourEssence.Application.Services.Implementations
{
    public class ServiceEtiqueta : IServiceEtiqueta
    {
        // Repositorio de etiquetas para acceder a los datos
    private readonly IRepositoryEtiqueta _repositoryEtiqueta;

        // AutoMapper para convertir entidades en DTOs
        private readonly IMapper _mapper;

        // Constructor que recibe las dependencias necesarias (repositorio y mapper)
        public ServiceEtiqueta(IRepositoryEtiqueta repositoryEtiqueta, IMapper mapper)
        {
            _repositoryEtiqueta = repositoryEtiqueta;
            _mapper = mapper;
        }

        // Lista de etiquetas desde el repositorio y convertirlas en DTOs
        public async Task<ICollection<EtiquetaDto>> ListAsync()
        {
            var etiquetas = await _repositoryEtiqueta.ListAsync();
            return _mapper.Map<ICollection<EtiquetaDto>>(etiquetas);
        }

        // Etiqueta por su ID, convertirla en DTO si existe
        public async Task<EtiquetaDto?> GetByIdAsync(int id)
        {
            var etiqueta = await _repositoryEtiqueta.GetByIdAsync(id);
            return etiqueta != null ? _mapper.Map<EtiquetaDto>(etiqueta) : null;
        }

        // Crear una nueva etiqueta
        public async Task<EtiquetaDto> CreateAsync(EtiquetaDto dto)
        {
            var entity = _mapper.Map<VelourEssence.Infraestructure.Models.Etiqueta>(dto);
            var result = await _repositoryEtiqueta.AddAsync(entity);
            return _mapper.Map<EtiquetaDto>(result);
        }

        // Actualizar una etiqueta existente
        public async Task<bool> UpdateAsync(EtiquetaDto dto)
        {
            var entity = _mapper.Map<VelourEssence.Infraestructure.Models.Etiqueta>(dto);
            return await _repositoryEtiqueta.UpdateAsync(entity);
        }
        // Elimina una etiqueta por ID
        public async Task<bool> DeleteAsync(int id)
        {
            return await _repositoryEtiqueta.DeleteAsync(id);
        }
    }
     
}
