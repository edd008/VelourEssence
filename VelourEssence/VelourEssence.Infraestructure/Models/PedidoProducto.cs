using System;
using System.Collections.Generic;

namespace VelourEssence.Infraestructure.Models;

public partial class PedidoProducto
{
    public int IdPedido { get; set; }

    public int IdProducto { get; set; }

    public int? Cantidad { get; set; }

    public decimal? PrecioUnitario { get; set; }

    public virtual Pedido IdPedidoNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
