using AutoMapper;
using VelourEssence.Application.DTOs;
using VelourEssence.Infraestructure.Models;

// Perfil de AutoMapper para mapear entre la entidad ImagenProducto y su DTO ImagenProductoDto

public class ImagenProductoProfile : Profile
{
    public ImagenProductoProfile()
    {
        CreateMap<ImagenProducto, ImagenProductoDto>()
            .ForMember(dest => dest.IdImagenProducto, opt => opt.MapFrom(src => src.IdImagen))
            .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.UrlImagen));
    }
}
