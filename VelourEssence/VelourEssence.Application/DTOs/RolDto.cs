using System.ComponentModel.DataAnnotations;

namespace VelourEssence.Application.DTOs
{
    // DTO para mostrar información básica de un rol
    public record RolDto
    {
        public int IdRol { get; set; }

        [Required(ErrorMessage = "El nombre del rol es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre del rol no puede exceder los 50 caracteres")]
        public string NombreRol { get; set; } = string.Empty;
        
        // Alias para compatibilidad con AuthController
        public string Nombre => NombreRol;
    }
}
