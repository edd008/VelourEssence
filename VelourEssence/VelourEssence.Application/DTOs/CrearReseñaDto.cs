using System.ComponentModel.DataAnnotations;

namespace VelourEssence.Application.DTOs
{
    /// <summary>
    /// DTO para crear una nueva reseña de producto
    /// </summary>
    public record CrearReseñaDto
    {
        /// <summary>
        /// ID del producto al cual se está reseñando
        /// </summary>
        [Required(ErrorMessage = "El producto es requerido")]
        public int IdProducto { get; set; }

        /// <summary>
        /// ID del usuario que está creando la reseña
        /// </summary>
        [Required(ErrorMessage = "El usuario es requerido")]
        public int IdUsuario { get; set; }

        /// <summary>
        /// Comentario textual de la reseña
        /// </summary>
        [Required(ErrorMessage = "El comentario es requerido")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "El comentario debe tener entre 10 y 1000 caracteres")]
        public string Comentario { get; set; } = string.Empty;

        /// <summary>
        /// Valoración numérica del 1 al 5
        /// </summary>
        [Required(ErrorMessage = "La valoración es requerida")]
        [Range(1, 5, ErrorMessage = "La valoración debe estar entre 1 y 5")]
        public int Valoracion { get; set; }

        /// <summary>
        /// Fecha de creación (se establece automáticamente)
        /// </summary>
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
