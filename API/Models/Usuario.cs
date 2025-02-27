using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Mail { get; set; } = null!;

    public string? SessionActive { get; set; }

    public string? Status { get; set; }

    public int PersonaIdPersona { get; set; }

    public int IntentosFallidos { get; set; }

    public virtual Persona PersonaIdPersonaNavigation { get; set; } = null!;

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    public virtual ICollection<Role> IdRols { get; set; } = new List<Role>();
}
