namespace API.Models
{
    public class LoginDto
    {
        public string Credencial { get; set; }  // Puede ser correo o nombre de usuario
        public string? Password { get; set; }    // Contraseña del usuario
    }
}
