using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Session
{
    public int IdSession { get; set; }

    public DateTime FechaIngreso { get; set; }

    public DateTime? FechaCierre { get; set; }

    public int UsuariosIdUsuario { get; set; }

    public virtual Usuario UsuariosIdUsuarioNavigation { get; set; } = null!;
}
