using System.ComponentModel.DataAnnotations;

namespace VelourEssence.Application.DTOs
{
    /// <summary>
    /// DTO para el proceso de login
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Correo electrónico o nombre de usuario
        /// </summary>
        [Required(ErrorMessage = "El correo o nombre de usuario es requerido")]
        [Display(Name = "Correo o Usuario")]
        public string EmailOUsuario { get; set; } = string.Empty;

        /// <summary>
        /// Contraseña del usuario
        /// </summary>
        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Contraseña { get; set; } = string.Empty;

        /// <summary>
        /// Opción para recordar al usuario (mantener sesión activa)
        /// </summary>
        [Display(Name = "Recordarme")]
        public bool RecordarMe { get; set; } = false;
    }
}
