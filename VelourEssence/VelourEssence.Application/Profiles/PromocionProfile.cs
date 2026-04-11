using AutoMapper;
using VelourEssence.Application.DTOs;
using VelourEssence.Infraestructure.Models;

namespace VelourEssence.Application.Profiles
{
    // Profile de AutoMapper para mapear entidad Promocion a DTO
    public class PromocionProfile : Profile
    {
        public PromocionProfile()
        {
            // Mapeo de Promocion a PromocionDto
            CreateMap<Promocion, PromocionDto>()
                // Mapea el nombre del producto desde la navegación
                .ForMember(dest => dest.NombreProducto, opt => opt.MapFrom(src =>
                    src.IdProductoNavigation != null ? src.IdProductoNavigation.Nombre : null))
                // Mapea el nombre de la categoría desde la navegación
                .ForMember(dest => dest.NombreCategoria, opt => opt.MapFrom(src =>
                    src.IdCategoriaNavigation != null ? src.IdCategoriaNavigation.Nombre : null))
                // Mapea el ID del producto
                .ForMember(dest => dest.IdProducto, opt => opt.MapFrom(src => src.IdProducto ?? 0))
                .ForMember(dest => dest.ProductoId, opt => opt.MapFrom(src => src.IdProducto ?? 0));

            // Mapeo de PromocionDto a Promocion (para crear/actualizar)
            CreateMap<PromocionDto, Promocion>()
                .ForMember(dest => dest.IdProducto, opt => opt.MapFrom(src => 
                    src.IdProducto > 0 ? src.IdProducto : (int?)null))
                .ForMember(dest => dest.IdCategoria, opt => opt.MapFrom(src => 
                    src.IdCategoria > 0 ? src.IdCategoria : (int?)null))
                // Ignorar propiedades de navegación
                .ForMember(dest => dest.IdProductoNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.IdCategoriaNavigation, opt => opt.Ignore());

            // Mapeo específico para crear/editar promociones
            CreateMap<CrearPromocionDto, Promocion>()
                .ForMember(dest => dest.IdProducto, opt => opt.MapFrom(src => 
                    src.Tipo == "Producto" ? src.IdProducto : (int?)null))
                .ForMember(dest => dest.IdCategoria, opt => opt.MapFrom(src => 
                    src.Tipo == "Categoria" ? src.IdCategoria : (int?)null))
                // Ignorar propiedades de navegación
                .ForMember(dest => dest.IdProductoNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.IdCategoriaNavigation, opt => opt.Ignore());

            // Mapeo de Promocion a CrearPromocionDto (para editar)
            CreateMap<Promocion, CrearPromocionDto>()
                .ForMember(dest => dest.NombreProducto, opt => opt.MapFrom(src =>
                    src.IdProductoNavigation != null ? src.IdProductoNavigation.Nombre : null))
                .ForMember(dest => dest.NombreCategoria, opt => opt.MapFrom(src =>
                    src.IdCategoriaNavigation != null ? src.IdCategoriaNavigation.Nombre : null));
        }
    }
}