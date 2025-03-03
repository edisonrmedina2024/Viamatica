using API.Data.servicesData.services;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Application
{
    public interface IRoleApp
    {
        Task<List<RoleDTO>> ObtenerRoles();
        Task<RoleDTO> ObtenerRolesPorId(int id);
        Task<RoleDTO> GetMenuByUserName(string username);
        Task<RoleDTO> CreateRol(CreateRoleDto createRoleDto);
        Task<RoleDTO> UpdateRol(int idRol, CreateRoleDto createRoleDto);
        Task<List<ReadRolOptionDto>> getMenus();
        Task<ActionResult<bool>> deleteRol(int id);
    }
}
