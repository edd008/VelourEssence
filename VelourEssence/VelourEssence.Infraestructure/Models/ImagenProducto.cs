using System;
using System.Collections.Generic;

namespace VelourEssence.Infraestructure.Models;

public partial class ImagenProducto
{
    public int IdImagen { get; set; }

    public int? IdProducto { get; set; }

    public string UrlImagen { get; set; } = null!;

    public virtual Producto? IdProductoNavigation { get; set; }
}
