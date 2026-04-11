using System;
using System.Collections.Generic;

namespace VelourEssence.Infraestructure.Models;

public partial class EstadoPedido
{
    public int IdEstadoPedido { get; set; }

    public string? NombreEstado { get; set; }

    public virtual ICollection<Pedido> Pedido { get; set; } = new List<Pedido>();
}
