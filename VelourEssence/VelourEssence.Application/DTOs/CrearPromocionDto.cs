using System.ComponentModel.DataAnnotations;

namespace VelourEssence.Application.DTOs
{
    // DTO para crear y editar promociones
    public record CrearPromocionDto
    {
        public int IdPromocion { get; set; }

        [Required(ErrorMessage = "El nombre de la promoción es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El tipo de promoción es obligatorio")]
        public string Tipo { get; set; } = null!; // "Producto" o "Categoria"

        public int? IdProducto { get; set; }

        public int? IdCategoria { get; set; }

        [Required(ErrorMessage = "El porcentaje de descuento es obligatorio")]
        [Range(0.01, 100, ErrorMessage = "El porcentaje debe estar entre 0.01% y 100%")]
        public decimal PorcentajeDescuento { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        public DateOnly FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        public DateOnly FechaFin { get; set; }

        // Propiedades para mostrar nombres en vez de IDs
        public string? NombreProducto { get; set; }
        public string? NombreCategoria { get; set; }
    }
}
