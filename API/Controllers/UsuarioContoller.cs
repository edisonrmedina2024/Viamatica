using API.Application;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioContoller : ControllerBase
    {
        private readonly IUsuarioApp _usuarioService;

        public UsuarioContoller(IUsuarioApp usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> GetUsuarios()
        {
            return await _usuarioService.ObtenerUsuarios();
        }

        [HttpPost]
        public async Task<ActionResult<Usuario>> CreateUsuario(CrearUsuarioDto usuario)
        {
            return await _usuarioService.AgregarUsuario(usuario);
        }

        // Obtener un usuario por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuarioPorId(int id)
        {
            try
            {
                var usuario = await _usuarioService.ObtenerUsuarioPorId(id);

                if (usuario == null)
                {
                    return NotFound(new { mensaje = "Usuario no encontrado" });
                }

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "Error al obtener el usuario", error = ex.Message });
            }
        }

        // Actualizar un usuario existente
        [HttpPut]
        public async Task<ActionResult> UpdateUsuario([FromBody] Usuario usuario)
        {
            if (usuario == null)
            {
                return BadRequest(new { mensaje = "Los datos del usuario son inválidos" });
            }

            try
            {
                await _usuarioService.ActualizarUsuario(usuario);
                return NoContent(); // Devuelve un status 204 (sin contenido)
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "Error al actualizar el usuario", error = ex.Message });
            }
        }

        // Eliminar un usuario por ID
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUsuario(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { mensaje = "ID de usuario inválido" });
            }

            try
            {
                await _usuarioService.EliminarUsuario(id);
                return NoContent(); // Devuelve un status 204 (sin contenido)
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "Error al eliminar el usuario", error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Credencial) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest(new { mensaje = "Credenciales inválidas" });
            }

            try
            {
                // Llamar al servicio de login
                var result = await _usuarioService.LoginUsuarioAsync(loginDto);

                if (result.Exito == 0)
                {
                    // Si el login falla (Exito == 0)
                    return Unauthorized(new { mensaje = result.Mensaje , exito = result.Exito });
                }

                // Si el login es exitoso (Exito == 1)
                return Ok(new { mensaje = result.Mensaje, exito = result.Exito });
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "Error al intentar iniciar sesión", error = ex.Message });
            }
        }
        [HttpPost("logout")]
        public async Task<ActionResult> Logout([FromBody] LoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Credencial))
            {
                return BadRequest(new { mensaje = "Credencial inválida" });
            }

            try
            {
                // Llamar al servicio para cerrar sesión
                var result = await _usuarioService.CerrarSesionAsync(loginDto.Credencial);

                if (result.Exito == 0)
                {
                    // Si el cierre de sesión falla (Exito == 0)
                    return Unauthorized(new { mensaje = result.Mensaje, exito = result.Exito });
                }

                // Si el cierre de sesión es exitoso (Exito == 1)
                return Ok(new { mensaje = result.Mensaje, exito = result.Exito });
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "Error al intentar cerrar sesión", error = ex.Message });
            }
        }

        [HttpGet("statics")]
        public async Task<ActionResult<DashboardStatsDTO>> GetDashboardStats()
        {
            // Aquí puedes agregar la validación de roles para solo permitir el acceso a administradores
            var stats = await _usuarioService.DashboardStats();
            return Ok(stats);
        }
    }
}
