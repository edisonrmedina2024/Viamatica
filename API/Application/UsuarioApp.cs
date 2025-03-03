using API.Data.servicesData.services;
using API.Models;
using Azure.Core;
using System.ComponentModel.Design;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace API.Application
{
    public class UsuarioApp : IUsuarioApp
    { 
        private readonly IUserRepository _userRepository;
        private readonly IPersonaRepository _personaRepository;
        private readonly IConfiguration _configuration;

        public UsuarioApp(
            IUserRepository userRepository,
            IPersonaRepository personaRepository,
            IConfiguration configuration
            )
        {
            _userRepository = userRepository;
            _personaRepository = personaRepository;
            _configuration = configuration;
        }
        public async Task<bool> ActualizarUsuario(ActualizarUsuarioDto usuarioDto, string usuername)
        {
            if (usuarioDto == null)
            {
                throw new ArgumentNullException(nameof(usuarioDto));
            }

            var usuarioExistente = await _userRepository.ListAllAsync();
            var usuario = usuarioExistente.FirstOrDefault(u => u.UserName == usuername);

            if (usuario == null)
            {
                throw new ArgumentException("El usuario no existe.");
            }

            var persona = await _personaRepository.GetByIdAsync(usuario.PersonaIdPersona);

            if (persona == null)
            {
                throw new ArgumentException("La persona asociada al usuario no existe.");
            }

            if (!string.IsNullOrEmpty(usuarioDto.Identificacion) && !Regex.IsMatch(usuarioDto.Identificacion, @"^\d{10}$") ||
                Regex.IsMatch(usuarioDto.Identificacion, @"(\d)\1{3}"))
            {
                throw new ArgumentException("La identificación debe tener 10 dígitos y no contener 4 números seguidos iguales.");
            }

            if (!string.IsNullOrEmpty(usuarioDto.Nombres))
                persona.Nombres = usuarioDto.Nombres;

            if (!string.IsNullOrEmpty(usuarioDto.Apellidos))
                persona.Apellidos = usuarioDto.Apellidos;

            if (usuarioDto.FechaNacimiento.HasValue)
                persona.FechaNacimiento = usuarioDto.FechaNacimiento.Value;

            if (!string.IsNullOrEmpty(usuarioDto.Identificacion))
                persona.Identificacion = usuarioDto.Identificacion;

            if(!string.IsNullOrEmpty(usuarioDto.UserName))
                usuario.UserName = usuarioDto.UserName;

            // Guardar los cambios en la base de datos
            await _personaRepository.UpdateAsync(persona); 
            return await _userRepository.UpdateAsync(usuario); 
        }
        public async Task<Usuario> AgregarUsuario(CrearUsuarioDto personaDto)
        {
            if (personaDto == null)
            {
                throw new ArgumentNullException(nameof(personaDto));
            }

            // Validar la identificación
            if (!Regex.IsMatch(personaDto.Identificacion, @"^\d{10}$") ||
                Regex.IsMatch(personaDto.Identificacion, @"(\d)\1{3}"))
            {
                throw new ArgumentException("La identificación debe tener 10 dígitos y no contener 4 números seguidos iguales.");
            }

            // Validar la contraseña
            if (!ValidarPassword(personaDto.Password))
            {
                throw new ArgumentException("La contraseña no cumple con los requisitos.");
            }

            // Obtener todas las personas
            var personaExistente = await _personaRepository.ListAllAsync();
            var persona = personaExistente.FirstOrDefault(p => p.Identificacion == personaDto.Identificacion);

            if (persona == null)
            {
                persona = new Persona
                {
                    Nombres = personaDto.Nombres,
                    Apellidos = personaDto.Apellidos,
                    Identificacion = personaDto.Identificacion,
                    FechaNacimiento = personaDto.FechaNacimiento
                };

                await _personaRepository.AddAsync(persona);
            }

            var usuariosExistentes = await _userRepository.ListAllAsync();
            var baseUserName = personaDto.UserName;
            var userNameFinal = baseUserName;
            var contador = 1;

            while (usuariosExistentes.Any(u => u.UserName == userNameFinal))
            {
                userNameFinal = $"{baseUserName}{contador}";
                contador++;
            }

            // Generar un correo único
            var correoGenerado = $"{personaDto.Nombres[0].ToString().ToLower()}{personaDto.Apellidos.Split(' ')[0].ToLower()}@mail.com";
            var correoFinal = correoGenerado;
            contador = 1;

            while (usuariosExistentes.Any(u => u.Mail == correoFinal))
            {
                correoFinal = $"{personaDto.Nombres[0].ToString().ToLower()}{personaDto.Apellidos.Split(' ')[0].ToLower()}{contador}@mail.com";
                contador++;
            }

            var hashedPassword = BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(personaDto.Password))).Replace("-", "").ToLower();

            var usuarioNuevo = new Usuario
            {
                UserName = userNameFinal,
                Password = hashedPassword, // ✅ La contraseña viene del frontend y ya fue validada
                Mail = correoFinal,
                PersonaIdPersona = persona.IdPersona,
                Status = "Activo"
            };

            return await _userRepository.AddAsync(usuarioNuevo);
        }
         
        public async Task<bool> EliminarUsuario(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "El ID del usuario debe ser mayor a cero.");
            }

            return await _userRepository.DeleteAsync(id);

        }

        public async Task<Usuario> ObtenerUsuarioPorId(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "El ID del usuario debe ser mayor a cero.");
            }

            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<List<UsuarioDTO>> ObtenerUsuarios()
        {
            var usuarios = await _userRepository.ListAllAsync();

            return usuarios.Select(u => new UsuarioDTO
            {
                IdUsuario = u.IdUsuario,
                UserName = u.UserName,
                Mail = u.Mail,
                SessionActive = u.SessionActive,
                Status = u.Status,
                PersonaIdPersona = u.PersonaIdPersona,
                IdRoles = u.IdRols.Select(r => new RoleDTO 
                { 
                    IdRol = r.IdRol,
                    RolName = r.RolName,
                }
                ).ToList(),
                IdSessions = u.Sessions.Select(s => new SessionDTO 
                    {
                        IdSession = s.IdSession,
                        FechaIngreso = s.FechaIngreso,
                        FechaCierre = s.FechaCierre,
                    }
                ).ToList()
            }).ToList();
        }

        public async Task<LoginResult> LoginUsuarioAsync(LoginDto loginDto)
        {
            var result = await _userRepository.LoginUsuarioAsync(loginDto.Credencial,loginDto.Password);

            return result;
        }

        public async Task<LoginResult> CerrarSesionAsync(string correo)
        {
            var result = await _userRepository.CerrarSesionAsync(correo);

            return result;
        }

        private string GenerarNombreUsuario(string nombres, string apellidos)
        {
            string baseUserName = nombres.Substring(0, 2).ToUpper() + apellidos.Split(' ')[0];

            baseUserName = Regex.Replace(baseUserName, @"[^a-zA-Z0-9]", "");

            if (!baseUserName.Any(char.IsDigit))
            {
                baseUserName += "1";
            }

            if (baseUserName.Length < 8)
            {
                baseUserName = baseUserName.PadRight(8, 'X'); 
            }
            else if (baseUserName.Length > 20)
            {
                baseUserName = baseUserName.Substring(0, 20);
            }

            return baseUserName;
        }

        private bool ValidarPassword(string password)
        {

            return password.Length >= 8 &&
                   password.Any(char.IsUpper) &&
                   password.Any(ch => "!@#$%^&*()_+-=".Contains(ch)) &&
                   !password.Contains(" ");
        }

        public async Task<DashboardStatsDTO> DashboardStats()
        {
            // Ejecutar el procedimiento almacenado para cerrar sesión
            var usuarios = await _userRepository.ListAllAsync();
            var activos = usuarios.Where(u => u.SessionActive == "Y").Count();
            var inactivos = usuarios.Where(u => u.SessionActive == "N").Count();
            var bloqueados = usuarios.Where(u => u.Status == "Bloqueado           ").Count();
            var totalIntentosFallidos = usuarios.Where(u => u.IntentosFallidos > 0).Sum(u => u.IntentosFallidos);

            // Devolver el resultado del procedimiento
            return new DashboardStatsDTO
            {
                ActiveUsers = activos,
                BlockedUsers = bloqueados,
                FailedLogins = totalIntentosFallidos,
                InactiveUsers = inactivos
            };

        }

        public async Task<ProfileDTO> profile(string username)
        {
            var usuarios = await _userRepository.ListAllAsync();
            var ususario = usuarios.Where(u => u.UserName == username || u.Mail == username).FirstOrDefault();
            var ultimaSession = ususario.Sessions.OrderByDescending(s => s.FechaCierre).Skip(1).FirstOrDefault();

            var ultimaSessionInicio = ususario.Sessions.LastOrDefault().FechaIngreso.ToString();
            var ultimaSessionFin = ususario.Sessions.LastOrDefault().FechaCierre.ToString();

            return new ProfileDTO
            {
                email = ususario.Mail,
                usuario = ususario.UserName,
                intentoFallidos = ususario.IntentosFallidos,
                ultimaSession = ultimaSessionInicio,
                ultimaSessionInicio = ultimaSessionInicio,
                ultimaSessionFin = ultimaSessionFin,
            };

        }

        public async Task<bool> recoveryPassword(RecoveryPasswordDTO recoveryPasswordDTO)
        {
            var usuarios = await _userRepository.ListAllAsync();
            var ususario = usuarios.Where(u => u.UserName == recoveryPasswordDTO.Username || u.Mail == recoveryPasswordDTO.Username).FirstOrDefault();
            var hashedPassword = BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(recoveryPasswordDTO.NewPassword))).Replace("-", "").ToLower();
            ususario.Password = hashedPassword;

            return await _userRepository.UpdateAsync(ususario);
        }


    }
}
