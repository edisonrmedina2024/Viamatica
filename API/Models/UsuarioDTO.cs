namespace API.Models
{
    public class UsuarioDTO
    {
        public int IdUsuario { get; set; }

        public string UserName { get; set; } = null!;

        public string Mail { get; set; } = null!;

        public string? SessionActive { get; set; }

        public string? Status { get; set; }

        public int PersonaIdPersona { get; set; }

        public List<RoleDTO> IdRoles { get; set; } = new List<RoleDTO>();

        public List<SessionDTO> IdSessions { get; set; } = new List<SessionDTO>();
    }
}
