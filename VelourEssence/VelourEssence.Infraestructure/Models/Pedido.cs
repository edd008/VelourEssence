using System;
using System.Collections.Generic;

namespace VelourEssence.Infraestructure.Models;

public partial class Pedido
{
    public int IdPedido { get; set; }

    public int? IdUsuario { get; set; }

    public DateTime? FechaPedido { get; set; }

    public string? DireccionEnvio { get; set; }

    public string? MetodoPago { get; set; }

    public decimal? Subtotal { get; set; }

    public decimal? Impuesto { get; set; }

    public decimal? Total { get; set; }

    public int? IdEstadoPedido { get; set; }

    public virtual EstadoPedido? IdEstadoPedidoNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }

    public virtual ICollection<PedidoProducto> PedidoProducto { get; set; } = new List<PedidoProducto>();

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
