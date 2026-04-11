using System.ComponentModel.DataAnnotations;

namespace VelourEssence.Web.Models
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string NuevaContraseña { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("NuevaContraseña", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarContraseña { get; set; } = string.Empty;
    }
}
