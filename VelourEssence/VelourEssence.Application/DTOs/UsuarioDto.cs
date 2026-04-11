using System;
using System.ComponentModel.DataAnnotations;

namespace VelourEssence.Application.DTOs
{
    // DTO para mostrar la información de un usuario con validaciones
    public record UsuarioDto
    {
        // Identificador único del usuario
        public int IdUsuario { get; set; }

        // Nombre de usuario 
        public string NombreUsuario { get; set; } = string.Empty;

        // Correo electrónico 
        public string Correo { get; set; } = string.Empty;

        // Contraseña 
        public string? Contraseña { get; set; }

        // Fecha y hora del último inicio de sesión
        public DateTime? UltimoInicioSesion { get; set; }

        // Id del rol del usuario
        public int? IdRol { get; set; }

        // Nombre del rol (opcional, para mostrar el nombre del rol relacionado)
        public string? NombreRol { get; set; }
        
    // Rol del usuario (para claims de autenticación)
    public string Rol { get; set; } = string.Empty;

    // Token de recuperación de contraseña
    public string? PasswordResetToken { get; set; }

    // Expiración del token de recuperación
    public DateTime? PasswordResetTokenExpiration { get; set; }
    }
}
