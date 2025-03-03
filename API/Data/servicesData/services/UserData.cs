using API.Data;
using API.Data.servicesData.repositories;
using API.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace API.Data.servicesData.services
{
    public class UserData : BaseRepository<Usuario>, IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;


        public UserData(ApplicationDbContext context, IConfiguration configuration) : base(context)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<LoginResult> LoginUsuarioAsync(string credencial, string password)
        {

            password = BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password))).Replace("-", "").ToLower();

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
                            if (Convert.ToInt32(reader["Exito"]) == 1)
                            {
                                var tokenHandler = new JwtSecurityTokenHandler();
                                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
                                var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"]);

                                var tokenDescriptor = new SecurityTokenDescriptor
                                {
                                    Subject = new ClaimsIdentity(new[]
                                    {
                                        new Claim(ClaimTypes.Name, credencial)
                                    }),
                                    Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
                                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                                    Issuer = _configuration["Jwt:Issuer"],
                                    Audience = _configuration["Jwt:Audience"]
                                };

                                var token = tokenHandler.CreateToken(tokenDescriptor);
                                string userToken = tokenHandler.WriteToken(token);

                                return new LoginResult
                                {
                                    Mensaje = reader["Mensaje"].ToString(),
                                    Exito = Convert.ToInt32(reader["Exito"]),
                                    JWT = userToken
                                };
                            }
                            else
                            {
                                return new LoginResult
                                {
                                    Mensaje = reader["Mensaje"].ToString(),
                                    Exito = Convert.ToInt32(reader["Exito"]),
                                    JWT = ""
                                };

                            }

                            
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
