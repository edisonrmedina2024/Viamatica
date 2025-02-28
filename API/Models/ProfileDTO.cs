namespace API.Models
{
    public class ProfileDTO
    {
        public string usuario { get; set; }
        public string email { get; set; }
        public int intentoFallidos { get; set; }
        public string ultimaSession { get; set; }
        public string ultimaSessionInicio { get; set; }
        public string ultimaSessionFin { get; set; }

    }
}
