using API.Application;
using API.Data.servicesData.services;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleApp _roleApp;

        public RoleController(IRoleApp usuarioService)
        {
            _roleApp = usuarioService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDTO>>> GetRoles()
        {
            return await _roleApp.ObtenerRoles();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDTO>> GetRolByIdUser(int id)
        {
            var role = await _roleApp.ObtenerRolesPorId(id);

            if (role == null)
            {
                return NotFound($"No se encontró el rol con ID {id}");
            }

            return Ok(role);
        }

        [HttpPost]
        public async Task<ActionResult<RoleDTO>> CreateRol(CreateRoleDto createRoleDto){
            return await _roleApp.CreateRol(createRoleDto);
        }

        [HttpPut]
        public async Task<ActionResult<RoleDTO>> UpdateRol(int id, CreateRoleDto createRoleDto)
        {
            return await _roleApp.UpdateRol(id, createRoleDto);
        }

        [HttpGet("menu/{username}")]
        public async Task<ActionResult<RoleDTO>> GetMenuByUserName(string username)
        {
            var role = await _roleApp.GetMenuByUserName(username);

            if (role == null)
            {
                return NotFound($"No se encontró un rol asignado para el usuario '{username}'");
            }

            return Ok(role);
        }

        [HttpGet("menus")]
        public async Task<ActionResult<RoleDTO>> getMenus()
        {
            var menus = await _roleApp.getMenus();

            if (menus == null)
            {
                return NotFound("No existen menus");
            }

            return Ok(menus);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> deleteRol(int id)
        {
            return await _roleApp.deleteRol(id);

        }
    }

}

