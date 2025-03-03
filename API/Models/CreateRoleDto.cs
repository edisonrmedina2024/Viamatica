namespace API.Models
{
    public class CreateRoleDto
    {
        public string RolName { get; set; } = null!;
        public List<int> RolOpcionesId { get; set; } = new List<int>();

    }
}
