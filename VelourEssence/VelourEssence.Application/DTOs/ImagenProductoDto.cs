using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VelourEssence.Application.DTOs
{
    // DTO para transferencia de datos de imágenes de productos
    public record ImagenProductoDto
    {
        // ID único de la imagen
        public int IdImagenProducto { get; set; }

        // Ruta o URL de la imagen
        public string? Url { get; set; }

        // ID del producto asociado
        public int IdProducto { get; set; }

        // Indica si es la imagen principal del producto
        public bool EsPrincipal { get; set; }

        // Texto alternativo para la imagen
        public string? AltText { get; set; }

        // Ruta de la imagen (alias de Url para compatibilidad)
        public string? Ruta => Url;
    }
}