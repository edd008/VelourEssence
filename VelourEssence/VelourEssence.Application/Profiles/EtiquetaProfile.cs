using AutoMapper;
using VelourEssence.Application.DTOs;
using VelourEssence.Infraestructure.Models;

namespace VelourEssence.Application.Profiles
{
    // Perfil de AutoMapper para mapear entre la entidad Etiqueta y su DTO
    public class EtiquetaProfile : Profile
    {
        public EtiquetaProfile()
        {
            // Configura el mapeo entre Etiqueta y EtiquetaDto
            CreateMap<Etiqueta, EtiquetaDto>().ReverseMap();
        }
    }
}
