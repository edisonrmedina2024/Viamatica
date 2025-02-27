using API.Data;
using API.Data.servicesData.repositories;
using API.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.SqlClient;

namespace API.Data.servicesData.services
{
    public class UserData : BaseRepository<Usuario>, IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserData(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<LoginResult> LoginUsuarioAsync(string credencial, string password)
        {
            using (var connection = _context.Database.GetDbConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_LoginUsuario";
                    command.CommandType = CommandType.StoredProcedure;

                    // ✅ Usa Microsoft.Data.SqlClient.SqlParameter en lugar de System.Data.SqlClient.SqlParameter
                    var credencialParam = new SqlParameter("@Credencial", SqlDbType.NVarChar, 100) { Value = credencial };
                    var passwordParam = new SqlParameter("@Password", SqlDbType.NVarChar, 100) { Value = password };

                    command.Parameters.Add(credencialParam);
                    command.Parameters.Add(passwordParam);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new LoginResult
                            {
                                Mensaje = reader["Mensaje"].ToString(),
                                Exito = Convert.ToInt32(reader["Exito"])
                            };
                        }
                    }
                }
            }

            return new LoginResult { Mensaje = "Error al procesar la solicitud", Exito = 0 };
        }

        public async Task<LoginResult> CerrarSesionAsync(string credencial)
        {
            using (var connection = _context.Database.GetDbConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_CerrarSesion"; // Llamada al procedimiento almacenado para cerrar sesión
                    command.CommandType = CommandType.StoredProcedure;

                    // Parámetro de entrada: credencial (correo o nombre de usuario)
                    var credencialParam = new SqlParameter("@Credencial", SqlDbType.NVarChar, 100) { Value = credencial };
                    command.Parameters.Add(credencialParam);

                    // Ejecutar el procedimiento almacenado
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new LoginResult
                            {
                                Mensaje = reader["Mensaje"].ToString(),
                                Exito = Convert.ToInt32(reader["Exito"])
                            };
                        }
                    }
                }
            }

            return new LoginResult { Mensaje = "Error al procesar la solicitud", Exito = 0 };
        }

       
    }
}
