using System.ComponentModel.DataAnnotations;

namespace VelourEssence.Application.DTOs
{
    // Enumeración para los métodos de pago
    public enum MetodoPago
    {
        TarjetaCredito = 1,
        TarjetaDebito = 2,
        Efectivo = 3
    }

    // DTO base para el proceso de pago
    public class ProcesarPagoDto
    {
        [Required(ErrorMessage = "El método de pago es obligatorio")]
        public MetodoPago MetodoPago { get; set; }

        [Required(ErrorMessage = "El total del pedido es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El total debe ser mayor a 0")]
        public decimal TotalPedido { get; set; }

        public int IdCarrito { get; set; }
    }

    // DTO para pago con tarjeta (crédito o débito)
    public class PagoTarjetaDto : ProcesarPagoDto
    {
        [Required(ErrorMessage = "El número de tarjeta es obligatorio")]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "El número de tarjeta debe tener 16 dígitos")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "El número de tarjeta debe contener solo dígitos")]
        public string NumeroTarjeta { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de expiración es obligatoria")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "La fecha debe tener el formato MM/AA")]
        public string FechaExpiracion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El código CVV es obligatorio")]
        [StringLength(4, MinimumLength = 3, ErrorMessage = "El CVV debe tener 3 o 4 dígitos")]
        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "El CVV debe contener solo dígitos")]
        public string CodigoCVV { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre del titular es obligatorio")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
        public string NombreTitular { get; set; } = string.Empty;
    }

    // DTO para pago en efectivo
    public class PagoEfectivoDto : ProcesarPagoDto
    {
        [Required(ErrorMessage = "El monto pagado es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal MontoPagado { get; set; }

        public decimal Vuelto => MontoPagado - TotalPedido;
    }

    // DTO para el resultado del pago
    public class ResultadoPagoDto
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public string NumeroTransaccion { get; set; } = string.Empty;
        public MetodoPago MetodoPago { get; set; }
        public decimal TotalPagado { get; set; }
        public decimal? Vuelto { get; set; }
        public DateTime FechaPago { get; set; }
        public int IdPedido { get; set; }
        public int IdPago { get; set; }
    }

    // DTO para mostrar información del pago
    public class PagoInfoDto
    {
        public int IdPago { get; set; }
        public int IdPedido { get; set; }
        public string TipoPago { get; set; } = string.Empty;
        public decimal MontoTotal { get; set; }
        public DateTime FechaPago { get; set; }
        public string EstadoPago { get; set; } = string.Empty;
        public string? NumeroTarjetaEnmascarado { get; set; }
        public string? NombreTitular { get; set; }
        public decimal? MontoRecibido { get; set; }
        public decimal? Vuelto { get; set; }
        
        // Información del pedido
        public DateTime FechaPedido { get; set; }
        public string EstadoPedido { get; set; } = string.Empty;
        public string DireccionEnvio { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal Impuesto { get; set; }
        public int CantidadProductos { get; set; }
    }

    // DTO para validación de tarjeta
    public class ValidacionTarjetaDto
    {
        public bool EsValida { get; set; }
        public List<string> Errores { get; set; } = new List<string>();
        public string TipoTarjeta { get; set; } = string.Empty; // Visa, Mastercard, etc.
    }

    public class SeleccionMetodoPagoDto
    {
        public int IdCarrito { get; set; }
        public decimal TotalPedido { get; set; }
        public List<string> MetodosPagoDisponibles { get; set; } = new List<string>();
    }

    public class ConfirmacionPagoDto
    {
        public PagoInfoDto Pago { get; set; } = new PagoInfoDto();
        public string NumeroTransaccion { get; set; } = string.Empty;
        public string? MensajeExito { get; set; }
        public decimal? Vuelto { get; set; }
    }
}
