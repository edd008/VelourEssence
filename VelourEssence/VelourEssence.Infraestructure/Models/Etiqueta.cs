using System;
using System.Collections.Generic;

namespace VelourEssence.Infraestructure.Models;

public partial class Etiqueta
{
    public int IdEtiqueta { get; set; }

    public string? Nombre { get; set; }

    public virtual ICollection<Producto> IdProducto { get; set; } = new List<Producto>();
}
