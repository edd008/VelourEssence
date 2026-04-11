using System;
using System.Collections.Generic;

namespace VelourEssence.Infraestructure.Models;

public partial class Rol
{
    public int IdRol { get; set; }

    public string NombreRol { get; set; } = null!;

    public virtual ICollection<Usuario> Usuario { get; set; } = new List<Usuario>();
}
