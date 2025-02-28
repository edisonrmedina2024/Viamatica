namespace API.Models
{
    public class LoginResult
    {
        public string Mensaje { get; set; }
        public int Exito { get; set; }
        public string? JWT { get; set; }
    }
}
