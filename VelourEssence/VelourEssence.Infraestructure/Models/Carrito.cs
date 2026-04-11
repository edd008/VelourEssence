using System;
using System.Collections.Generic;

namespace VelourEssence.Infraestructure.Models;

public partial class Carrito
{
    public int IdCarrito { get; set; }

    public int? IdUsuario { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<CarritoProducto> CarritoProducto { get; set; } = new List<CarritoProducto>();

    public virtual ICollection<CarritoProductoPersonalizado> CarritoProductoPersonalizado { get; set; } = new List<CarritoProductoPersonalizado>();

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
