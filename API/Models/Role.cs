using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Role
{
    public int IdRol { get; set; }

    public string RolName { get; set; } = null!;

    public virtual ICollection<Usuario> IdUsuarios { get; set; } = new List<Usuario>();

    public virtual ICollection<RolOpcione> RolOpcionesIdOpcions { get; set; } = new List<RolOpcione>();
}
