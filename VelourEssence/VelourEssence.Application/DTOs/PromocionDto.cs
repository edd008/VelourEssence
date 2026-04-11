namespace VelourEssence.Application.DTOs
{
    // DTO para mostrar una promoción o descuento aplicado a productos o categorías
    public record PromocionDto
    {
        // Identificador único de la promoción
        public int IdPromocion { get; set; }

        // Nombre descriptivo de la promoción
        public string? Nombre { get; set; }

        // Tipo de promoción (por ejemplo: "Producto", "Categoría", etc.)
        public string? Tipo { get; set; }

        // Porcentaje de descuento aplicado (puede ser null si no definido)
        public decimal? PorcentajeDescuento { get; set; }

        // Fecha de inicio de la promoción
        public DateOnly? FechaInicio { get; set; }

        // Fecha final de la promoción
        public DateOnly? FechaFin { get; set; }

        // Nombre del producto al que aplica la promoción (solo nombre, no ID)
        public string? NombreProducto { get; set; }

        // Nombre de la categoría a la que aplica la promoción (solo nombre, no ID)
        public string? NombreCategoria { get; set; }

        // Identificador de la categoría a la que aplica la promoción (opcional)
        public int? IdCategoria { get; set; }

        // Aquí podrías agregar más propiedades si el modelo lo requiere
        public int IdProducto { get; set; }

        public int ProductoId { get; set; }

        // Estado dinámico
        public string Estado
        {
            get
            {
                var hoy = DateOnly.FromDateTime(DateTime.Today);
                if (hoy < FechaInicio) return "Pendiente";
                if (hoy > FechaFin) return "Aplicado";
                return "Vigente";
            }
        }

        // Color representativo (opcional si lo ocupás directamente desde el DTO)
        public string ColorEstado => Estado switch
        {
            "Pendiente" => "#ADD8E6",
            "Vigente" => "#FF4D4D",
            "Aplicado" => "#D3D3D3",
            _ => "#FFFFFF"
        };
    }
}
