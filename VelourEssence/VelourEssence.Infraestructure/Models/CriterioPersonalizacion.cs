using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VelourEssence.Infraestructure.Models
{
    /// <summary>
    /// Criterio de personalización para productos (ej: Concentración, Tamaño)
    /// </summary>
    public class CriterioPersonalizacion
    {
        [Key]
        public int IdCriterio { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = null!; // "Concentración", "Tamaño"

        [StringLength(255)]
        public string? Descripcion { get; set; }

        [StringLength(50)]
        public string TipoDato { get; set; } = "Lista"; // "Lista", "Texto", "Numero"

        [Required]
        public bool Obligatorio { get; set; } = true;

        [Required]
        public bool Activo { get; set; } = true;

        public int Orden { get; set; } = 0;

        // Navegación
        [ForeignKey("ProductoId")]
        public virtual Producto Producto { get; set; } = null!;
        
        public virtual ICollection<OpcionPersonalizacion> Opciones { get; set; } = new List<OpcionPersonalizacion>();
    }
}
