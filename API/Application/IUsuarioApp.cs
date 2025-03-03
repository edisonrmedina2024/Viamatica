using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Application
{
    public interface IUsuarioApp
    {
        Task<List<UsuarioDTO>> ObtenerUsuarios();
        Task<Usuario> ObtenerUsuarioPorId(int id);
        Task<Usuario> AgregarUsuario(CrearUsuarioDto usuario);
        Task<bool> ActualizarUsuario(ActualizarUsuarioDto usuarioDto,string usuername);
        Task<bool> EliminarUsuario(int id);
        Task<LoginResult> LoginUsuarioAsync(LoginDto loginDto);
        Task<LoginResult> CerrarSesionAsync(string correo);
        Task<DashboardStatsDTO> DashboardStats();
        Task<ProfileDTO> profile(string username);
        Task<bool> recoveryPassword(RecoveryPasswordDTO recoveryPasswordDTO);
    }
}
