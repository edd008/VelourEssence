using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VelourEssence.Application.DTOs
{
    // DTO para mostrar información básica de productos
    public record ProductoDto
    {
        // ID único del producto
        public int IdProducto { get; set; }

        // Nombre del producto
        public string? Nombre { get; set; }

        // Descripción del producto
        public string? Descripcion { get; set; }

        // Precio original del producto
        public decimal? Precio { get; set; }

        // Marca del producto
        public string? Marca { get; set; }

        // Concentración del producto
        public string? Concentracion { get; set; }

        // Género del producto
        public string? Genero { get; set; }

        // Precio con descuento aplicado
        public decimal? PrecioConDescuento { get; set; }

        // Indica si el producto tiene promoción activa
        public bool TienePromocion { get; set; }

        // Lista de imágenes del producto
        public List<ImagenProductoDto> Imagenes { get; set; } = new();

        // Calcula automáticamente el porcentaje de descuento
        public decimal? PorcentajeDescuentoAplicado =>
            (TienePromocion && Precio.HasValue && PrecioConDescuento.HasValue && Precio > 0)
                ? (1 - (PrecioConDescuento.Value / Precio.Value)) * 100
                : null;
    }
}