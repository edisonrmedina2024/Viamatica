using API.Data.servicesData.services;

namespace API.Models
{
    public class RoleDTO
    {
        
        public int IdRol { get; set; }

        public string RolName { get; set; } = null!;

        public virtual List<ReadRolOptionDto> RolOpcionesIdOpcions { get; set; } = new List<ReadRolOptionDto>();


    }
}

