using System;
using System.Collections.Generic;

namespace VelourEssence.Infraestructure.Models;

public partial class Producto
{
    public int IdProducto { get; set; }

    public string? Nombre { get; set; }

    public string? Descripcion { get; set; }

    public decimal? Precio { get; set; }

    public int? Stock { get; set; }

    public int? IdCategoria { get; set; }

    public string? Marca { get; set; }

    public string? Concentracion { get; set; }

    public string? Genero { get; set; }

    public virtual ICollection<CarritoProducto> CarritoProducto { get; set; } = new List<CarritoProducto>();

    public virtual Categoria? IdCategoriaNavigation { get; set; }

    public virtual ICollection<ImagenProducto> ImagenProducto { get; set; } = new List<ImagenProducto>();

    public virtual ICollection<PedidoProducto> PedidoProducto { get; set; } = new List<PedidoProducto>();

    public virtual ICollection<ProductoPersonalizado> ProductoPersonalizado { get; set; } = new List<ProductoPersonalizado>();

    public virtual ICollection<Promocion> Promocion { get; set; } = new List<Promocion>();

    public virtual ICollection<Reseña> Reseña { get; set; } = new List<Reseña>();

    public virtual ICollection<Etiqueta> IdEtiqueta { get; set; } = new List<Etiqueta>();
}
