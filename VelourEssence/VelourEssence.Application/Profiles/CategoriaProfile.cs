using AutoMapper;
using VelourEssence.Application.DTOs;
using VelourEssence.Infraestructure.Models;

namespace VelourEssence.Application.Profiles
{
    // Perfil de AutoMapper para mapear entre Categoria y CategoriaDto
    public class CategoriaProfile : Profile
    {
        public CategoriaProfile()
        {
            // Mapeo bidireccional entre Categoria y CategoriaDto
            CreateMap<Categoria, CategoriaDto>().ReverseMap();
        }
    }
}
