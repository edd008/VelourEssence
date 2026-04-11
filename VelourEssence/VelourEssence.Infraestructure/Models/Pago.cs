using System;

namespace VelourEssence.Infraestructure.Models;

public partial class Pago
{
    public int IdPago { get; set; }

    public int IdPedido { get; set; }

    public string TipoPago { get; set; } = null!; // "TarjetaCredito", "TarjetaDebito", "Efectivo"

    public decimal MontoTotal { get; set; }

    public DateTime FechaPago { get; set; }

    public string EstadoPago { get; set; } = null!; // "Pendiente", "Completado", "Fallido"

    // Campos específicos para tarjeta (se almacenan encriptados en un sistema real)
    public string? NumeroTarjetaEnmascarado { get; set; } // Solo últimos 4 dígitos para referencia

    public string? NombreTitular { get; set; }

    public string? FechaExpiracion { get; set; } // MM/YY

    // Campos específicos para efectivo
    public decimal? MontoRecibido { get; set; }

    public decimal? Vuelto { get; set; }

    // Navegación
    public virtual Pedido IdPedidoNavigation { get; set; } = null!;
}
