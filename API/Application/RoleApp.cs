using API.Data.servicesData.services;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Application
{
    public class RoleApp : IRoleApp
    {
        private readonly IRolRepository _rolRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRolOpcionesRepository _rolOpcionesRepository;

        public RoleApp(
            IRolRepository rolRepository,
            IUserRepository userRepository,
            IRolOpcionesRepository rolOpcionesRepository
            )
        {
            _rolOpcionesRepository = rolOpcionesRepository;
            _rolRepository = rolRepository;
            _userRepository = userRepository;
        }

        [Authorize]
        public async Task<List<RoleDTO>> ObtenerRoles()
        {
            var roles = await _rolRepository.ListAllAsync();
            return roles.Select(r => new RoleDTO 
                {
                    IdRol = r.IdRol,
                    RolName = r.RolName,
                    RolOpcionesIdOpcions = r.RolOpcionesIdOpcions.Select(ro => new ReadRolOptionDto
                    {
                            IdOpcion = ro.IdOpcion,
                            NombreOpcion = ro.NombreOpcion,
                            Route = ro.Route,
                        }
                    ).ToList()
                }
            ).ToList();
        }
        [Authorize]
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
                RolOpcionesIdOpcions = role.RolOpcionesIdOpcions.Select(ro => new ReadRolOptionDto
                {
                    IdOpcion = ro.IdOpcion,
                    NombreOpcion = ro.NombreOpcion,
                    Route = ro.Route,
                }
                ).ToList()
            };
        }
        [Authorize]
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
        [Authorize]
        public async Task<RoleDTO> CreateRol(CreateRoleDto createRoleDto)
        {
            if(createRoleDto == null)
            {
                return null;
            }

            if(createRoleDto.RolOpcionesId.Any() == null)
            {
                return null;
            }

            var roles = await _rolRepository.ListAllAsync();
            
            var existeRol = roles.Exists(r => r.RolName.ToLower() == createRoleDto.RolName.ToLower());

            if (existeRol)
            {
                return null;
            }

            var rol = new Role()
            {
                RolName = createRoleDto.RolName,
                RolOpcionesIdOpcions = new List<RolOpcione>(),
            };

            var opciones = await _rolOpcionesRepository.ListAllAsync();
            var rangoOpciones = opciones.Where(
                        op => createRoleDto.RolOpcionesId.Contains(op.IdOpcion)
                        );

            rol.RolOpcionesIdOpcions.AddRange(rangoOpciones);
            var nuevoRol = await _rolRepository.AddAsync(rol);

            var listaOpciones = rol.RolOpcionesIdOpcions.Select(x =>
            
                new ReadRolOptionDto
                {
                    IdOpcion = x.IdOpcion,
                    NombreOpcion = x.NombreOpcion,
                    Route = x.Route,
                }
            ).ToList();

            return new RoleDTO
            {
                IdRol = nuevoRol.IdRol,
                RolName = createRoleDto.RolName,
                RolOpcionesIdOpcions = listaOpciones
            };
        }
        [Authorize]
        public async Task<RoleDTO> UpdateRol(int idRol, CreateRoleDto createRoleDto)
        {
            if (createRoleDto == null)
            {
                return null;
            }

            var rol = await _rolRepository.GetByIdAsync(idRol);

            if (rol == null)
            {
                return null; // No existe el rol
            }

            // Verifica si hay otro rol con el mismo nombre
            var roles = await _rolRepository.ListAllAsync();
            var existeOtroRol = roles.Any(r => r.RolName.ToLower() == createRoleDto.RolName.ToLower() && r.IdRol != idRol);

            if (existeOtroRol)
            {
                return null; // Ya existe otro rol con ese nombre
            }

            // Actualiza el nombre del rol
            rol.RolName = createRoleDto.RolName;

            // Actualiza las opciones del rol
            var opciones = await _rolOpcionesRepository.ListAllAsync();
            var nuevasOpciones = opciones.Where(op => createRoleDto.RolOpcionesId.Contains(op.IdOpcion)).ToList();

            rol.RolOpcionesIdOpcions.Clear(); // Limpia las opciones anteriores
            rol.RolOpcionesIdOpcions.AddRange(nuevasOpciones); // Agrega las nuevas opciones

            await _rolRepository.UpdateAsync(rol); // Guarda los cambios en la BD

            // Retorna el DTO actualizado
            var listaOpciones = rol.RolOpcionesIdOpcions.Select(x => new ReadRolOptionDto
            {
                IdOpcion = x.IdOpcion,
                NombreOpcion = x.NombreOpcion,
                Route = x.Route,
            }).ToList();

            return new RoleDTO
            {
                IdRol = rol.IdRol,
                RolName = rol.RolName,
                RolOpcionesIdOpcions = listaOpciones
            };
        }
        [Authorize]
        public async Task<List<ReadRolOptionDto>> getMenus()
        {

            var menus = (await _rolRepository.GetByIdAsync(2)).RolOpcionesIdOpcions.Select( ro => new ReadRolOptionDto
            {
                IdOpcion = ro.IdOpcion,
                NombreOpcion = ro.NombreOpcion,
                Route = ro.Route,
            }).ToList();

            return menus;
        }
        [Authorize]
        public async Task<ActionResult<bool>> deleteRol(int id)
        {
            return await _rolRepository.DeleteAsync(id);
        }
    }
}
