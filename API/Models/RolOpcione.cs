using System;
using System.Collections.Generic;

namespace API.Models;

public partial class RolOpcione
{
    public int IdOpcion { get; set; }

    public string NombreOpcion { get; set; } = null!;

    public string Route { get; set; } = null!;

    public virtual ICollection<Role> RolIdRols { get; set; } = new List<Role>();
}
