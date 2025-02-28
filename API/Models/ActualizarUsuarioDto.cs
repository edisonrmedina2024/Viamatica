namespace API.Models
{
    public class ActualizarUsuarioDto
    {
        public string UserName { get; set; }
        public string Identificacion { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public DateTime? FechaNacimiento { get; set; }
    }
}
