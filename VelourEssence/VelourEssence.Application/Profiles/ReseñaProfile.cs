using AutoMapper;
using VelourEssence.Application.DTOs;
using VelourEssence.Infraestructure.Models;

namespace VelourEssence.Application.Profiles
{
    // Profile de AutoMapper para mapear entidad Reseña a DTO
    public class ReseñaProfile : Profile
    {
        public ReseñaProfile()
        {
            // Mapeo de Reseña a ReseñaDto
            CreateMap<Reseña, ReseñaDto>()
                // Mapea el nombre del usuario desde la navegación
                .ForMember(dest => dest.NombreUsuario, opt => opt.MapFrom(src =>
                    src.IdUsuarioNavigation != null ? src.IdUsuarioNavigation.NombreUsuario : null))

                // Mapea el nombre del producto desde la navegación
                .ForMember(dest => dest.NombreProducto, opt => opt.MapFrom(src =>
                    src.IdProductoNavigation != null ? src.IdProductoNavigation.Nombre : null))

                // Mapea la valoración directamente
                .ForMember(dest => dest.Valoracion, opt => opt.MapFrom(src => src.Valoracion));

            // Mapeo de CrearReseñaDto a Reseña

            CreateMap<CrearReseñaDto, Reseña>()
           .ForMember(dest => dest.IdReseña, opt => opt.Ignore())
           .ForMember(dest => dest.IdUsuarioNavigation, opt => opt.Ignore())
           .ForMember(dest => dest.IdProductoNavigation, opt => opt.Ignore());


            /*CreateMap<CrearReseñaDto, Reseña>()
                .ForMember(dest => dest.IdReseña, opt => opt.Ignore()) // Se genera automáticamente
                .ForMember(dest => dest.IdUsuarioNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.IdProductoNavigation, opt => opt.Ignore());*/
        }
    }
}