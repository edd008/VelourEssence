using AutoMapper;
using VelourEssence.Application.DTOs;
using VelourEssence.Application.Services.Interfaces;
using VelourEssence.Infraestructure.Repository.Interfaces;
using VelourEssence.Infraestructure.Models;

namespace VelourEssence.Application.Services.Implementations
{
    public class ServicePromocion : IServicePromocion
    {
        // Repositorio de Promoción para acceder a los datos
        private readonly IRepositoryPromocion _repositoryPromocion;
        // Repositorio de Producto para obtener productos
        private readonly IRepositoryProductos _repositoryProducto;
        // AutoMapper para convertir Promociones en DTOs
        private readonly IMapper _mapper;

        // Constructor que recibe las dependencias necesarias
        public ServicePromocion(IRepositoryPromocion repositoryPromocion, IRepositoryProductos repositoryProducto, IMapper mapper)
        {
            _repositoryPromocion = repositoryPromocion;
            _repositoryProducto = repositoryProducto;
            _mapper = mapper;
        }

        // Lista de las promociones del repositorio
        public async Task<ICollection<PromocionDto>> ListAsync()
        {
            var promociones = await _repositoryPromocion.ListAsync();
            return _mapper.Map<ICollection<PromocionDto>>(promociones);
        }

        // Promoción por su ID
        public async Task<PromocionDto?> GetByIdAsync(int id)
        {
            var promocion = await _repositoryPromocion.GetByIdAsync(id);
            return promocion != null ? _mapper.Map<PromocionDto>(promocion) : null;
        }

        // Obtiene una promoción para editar
        public async Task<CrearPromocionDto?> GetForEditAsync(int id)
        {
            var promocion = await _repositoryPromocion.GetByIdAsync(id);
            return promocion != null ? _mapper.Map<CrearPromocionDto>(promocion) : null;
        }

        //new
        public async Task<ICollection<PromocionDto>> ListVigentesAsync()
        {
            var promociones = await _repositoryPromocion.ListAsync();
            var dtoList = _mapper.Map<ICollection<PromocionDto>>(promociones);

            // Solo promociones vigentes
            return dtoList.Where(p => p.Estado == "Vigente").ToList();
        }

        // Crea una nueva promoción
        public async Task<bool> CreateAsync(CrearPromocionDto promocionDto)
        {
            var promocion = _mapper.Map<Promocion>(promocionDto);
            return await _repositoryPromocion.CreateAsync(promocion);
        }

        // Actualiza una promoción existente
        public async Task<bool> UpdateAsync(CrearPromocionDto promocionDto)
        {
            var promocion = _mapper.Map<Promocion>(promocionDto);
            return await _repositoryPromocion.UpdateAsync(promocion);
        }

        // Elimina una promoción
        public async Task<bool> DeleteAsync(int id)
        {
            return await _repositoryPromocion.DeleteAsync(id);
        }

        // Obtiene todas las categorías para dropdowns
        public async Task<ICollection<CategoriaDto>> GetCategoriasAsync()
        {
            var categorias = await _repositoryPromocion.GetCategoriasAsync();
            return _mapper.Map<ICollection<CategoriaDto>>(categorias);
        }

        // Obtiene todos los productos para dropdowns
        public async Task<ICollection<ProductoDto>> GetProductosAsync()
        {
            var productos = await _repositoryProducto.ListAsync();
            return _mapper.Map<ICollection<ProductoDto>>(productos);
        }
    }
}
