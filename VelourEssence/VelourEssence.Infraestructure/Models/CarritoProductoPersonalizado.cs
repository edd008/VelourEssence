using System;
using System.Collections.Generic;

namespace VelourEssence.Infraestructure.Models;

public partial class CarritoProductoPersonalizado
{
    public int IdCarrito { get; set; }

    public int IdProductoPersonalizado { get; set; }

    public int? Cantidad { get; set; }

    public virtual Carrito IdCarritoNavigation { get; set; } = null!;

    public virtual ProductoPersonalizado IdProductoPersonalizadoNavigation { get; set; } = null!;
}
