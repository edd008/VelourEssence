using System.ComponentModel.DataAnnotations;

namespace VelourEssence.Application.DTOs
{
    /// <summary>
    /// DTO para el proceso de registro de nuevos usuarios
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// Nombre de usuario único
        /// </summary>
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [StringLength(50, ErrorMessage = "El nombre de usuario no puede exceder 50 caracteres")]
        [Display(Name = "Nombre de Usuario")]
        public string NombreUsuario { get; set; } = string.Empty;

        /// <summary>
        /// Correo electrónico único
        /// </summary>
        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder 100 caracteres")]
        [Display(Name = "Correo Electrónico")]
        public string Correo { get; set; } = string.Empty;

        /// <summary>
        /// Contraseña del usuario
        /// </summary>
        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Contraseña { get; set; } = string.Empty;

        /// <summary>
        /// Confirmación de la contraseña
        /// </summary>
        [Required(ErrorMessage = "La confirmación de contraseña es requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("Contraseña", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarContraseña { get; set; } = string.Empty;

        /// <summary>
        /// Aceptación de términos y condiciones
        /// </summary>
        [Required(ErrorMessage = "Debes aceptar los términos y condiciones")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Debes aceptar los términos y condiciones")]
        [Display(Name = "Acepto los términos y condiciones")]
        public bool AceptarTerminos { get; set; } = false;

        /// <summary>
        /// Lista de roles disponibles (usado en la vista)
        /// </summary>
        public List<RolDto> RolesDisponibles { get; set; } = new List<RolDto>();
    }
}
