using System;
using System.Collections.Generic;

namespace VelourEssence.Infraestructure.Models;

public partial class Reseña
{
    public int IdReseña { get; set; }

    public int? IdUsuario { get; set; }

    public int? IdProducto { get; set; }

    public DateTime? Fecha { get; set; }

    public string? Comentario { get; set; }

    public int? Valoracion { get; set; }

    public virtual Producto? IdProductoNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
