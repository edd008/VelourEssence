using AutoMapper;
using VelourEssence.Application.DTOs;
using VelourEssence.Infraestructure.Models;

namespace VelourEssence.Application.Profiles
{
    // Profile de AutoMapper para mapear entidad Usuario a DTO
    public class UsuarioProfile : Profile
    {
        public UsuarioProfile()
        {
            // Mapeo de Usuario a UsuarioDto
            CreateMap<Usuario, UsuarioDto>()
                // Mapea el nombre del rol desde la navegación del rol
                .ForMember(dest => dest.NombreRol, opt => opt.MapFrom(src =>
                    src.IdRolNavigation != null ? src.IdRolNavigation.NombreRol : null))
                // Mapea el rol para claims de autenticación
                .ForMember(dest => dest.Rol, opt => opt.MapFrom(src =>
                    src.IdRolNavigation != null ? src.IdRolNavigation.NombreRol : string.Empty));

            // Mapeo de CrearUsuarioDto a Usuario
            CreateMap<CrearUsuarioDto, Usuario>()
                .ForMember(dest => dest.IdUsuario, opt => opt.Ignore()) // Se genera automáticamente
                .ForMember(dest => dest.UltimoInicioSesion, opt => opt.Ignore()) // Se establece en null inicialmente
                .ForMember(dest => dest.IdRolNavigation, opt => opt.Ignore()) // Se maneja por separado
                .ForMember(dest => dest.Carrito, opt => opt.Ignore())
                .ForMember(dest => dest.Pedido, opt => opt.Ignore())
                .ForMember(dest => dest.ProductoPersonalizado, opt => opt.Ignore())
                .ForMember(dest => dest.Reseña, opt => opt.Ignore());

            // Mapeo de EditarUsuarioDto a Usuario
            CreateMap<EditarUsuarioDto, Usuario>()
                .ForMember(dest => dest.IdRolNavigation, opt => opt.Ignore()) // Se maneja por separado
                .ForMember(dest => dest.Carrito, opt => opt.Ignore())
                .ForMember(dest => dest.Pedido, opt => opt.Ignore())
                .ForMember(dest => dest.ProductoPersonalizado, opt => opt.Ignore())
                .ForMember(dest => dest.Reseña, opt => opt.Ignore());

            // Mapeo de Usuario a EditarUsuarioDto
            CreateMap<Usuario, EditarUsuarioDto>()
                .ForMember(dest => dest.Contraseña, opt => opt.Ignore()) // No mostrar contraseña
                .ForMember(dest => dest.ConfirmarContraseña, opt => opt.Ignore());

            // Mapeo de RegisterDto a Usuario
            CreateMap<RegisterDto, Usuario>()
                .ForMember(dest => dest.IdUsuario, opt => opt.Ignore()) // Se genera automáticamente
                .ForMember(dest => dest.UltimoInicioSesion, opt => opt.Ignore()) // Se establece en el servicio
                .ForMember(dest => dest.IdRolNavigation, opt => opt.Ignore()) // Se maneja por separado
                .ForMember(dest => dest.IdRol, opt => opt.Ignore()) // Se establece en el servicio
                .ForMember(dest => dest.Carrito, opt => opt.Ignore())
                .ForMember(dest => dest.Pedido, opt => opt.Ignore())
                .ForMember(dest => dest.ProductoPersonalizado, opt => opt.Ignore())
                .ForMember(dest => dest.Reseña, opt => opt.Ignore());

            // Mapeo de Rol a RolDto
            CreateMap<Rol, RolDto>();
        }
    }
}