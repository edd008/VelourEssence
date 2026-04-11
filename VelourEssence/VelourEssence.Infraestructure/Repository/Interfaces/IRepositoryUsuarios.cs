using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VelourEssence.Infraestructure.Models;

namespace VelourEssence.Infraestructure.Repository.Interfaces
{
    // Interfaz para operaciones de repositorio de usuarios
    public interface IRepositoryUsuarios
    {
        // Obtiene todos los usuarios del sistema
        Task<ICollection<Usuario>> ListAsync();

        // Busca un usuario por ID, retorna null si no existe
        Task<Usuario?> GetByIdAsync(int id);

        // Crea un nuevo usuario
        Task<Usuario> CreateAsync(Usuario usuario);

        // Actualiza un usuario existente
        Task<Usuario?> UpdateAsync(Usuario usuario);

        // Elimina un usuario por ID
        Task<bool> DeleteAsync(int id);

        // Obtiene todos los roles disponibles
        Task<ICollection<Rol>> GetRolesAsync();

        // Verifica si un nombre de usuario ya existe
        Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario, int? idUsuarioExcluir = null);

        // Verifica si un correo ya existe
        Task<bool> ExisteCorreoAsync(string correo, int? idUsuarioExcluir = null);

        // Métodos para autenticación
        
        // Busca un usuario por correo electrónico o nombre de usuario
        Task<Usuario?> GetByEmailOrUsernameAsync(string emailOUsuario);

    // Busca usuario por correo (para recuperación de contraseña)
    Task<Usuario?> GetByEmailAsync(string email);

    // Guarda el token y expiración para recuperación de contraseña
    Task<bool> SetPasswordResetTokenAsync(int idUsuario, string token, DateTime expiration);

    // Limpia el token de recuperación (después de usar o expirar)
    Task<bool> ClearPasswordResetTokenAsync(int idUsuario);

        // Actualiza la fecha de último inicio de sesión
        Task UpdateLastLoginAsync(int idUsuario);
    }
}