using API.Models;

namespace API.Application
{
    public interface IUsuarioApp
    {
        Task<List<UsuarioDTO>> ObtenerUsuarios();
        Task<Usuario> ObtenerUsuarioPorId(int id);
        Task<Usuario> AgregarUsuario(CrearUsuarioDto usuario);
        Task ActualizarUsuario(Usuario usuario);
        Task EliminarUsuario(int id);
        Task<LoginResult> LoginUsuarioAsync(LoginDto loginDto);
        Task<LoginResult> CerrarSesionAsync(string correo);
        Task<DashboardStatsDTO> DashboardStats();

    }
}
