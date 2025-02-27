using API.Data.servicesData.repositories;
using API.Models;

namespace API.Data.servicesData.services
{
    public class RolData : BaseRepository<Role>, IRolRepository
    {
        private readonly ApplicationDbContext _context;

        public RolData(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

    }
}
