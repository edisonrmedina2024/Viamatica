using API.Data.servicesData.services;
using API.Models;
using System.ComponentModel.Design;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Xml;

namespace API.Application
{
    public class UsuarioApp : IUsuarioApp
    { 
        private readonly IUserRepository _userRepository;
        private readonly IPersonaRepository _personaRepository;

        public UsuarioApp(IUserRepository userRepository, IPersonaRepository personaRepository)
        {
            _userRepository = userRepository;
            _personaRepository = personaRepository;
        }

        // Método para actualizar un usuario
        public async Task ActualizarUsuario(Usuario usuario)
        {
            if (usuario == null)
            {
                throw new ArgumentNullException(nameof(usuario));
            }

            await _userRepository.UpdateAsync(usuario);
        }

        // Método para agregar un nuevo usuario
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

            // ✅ Generar un nombre de usuario válido
            var usuariosExistentes = await _userRepository.ListAllAsync();
            var baseUserName = GenerarNombreUsuario(personaDto.Nombres, personaDto.Apellidos);
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

            // Crear usuario
            var usuarioNuevo = new Usuario
            {
                UserName = userNameFinal,
                Password = personaDto.Password, // ✅ La contraseña viene del frontend y ya fue validada
                Mail = correoFinal,
                PersonaIdPersona = persona.IdPersona,
                Status = "Activo"
            };

            return await _userRepository.AddAsync(usuarioNuevo);
        }

        // Método para eliminar un usuario
        public async Task EliminarUsuario(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "El ID del usuario debe ser mayor a cero.");
            }

            await _userRepository.DeleteAsync(id);
        }

        // Método para obtener un usuario por su ID
        public async Task<Usuario> ObtenerUsuarioPorId(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "El ID del usuario debe ser mayor a cero.");
            }

            return await _userRepository.GetByIdAsync(id);
        }

        // Método para obtener todos los usuarios
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
            // Ejecutar el procedimiento almacenado y mapear el resultado
            var result = await _userRepository.LoginUsuarioAsync(loginDto.Credencial,loginDto.Password);

            return result;
        }

        public async Task<LoginResult> CerrarSesionAsync(string correo)
        {
            // Ejecutar el procedimiento almacenado para cerrar sesión
            var result = await _userRepository.CerrarSesionAsync(correo);

            // Devolver el resultado del procedimiento
            return result;
        }

        private string GenerarNombreUsuario(string nombres, string apellidos)
        {
            // 1. Tomar las primeras dos letras del nombre y el primer apellido
            string baseUserName = nombres.Substring(0, 2).ToUpper() + apellidos.Split(' ')[0];

            // 2. Eliminar caracteres especiales
            baseUserName = Regex.Replace(baseUserName, @"[^a-zA-Z0-9]", "");

            // 3. Asegurar que tenga al menos un número (se agrega '1' al final si no tiene)
            if (!baseUserName.Any(char.IsDigit))
            {
                baseUserName += "1";
            }

            // 4. Asegurar que la longitud sea entre 8 y 20 caracteres
            if (baseUserName.Length < 8)
            {
                baseUserName = baseUserName.PadRight(8, 'X'); // Rellenar con "X"
            }
            else if (baseUserName.Length > 20)
            {
                baseUserName = baseUserName.Substring(0, 20);
            }

            return baseUserName;
        }

        private bool ValidarPassword(string password)
        {
            // Debe tener al menos 8 caracteres, al menos una mayúscula, un signo y no debe contener espacios
            return password.Length >= 8 &&
                   password.Any(char.IsUpper) &&
                   password.Any(ch => "!@#$%^&*()_+-=".Contains(ch)) &&
                   !password.Contains(" ");
        }

        public async Task<DashboardStatsDTO> DashboardStats()
        {
            // Ejecutar el procedimiento almacenado para cerrar sesión
            var users = await _userRepository.ListAllAsync();
            var activos = users.Where(u => u.SessionActive == "Y").Count();
            var inactivos = users.Where(u => u.SessionActive == "N").Count();
            var bloqueados = users.Where(u => u.Status == "Bloqueado           ").Count();
            var totalIntentosFallidos = users.Where(u => u.IntentosFallidos > 0).Sum(u => u.IntentosFallidos);

            // Devolver el resultado del procedimiento
            return new DashboardStatsDTO
            {
                ActiveUsers = activos,
                BlockedUsers = bloqueados,
                FailedLogins = totalIntentosFallidos,
                InactiveUsers = inactivos
            };
        }

    }
}
