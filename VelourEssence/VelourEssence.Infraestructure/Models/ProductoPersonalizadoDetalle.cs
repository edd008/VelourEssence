using System.ComponentModel.DataAnnotations;

namespace VelourEssence.Infraestructure.Models
{
    /// <summary>
    /// Detalle de las opciones seleccionadas para un producto personalizado
    /// </summary>
    public class ProductoPersonalizadoDetalle
    {
        [Key]
        public int IdDetalle { get; set; }

        [Required]
        public int IdProductoPersonalizado { get; set; }

        [Required]
        public int IdCriterio { get; set; }

        [Required]
        public int IdOpcion { get; set; }

        [StringLength(255)]
        public string? ValorPersonalizado { get; set; } // Para texto libre si aplica

        [Required]
        public decimal PrecioOpcion { get; set; } // Precio que tenía la opción al momento de la selección

        public DateTime FechaSeleccion { get; set; } = DateTime.Now;

        // Navegación
        public virtual ProductoPersonalizado IdProductoPersonalizadoNavigation { get; set; } = null!;
        public virtual CriterioPersonalizacion IdCriterioNavigation { get; set; } = null!;
        public virtual OpcionPersonalizacion IdOpcionNavigation { get; set; } = null!;
    }
}
