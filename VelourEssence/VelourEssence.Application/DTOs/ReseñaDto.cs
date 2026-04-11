using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VelourEssence.Application.DTOs
{
    // DTO para mostrar una reseña de un producto realizada por un usuario
    public record ReseñaDto
    {
        public int IdReseña { get; set; }

        public int IdUsuario { get; set; } // ← necesario
        public int IdProducto { get; set; } // ← necesario

        public string? NombreUsuario { get; set; }
        public string? NombreProducto { get; set; }

        public DateTime? Fecha { get; set; }
        public string? Comentario { get; set; }
        public int? Valoracion { get; set; }
    }

}
