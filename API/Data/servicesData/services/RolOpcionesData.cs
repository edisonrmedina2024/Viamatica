using API.Data.servicesData.repositories;
using API.Models;

namespace API.Data.servicesData.services
{
    public class RolOpcionesData : BaseRepository<RolOpcione>, IRolOpcionesRepository
    {
        private readonly ApplicationDbContext _context;

        public RolOpcionesData(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
