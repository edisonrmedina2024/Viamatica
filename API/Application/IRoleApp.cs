using API.Models;

namespace API.Application
{
    public interface IRoleApp
    {
        Task<List<RoleDTO>> ObtenerRoles();
        Task<RoleDTO> ObtenerRolesPorId(int id);
        Task<RoleDTO> GetMenuByUserName(string username);
    }
}
