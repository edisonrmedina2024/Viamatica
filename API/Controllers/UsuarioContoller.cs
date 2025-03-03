using API.Application;
using API.Models;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
        [Authorize]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> GetUsuarios()
        {
            return await _usuarioService.ObtenerUsuarios();
        }

        [HttpPost]
        public async Task<ActionResult<Usuario>> CreateUsuario(CrearUsuarioDto usuario)
        {
            return await _usuarioService.AgregarUsuario(usuario);
        }
        
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

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<bool>> UpdateUsuario([FromBody] ActualizarUsuarioDto usuario , string usuername)
        {
            if (usuario == null)
            {
                return BadRequest(new { mensaje = "Los datos del usuario son inválidos" });
            }

            try
            {
                await _usuarioService.ActualizarUsuario(usuario, usuername);
                return await _usuarioService.ActualizarUsuario(usuario, usuername);
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "Error al actualizar el usuario", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<bool>> DeleteUsuario(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { mensaje = "ID de usuario inválido" });
            }

            try
            {
                return await _usuarioService.EliminarUsuario(id);
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
                    return BadRequest(new { mensaje = result.Mensaje , exito = result.Exito , token =result.JWT});
                }

                // Si el login es exitoso (Exito == 1)
                return Ok(new { mensaje = result.Mensaje, exito = result.Exito, token = result.JWT });
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
                    return BadRequest(new { mensaje = result.Mensaje, exito = result.Exito });
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
        [Authorize]
        public async Task<ActionResult<DashboardStatsDTO>> GetDashboardStats()
        {
            // Aquí puedes agregar la validación de roles para solo permitir el acceso a administradores
            var stats = await _usuarioService.DashboardStats();
            return Ok(stats);
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<ProfileDTO>> profile(string username)
        {
            var profileInfo = await _usuarioService.profile(username);
            return Ok(profileInfo);
        }

        [HttpPost("recovery-password")]
        [Authorize]
        public async Task<ActionResult<bool>> recoveryPassword(RecoveryPasswordDTO recoveryPasswordDTO)
        {
            
            if (string.IsNullOrEmpty(recoveryPasswordDTO.Username) || string.IsNullOrEmpty(recoveryPasswordDTO.NewPassword))
            {
                return BadRequest("Faltan parámetros.");
            }

            var result = await _usuarioService.recoveryPassword(recoveryPasswordDTO);

            if (!result)
            {
                return NotFound("No se pudo cambiar la contraseña");
            }

            return Ok(true);
        }


    }
}
