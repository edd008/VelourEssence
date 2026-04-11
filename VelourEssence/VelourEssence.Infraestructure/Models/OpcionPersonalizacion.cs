using System.ComponentModel.DataAnnotations;

namespace VelourEssence.Infraestructure.Models
{
    /// <summary>
    /// Opción específica dentro de un criterio (ej: "Eau de Toilette", "50ml")
    /// </summary>
    public class OpcionPersonalizacion
    {
        [Key]
        public int IdOpcion { get; set; }

        [Required]
        public int IdCriterio { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = null!; // Nombre de la opción

        [Required]
        [StringLength(100)]
        public string Valor { get; set; } = null!; // Valor interno: "edt", "50ml"

        [StringLength(100)]
        public string? Etiqueta { get; set; } // Texto mostrado: "Eau de Toilette", "50ml"

        [Required]
        public decimal PrecioAdicional { get; set; } = 0; // Precio adicional en colones

        public int Orden { get; set; } = 0;

        [Required]
        public bool Activo { get; set; } = true;

        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Navegación
        public virtual CriterioPersonalizacion IdCriterioNavigation { get; set; } = null!;
        public virtual ICollection<ProductoPersonalizadoDetalle> ProductoPersonalizadoDetalles { get; set; } = new List<ProductoPersonalizadoDetalle>();
    }
}
