using API.Data.servicesData.services;
using API.Models;

namespace API.Application
{
    public class RoleApp : IRoleApp
    {
        private readonly IRolRepository _rolRepository;
        private readonly IUserRepository _userRepository;

        public RoleApp(IRolRepository rolRepository, IUserRepository userRepository)
        {
            _rolRepository = rolRepository;
            _userRepository = userRepository;
        }

        public async Task<List<RoleDTO>> ObtenerRoles()
        {
            var roles = await _rolRepository.ListAllAsync();
            return roles.Select(r => new RoleDTO 
                {
                    IdRol = r.IdRol,
                    RolName = r.RolName,
                    RolOpcionesIdOpcions = r.RolOpcionesIdOpcions.Select(ro => new RolOpcione
                        {
                            IdOpcion = ro.IdOpcion,
                            NombreOpcion = ro.NombreOpcion,
                            Route = ro.Route,
                            RolIdRols = ro.RolIdRols,
                        }
                    ).ToList()
                }
            ).ToList();
        }

        public async Task<RoleDTO> ObtenerRolesPorId(int id)
        {
            var role = await _rolRepository.GetByIdAsync(id);

            if (role == null)
            {
                return null;
            }

            return new RoleDTO
            {
                IdRol = role.IdRol,
                RolName = role.RolName,
                RolOpcionesIdOpcions = role.RolOpcionesIdOpcions.Select(ro => new RolOpcione
                {
                    IdOpcion = ro.IdOpcion,
                    NombreOpcion = ro.NombreOpcion,
                    Route = ro.Route,
                }
                ).ToList()
            };
        }

        public async Task<RoleDTO> GetMenuByUserName(string username)
        {
            var users = await _userRepository.ListAllAsync();
            var user = users.FirstOrDefault(u => u.UserName == username || u.Mail == username);

            if (user == null || user.IdRols == null || !user.IdRols.Any())
            {
                return null; 
            }
            var idRol = user.IdRols.FirstOrDefault().IdRol.ToString();
            var role = await ObtenerRolesPorId(Convert.ToInt32(idRol));
            return role;
        }
    }
}
