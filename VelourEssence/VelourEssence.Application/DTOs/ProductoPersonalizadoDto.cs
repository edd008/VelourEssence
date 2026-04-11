namespace VelourEssence.Application.DTOs
{
    /// <summary>
    /// DTO para selecciones del usuario
    /// </summary>
    public class SeleccionPersonalizacionDto
    {
        public int IdCriterio { get; set; }
        public int IdOpcion { get; set; }
        public string? ValorPersonalizado { get; set; }
    }

    /// <summary>
    /// DTO para mostrar producto personalizado completo
    /// </summary>
    public class ProductoPersonalizadoDetalleDto
    {
        public int IdProductoPersonalizado { get; set; }
        public ProductoDto ProductoBase { get; set; } = null!;
        public string? Descripcion { get; set; }
        public decimal PrecioFinal { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public List<DetalleSeleccionDto> Selecciones { get; set; } = new();
    }

    /// <summary>
    /// DTO para detalle de selección
    /// </summary>
    public class DetalleSeleccionDto
    {
        public string NombreCriterio { get; set; } = null!;
        public string NombreOpcion { get; set; } = null!;
        public decimal PrecioOpcion { get; set; }
        public string? ImagenUrl { get; set; }
        public string? IconoClase { get; set; }
        public string? ValorPersonalizado { get; set; }
    }

    /// <summary>
    /// DTO para cálculo de precio en tiempo real
    /// </summary>
    public class CalculoPrecioPersonalizadoDto
    {
        public int BaseProductoId { get; set; }
        public List<SeleccionPersonalizacionDto> Selecciones { get; set; } = new();
        public decimal PrecioBase { get; set; }
        public decimal PrecioPersonalizaciones { get; set; }
        public decimal PrecioTotal { get; set; }
        public bool EsValido { get; set; }
        public List<string> Errores { get; set; } = new();
    }

    /// <summary>
    /// DTO para mostrar información completa de un producto personalizado
    /// </summary>
    public class ProductoPersonalizadoDto
    {
        public int IdProductoPersonalizado { get; set; }
        public int IdUsuario { get; set; }
        public int IdProductoBase { get; set; }
        public string? Descripcion { get; set; }
        public decimal PrecioFinal { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Activo { get; set; }
        
        // Información del producto base
        public string NombreProductoBase { get; set; } = null!;
        public string? DescripcionProductoBase { get; set; }
        
        // Información del usuario
        public string NombreUsuario { get; set; } = null!;
        
        // Detalles de personalización
        public List<ProductoPersonalizadoDetalleDto> Detalles { get; set; } = new();
    }
}
