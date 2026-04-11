namespace VelourEssence.Application.DTOs
{
    // DTO para mostrar información completa de un producto
    public record ProductoDetalleDto
    {
        public int IdProducto { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal? Precio { get; set; }
        public string? Categoria { get; set; }
        public List<string> Etiquetas { get; set; } = new();
        public List<ImagenProductoDto> Imagenes { get; set; } = new();
        public double PromedioValoracion { get; set; }
        public List<ResenaDto> Resenas { get; set; } = new();

        // Propiedades para manejar promociones
        public bool TienePromocion { get; set; }
        public decimal? PrecioConDescuento { get; set; }

        // Calcula automáticamente el porcentaje de descuento aplicado
        public decimal? PorcentajeDescuentoAplicado =>
            (TienePromocion && Precio.HasValue && PrecioConDescuento.HasValue && Precio > 0)
                ? (1 - (PrecioConDescuento.Value / Precio.Value)) * 100
                : null;
    }

    // DTO para representar reseñas de productos
    public record ResenaDto
    {
        public string Usuario { get; set; } = string.Empty;
        public DateTime? Fecha { get; set; }
        public string? Comentario { get; set; }
        public int? Valoracion { get; set; }
    }
}