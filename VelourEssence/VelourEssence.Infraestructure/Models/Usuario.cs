using System;
using System.Collections.Generic;

namespace VelourEssence.Infraestructure.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string NombreUsuario { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Contraseña { get; set; } = null!;

    public DateTime? UltimoInicioSesion { get; set; }

    public int? IdRol { get; set; }

    public virtual ICollection<Carrito> Carrito { get; set; } = new List<Carrito>();

    public virtual Rol? IdRolNavigation { get; set; }

    public virtual ICollection<Pedido> Pedido { get; set; } = new List<Pedido>();

    public virtual ICollection<ProductoPersonalizado> ProductoPersonalizado { get; set; } = new List<ProductoPersonalizado>();

    public virtual ICollection<Reseña> Reseña { get; set; } = new List<Reseña>();

    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiration { get; set; }
}
