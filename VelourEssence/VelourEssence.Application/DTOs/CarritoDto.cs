namespace VelourEssence.Application.DTOs
{
    public class CarritoDto
    {
        public int IdCarrito { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaCreacion { get; set; }
        public List<CarritoProductoDto> Productos { get; set; } = new List<CarritoProductoDto>();
        public List<CarritoProductoPersonalizadoDto> ProductosPersonalizados { get; set; } = new List<CarritoProductoPersonalizadoDto>();
        public decimal Total { get; set; }
        public int CantidadTotal { get; set; }
        public decimal Impuestos { get; set; }
        public decimal TotalConImpuestos { get; set; }
    }

    public class CarritoProductoDto
    {
        public int IdCarritoProducto { get; set; }
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Impuesto { get; set; }
        public decimal SubtotalConImpuesto { get; set; }
        public string? ImagenUrl { get; set; }
        public int Stock { get; set; }
    }

    public class CarritoProductoPersonalizadoDto
    {
        public int IdCarritoProductoPersonalizado { get; set; }
        public int IdProductoBase { get; set; }
        public string NombreProductoBase { get; set; } = string.Empty;
        public decimal PrecioBase { get; set; }
        public int Cantidad { get; set; }
        public List<PersonalizacionSeleccionadaDto> Personalizaciones { get; set; } = new List<PersonalizacionSeleccionadaDto>();
        public decimal TotalPersonalizacion { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Impuesto { get; set; }
        public decimal SubtotalConImpuesto { get; set; }
        public string? ImagenUrl { get; set; }
    }

    public class PersonalizacionSeleccionadaDto
    {
        public string NombreCriterio { get; set; } = string.Empty;
        public string NombreOpcion { get; set; } = string.Empty;
        public decimal Costo { get; set; }
    }

    public class CarritoProductoPersonalizadoRequestDto
    {
        public int IdProductoBase { get; set; }
        public int Cantidad { get; set; }
        public List<CarritoSeleccionPersonalizacionDto> Selecciones { get; set; } = new List<CarritoSeleccionPersonalizacionDto>();
    }

    public class CarritoSeleccionPersonalizacionDto
    {
        public int IdCriterio { get; set; }
        public int IdOpcion { get; set; }
    }
}
