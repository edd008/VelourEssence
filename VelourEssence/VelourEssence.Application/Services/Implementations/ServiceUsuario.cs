using AutoMapper; 
using VelourEssence.Application.DTOs; 
using VelourEssence.Application.Services.Interfaces; 
using VelourEssence.Infraestructure.Repository.Interfaces;
using VelourEssence.Infraestructure.Models;
using BCrypt.Net;

namespace VelourEssence.Application.Services.Implementations
{
    public class ServiceUsuario : IServiceUsuario
    {
        // Repositorio que accede a los datos de usuarios en la base
        private readonly IRepositoryUsuarios _repositoryUsuarios;
        // Mapper para convertir entre entidades y DTOs
        private readonly IMapper _mapper;

        // Constructor que inyecta el repositorio y el mapper
        public ServiceUsuario(IRepositoryUsuarios repositoryUsuarios, IMapper mapper)
        {
            _repositoryUsuarios = repositoryUsuarios;
            _mapper = mapper;
        }

        //listar todos los usuarios en formato DTO
        public async Task<ICollection<UsuarioDto>> ListAsync()
        {
            var usuarios = await _repositoryUsuarios.ListAsync();
            return _mapper.Map<ICollection<UsuarioDto>>(usuarios);
        }

        // Usuario por su id
        public async Task<UsuarioDto?> GetByIdAsync(int id)
        {
            var usuario = await _repositoryUsuarios.GetByIdAsync(id);
            return usuario != null ? _mapper.Map<UsuarioDto>(usuario) : null;
        }

        // Crea un nuevo usuario
        public async Task<UsuarioDto> CreateAsync(CrearUsuarioDto crearUsuarioDto)
        {
            // Validar que el nombre de usuario no exista
            if (await _repositoryUsuarios.ExisteNombreUsuarioAsync(crearUsuarioDto.NombreUsuario))
            {
                throw new InvalidOperationException("El nombre de usuario ya existe");
            }

            // Validar que el correo no exista
            if (await _repositoryUsuarios.ExisteCorreoAsync(crearUsuarioDto.Correo))
            {
                throw new InvalidOperationException("El correo electrónico ya está registrado");
            }

            // Mapear DTO a entidad
            var usuario = _mapper.Map<Usuario>(crearUsuarioDto);
            
            // Encriptar la contraseña
            usuario.Contraseña = BCrypt.Net.BCrypt.HashPassword(crearUsuarioDto.Contraseña);

            // Crear usuario
            var usuarioCreado = await _repositoryUsuarios.CreateAsync(usuario);
            
            return _mapper.Map<UsuarioDto>(usuarioCreado);
        }

        // Actualiza un usuario existente
        public async Task<UsuarioDto?> UpdateAsync(EditarUsuarioDto editarUsuarioDto)
        {
            // Validar que el usuario existe
            var usuarioExistente = await _repositoryUsuarios.GetByIdAsync(editarUsuarioDto.IdUsuario);
            if (usuarioExistente == null)
            {
                return null;
            }

            // Validar que el nombre de usuario no exista para otro usuario
            if (await _repositoryUsuarios.ExisteNombreUsuarioAsync(editarUsuarioDto.NombreUsuario, editarUsuarioDto.IdUsuario))
            {
                throw new InvalidOperationException("El nombre de usuario ya existe");
            }

            // Validar que el correo no exista para otro usuario
            if (await _repositoryUsuarios.ExisteCorreoAsync(editarUsuarioDto.Correo, editarUsuarioDto.IdUsuario))
            {
                throw new InvalidOperationException("El correo electrónico ya está registrado");
            }

            // Mapear DTO a entidad
            var usuario = _mapper.Map<Usuario>(editarUsuarioDto);
            
            // Encriptar la contraseña solo si se proporciona una nueva
            if (!string.IsNullOrWhiteSpace(editarUsuarioDto.Contraseña))
            {
                usuario.Contraseña = BCrypt.Net.BCrypt.HashPassword(editarUsuarioDto.Contraseña);
            }

            // Actualizar usuario
            var usuarioActualizado = await _repositoryUsuarios.UpdateAsync(usuario);
            
            return usuarioActualizado != null ? _mapper.Map<UsuarioDto>(usuarioActualizado) : null;
        }

        // Elimina un usuario por ID
        public async Task<bool> DeleteAsync(int id)
        {
            return await _repositoryUsuarios.DeleteAsync(id);
        }

        // Obtiene todos los roles disponibles
        public async Task<ICollection<RolDto>> GetRolesAsync()
        {
            var roles = await _repositoryUsuarios.GetRolesAsync();
            return _mapper.Map<ICollection<RolDto>>(roles);
        }

        // Verifica si un nombre de usuario ya existe
        public async Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario, int? idUsuarioExcluir = null)
        {
            return await _repositoryUsuarios.ExisteNombreUsuarioAsync(nombreUsuario, idUsuarioExcluir);
        }

        // Verifica si un correo ya existe
        public async Task<bool> ExisteCorreoAsync(string correo, int? idUsuarioExcluir = null)
        {
            return await _repositoryUsuarios.ExisteCorreoAsync(correo, idUsuarioExcluir);
        }

        // Busca un usuario por correo electrónico o nombre de usuario
        public async Task<UsuarioDto?> GetByEmailOrUsernameAsync(string emailOUsuario)
        {
            var usuario = await _repositoryUsuarios.GetByEmailOrUsernameAsync(emailOUsuario);
            return usuario != null ? _mapper.Map<UsuarioDto>(usuario) : null;
        }

        // Busca usuario por correo (para recuperación de contraseña)
        public async Task<UsuarioDto?> GetByEmailAsync(string email)
        {
            var usuario = await _repositoryUsuarios.GetByEmailAsync(email);
            return usuario != null ? _mapper.Map<UsuarioDto>(usuario) : null;
        }

        // Guarda el token y expiración para recuperación de contraseña
        public async Task<bool> SetPasswordResetTokenAsync(int idUsuario, string token, DateTime expiration)
        {
            return await _repositoryUsuarios.SetPasswordResetTokenAsync(idUsuario, token, expiration);
        }

        // Limpia el token de recuperación (después de usar o expirar)
        public async Task<bool> ClearPasswordResetTokenAsync(int idUsuario)
        {
            return await _repositoryUsuarios.ClearPasswordResetTokenAsync(idUsuario);
        }

        // Actualiza la fecha de último inicio de sesión
        public async Task UpdateLastLoginAsync(int idUsuario)
        {
            await _repositoryUsuarios.UpdateLastLoginAsync(idUsuario);
        }

        // Autentica un usuario con credenciales
        public async Task<UsuarioDto?> AuthenticateAsync(string emailOUsuario, string contraseña)
        {
            var usuario = await _repositoryUsuarios.GetByEmailOrUsernameAsync(emailOUsuario);
            if (usuario == null)
                return null;

            // Verificar contraseña
            if (!BCrypt.Net.BCrypt.Verify(contraseña, usuario.Contraseña))
                return null;

            // Mapear a DTO con información del rol
            var usuarioDto = _mapper.Map<UsuarioDto>(usuario);
            
            // Actualizar fecha de último inicio de sesión
            await UpdateLastLoginAsync(usuario.IdUsuario);
            
            return usuarioDto;
        }

        // Registra un nuevo usuario
        public async Task<UsuarioDto> RegisterAsync(RegisterDto registerDto)
        {
            // Validar que el nombre de usuario no exista
            if (await _repositoryUsuarios.ExisteNombreUsuarioAsync(registerDto.NombreUsuario))
            {
                throw new InvalidOperationException("El nombre de usuario ya existe");
            }

            // Validar que el correo no exista
            if (await _repositoryUsuarios.ExisteCorreoAsync(registerDto.Correo))
            {
                throw new InvalidOperationException("El correo electrónico ya está registrado");
            }

            // Obtener rol de cliente por defecto
            var roles = await _repositoryUsuarios.GetRolesAsync();
            var rolCliente = roles.FirstOrDefault(r => r.NombreRol.ToLower() == "cliente");
            if (rolCliente == null)
            {
                throw new InvalidOperationException("No se encontró el rol de cliente");
            }

            // Crear entidad usuario
            var usuario = new Usuario
            {
                NombreUsuario = registerDto.NombreUsuario,
                Correo = registerDto.Correo,
                Contraseña = BCrypt.Net.BCrypt.HashPassword(registerDto.Contraseña),
                IdRol = rolCliente.IdRol,
                UltimoInicioSesion = DateTime.Now
            };

            var usuarioCreado = await _repositoryUsuarios.CreateAsync(usuario);
            return _mapper.Map<UsuarioDto>(usuarioCreado);
        }
    }
}
