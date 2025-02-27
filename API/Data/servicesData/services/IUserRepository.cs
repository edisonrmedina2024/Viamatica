using API.Data.servicesData.repositories;
using API.Models;

namespace API.Data.servicesData.services
{
    public interface IUserRepository : IGenericRepository<Usuario>
    {
        Task<LoginResult> LoginUsuarioAsync(string credencial, string password);
        Task<LoginResult> CerrarSesionAsync(string credencial);
        
    }
}
