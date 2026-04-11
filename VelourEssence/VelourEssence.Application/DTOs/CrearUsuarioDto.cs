using System.ComponentModel.DataAnnotations;

namespace VelourEssence.Application.DTOs
{
    // DTO para crear un nuevo usuario con validaciones
    public record CrearUsuarioDto
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre de usuario no puede exceder los 50 caracteres")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        [StringLength(100, ErrorMessage = "El correo electrónico no puede exceder los 100 caracteres")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres")]
        public string Contraseña { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe confirmar la contraseña")]
        [Compare("Contraseña", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarContraseña { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe seleccionar un rol")]
        public int IdRol { get; set; }
    }
}
