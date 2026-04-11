using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VelourEssence.Application.DTOs;

namespace VelourEssence.Application.Services.Interfaces
{
    // Interfaz para servicios de gestión de usuarios
    public interface IServiceUsuario
    {
        // Obtiene todos los usuarios disponibles
        Task<ICollection<UsuarioDto>> ListAsync();

        // Busca un usuario por ID, retorna null si no existe
        Task<UsuarioDto?> GetByIdAsync(int id);

        // Crea un nuevo usuario
        Task<UsuarioDto> CreateAsync(CrearUsuarioDto crearUsuarioDto);

        // Actualiza un usuario existente
        Task<UsuarioDto?> UpdateAsync(EditarUsuarioDto editarUsuarioDto);

        // Elimina un usuario por ID
        Task<bool> DeleteAsync(int id);

        // Obtiene todos los roles disponibles
        Task<ICollection<RolDto>> GetRolesAsync();

        // Verifica si un nombre de usuario ya existe (para validación)
        Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario, int? idUsuarioExcluir = null);

        // Verifica si un correo ya existe (para validación)
        Task<bool> ExisteCorreoAsync(string correo, int? idUsuarioExcluir = null);

        // Métodos para autenticación
        
        // Busca un usuario por correo electrónico o nombre de usuario
        Task<UsuarioDto?> GetByEmailOrUsernameAsync(string emailOUsuario);

    // Busca usuario por correo (para recuperación de contraseña)
    Task<UsuarioDto?> GetByEmailAsync(string email);

    // Guarda el token y expiración para recuperación de contraseña
    Task<bool> SetPasswordResetTokenAsync(int idUsuario, string token, DateTime expiration);

    // Limpia el token de recuperación (después de usar o expirar)
    Task<bool> ClearPasswordResetTokenAsync(int idUsuario);

        // Actualiza la fecha de último inicio de sesión
        Task UpdateLastLoginAsync(int idUsuario);
        
        // Autentica un usuario con credenciales
        Task<UsuarioDto?> AuthenticateAsync(string emailOUsuario, string contraseña);
        
        // Registra un nuevo usuario
        Task<UsuarioDto> RegisterAsync(RegisterDto registerDto);
    }
}