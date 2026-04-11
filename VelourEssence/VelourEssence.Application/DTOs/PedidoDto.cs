using System.ComponentModel.DataAnnotations;

namespace VelourEssence.Application.DTOs
{
    public class PedidoDto
    {
        public int IdPedido { get; set; }
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string EmailUsuario { get; set; } = string.Empty;
        public string DireccionEnvio { get; set; } = string.Empty;
        public DateTime FechaPedido { get; set; }
        public string Estado { get; set; } = string.Empty;
        public List<PedidoProductoDto> Productos { get; set; } = new List<PedidoProductoDto>();
        public decimal Subtotal { get; set; }
        public decimal Impuestos { get; set; }
        public decimal Total { get; set; }
        public int CantidadTotal { get; set; }
    }

    public class PedidoProductoDto
    {
        public int IdPedidoProducto { get; set; }
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Impuesto { get; set; }
        public decimal SubtotalConImpuesto { get; set; }
        public bool EsPersonalizado { get; set; }
        public List<PersonalizacionSeleccionadaDto>? Personalizaciones { get; set; }
        public decimal? PrecioBase { get; set; }
        public decimal? TotalPersonalizacion { get; set; }
        public string? ImagenUrl { get; set; }
    }

    public class CrearPedidoProductoDto
    {
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public bool EsPersonalizado { get; set; }
        public List<CarritoSeleccionPersonalizacionDto>? Personalizaciones { get; set; }
    }

    public class CrearPedidoDto
    {
        public int IdUsuario { get; set; }
        
        [Required(ErrorMessage = "La fecha es requerida")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "La dirección de envío es requerida")]
        [StringLength(500, ErrorMessage = "La dirección no puede exceder 500 caracteres")]
        public string DireccionEnvio { get; set; } = string.Empty;

        [StringLength(100)]
        public string Estado { get; set; } = "Pendiente";

        [StringLength(100)]
        public string MetodoPago { get; set; } = string.Empty;

        // Lista de productos normales del pedido
        public List<ProductoPedidoCrearDto> ProductosDelPedido { get; set; } = new();

        // Lista de productos personalizados del pedido
        public List<ProductoPersonalizadoPedidoDto> ProductosPersonalizadosDelPedido { get; set; } = new();

        // Campos calculados (solo lectura)
        public decimal Subtotal { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Total { get; set; }

        // Mantener compatibilidad con el DTO original
        public List<CrearPedidoProductoDto> Productos { get; set; } = new List<CrearPedidoProductoDto>();
    }

    public class ProductoPedidoCrearDto
    {
        [Required]
        public int IdProducto { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal PrecioUnitario { get; set; }

        public string? NombreProducto { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;
        public decimal ImpuestoLinea { get; set; }
        public decimal TotalLinea => Subtotal + ImpuestoLinea;
    }

    public class ProductoPersonalizadoPedidoDto
    {
        [Required]
        public int IdProductoPersonalizado { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal PrecioUnitario { get; set; }

        public string? NombreProductoPersonalizado { get; set; }
        public string? NombreProductoBase { get; set; }
        public decimal CostoProductoBase { get; set; }
        public List<CriterioSeleccionadoDto> CriteriosSeleccionados { get; set; } = new List<CriterioSeleccionadoDto>();

        public decimal Subtotal => Cantidad * PrecioUnitario;
        public decimal ImpuestoLinea { get; set; }
        public decimal TotalLinea => Subtotal + ImpuestoLinea;
    }

    public class CriterioSeleccionadoDto
    {
        public string NombreCriterio { get; set; } = string.Empty;
        public string OpcionSeleccionada { get; set; } = string.Empty;
        public decimal CostoOpcion { get; set; }
    }

    public class EditarPedidoDto
    {
        [Required]
        public int IdPedido { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "La dirección de envío es requerida")]
        [StringLength(500, ErrorMessage = "La dirección no puede exceder 500 caracteres")]
        public string DireccionEnvio { get; set; } = string.Empty;

        [StringLength(100)]
        public string Estado { get; set; } = string.Empty;

        [StringLength(100)]
        public string MetodoPago { get; set; } = string.Empty;

        public List<ProductoPedidoCrearDto> ProductosDelPedido { get; set; } = new List<ProductoPedidoCrearDto>();
        public List<ProductoPersonalizadoPedidoDto> ProductosPersonalizadosDelPedido { get; set; } = new List<ProductoPersonalizadoPedidoDto>();

        public decimal Subtotal { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Total { get; set; }

        public string? NombreCliente { get; set; }
    }

    public class CalcularTotalesRequest
    {
        public List<ProductoPedidoCrearDto> Productos { get; set; } = new List<ProductoPedidoCrearDto>();
        public List<ProductoPersonalizadoPedidoDto> ProductosPersonalizados { get; set; } = new List<ProductoPersonalizadoPedidoDto>();
    }

    public class CalcularTotalesResponse
    {
        public decimal Subtotal { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Total { get; set; }
        public List<LineaCalculadaDto> LineasCalculadas { get; set; } = new List<LineaCalculadaDto>();
    }

    public class LineaCalculadaDto
    {
        public int TipoProducto { get; set; } // 1 = Normal, 2 = Personalizado
        public int IdItem { get; set; }
        public decimal SubtotalLinea { get; set; }
        public decimal ImpuestoLinea { get; set; }
        public decimal TotalLinea { get; set; }
    }

    public class ProductoParaPedidoDto
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public decimal PrecioConPromocion { get; set; }
        public bool TienePromocion { get; set; }
        public string? DescripcionPromocion { get; set; }
        public bool EstaDisponible { get; set; }
        public int Stock { get; set; }
    }

    public class PagoDto
    {
        public int IdPago { get; set; }
        public int IdPedido { get; set; }
        public string MetodoPago { get; set; } = string.Empty; // "tarjeta_credito", "tarjeta_debito", "efectivo"
        public decimal MontoTotal { get; set; }
        public DateTime FechaPago { get; set; } = DateTime.Now;
        
        // Para tarjetas
        public string? NumeroTarjeta { get; set; }
        public string? NumeroTarjetaEnmascarada { get; set; }
        public string? FechaExpiracion { get; set; }
        public string? CodigoSeguridad { get; set; }
        public string? NombreTitular { get; set; }
        
        // Para efectivo
        public decimal? MontoPagado { get; set; }
        public decimal? MontoRecibido { get; set; }
        public decimal? Vuelto { get; set; }
    }
}
