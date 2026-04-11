using AutoMapper;
using VelourEssence.Application.DTOs;
using VelourEssence.Infraestructure.Models;

namespace VelourEssence.Application.Profiles
{
    public class PersonalizacionProfile : Profile
    {
        public PersonalizacionProfile()
        {
            CreateMap<CriterioPersonalizacion, CriterioPersonalizacionDto>();
            CreateMap<OpcionPersonalizacion, OpcionPersonalizacionDto>();
        }
    }
}
