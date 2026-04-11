using System;
using System.Collections.Generic;

namespace VelourEssence.Infraestructure.Models;

public partial class ProductoPersonalizado
{
    public int IdProductoPersonalizado { get; set; }

    public int? IdUsuario { get; set; }

    public int IdProductoBase { get; set; }

    public string? Descripcion { get; set; }

    public decimal PrecioFinal { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public bool Activo { get; set; } = true;

    public virtual Producto? BaseProducto { get; set; }

    public virtual ICollection<CarritoProductoPersonalizado> CarritoProductoPersonalizado { get; set; } = new List<CarritoProductoPersonalizado>();

    public virtual Usuario? IdUsuarioNavigation { get; set; }
    
    // Nueva navegación para detalles de personalización
    public virtual ICollection<ProductoPersonalizadoDetalle> ProductoPersonalizadoDetalles { get; set; } = new List<ProductoPersonalizadoDetalle>();
}
