namespace API.Models
{
    public class RoleDTO
    {
        
        public int IdRol { get; set; }

        public string RolName { get; set; } = null!;

        public virtual ICollection<RolOpcione> RolOpcionesIdOpcions { get; set; } = new List<RolOpcione>();


    }
}

