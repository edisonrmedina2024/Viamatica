using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Persona
{
    public int IdPersona { get; set; }

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string Identificacion { get; set; } = null!;

    public DateTime FechaNacimiento { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
