using API.Data.servicesData.repositories;
using API.Models;

namespace API.Data.servicesData.services
{
    public class PersonData : BaseRepository<Persona>, IPersonaRepository
    {
        private readonly ApplicationDbContext _context;

        public PersonData(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
