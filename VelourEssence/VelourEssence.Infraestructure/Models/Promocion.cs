using System;
using System.Collections.Generic;

namespace VelourEssence.Infraestructure.Models;

public partial class Promocion
{
    public int IdPromocion { get; set; }

    public string? Nombre { get; set; }

    public string? Tipo { get; set; }

    public int? IdProducto { get; set; }

    public int? IdCategoria { get; set; }

    public decimal? PorcentajeDescuento { get; set; }

    public DateOnly? FechaInicio { get; set; }

    public DateOnly? FechaFin { get; set; }

    public virtual Categoria? IdCategoriaNavigation { get; set; }

    public virtual Producto? IdProductoNavigation { get; set; }
}
