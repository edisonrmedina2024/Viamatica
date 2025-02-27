using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity.Infrastructure;

namespace API.Data.servicesData.repositories
{
    public class BaseRepository<T> : IGenericRepository<T> where T : class
    {

        private readonly ApplicationDbContext _context;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity == null)
            {
                return false;
            }

            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            if (typeof(T) == typeof(Usuario))
            {
                return await _context.Set<Usuario>()
                    .Include(u => u.IdRols) // 🔹 Incluye los roles del usuario
                    .Include(u => u.Sessions) // 🔹 Incluye las sesiones del usuario
                    .FirstOrDefaultAsync(u => u.IdUsuario == id) as T;
            }
            else if (typeof(T) == typeof(Role))
            {
                return await _context.Set<Role>()
                    .Include(r => r.RolOpcionesIdOpcions) // 🔹 Incluye las opciones del rol
                    .FirstOrDefaultAsync(r => r.IdRol == id) as T;
            }

            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<List<T>> GetPagedReponseAsync(int page, int size)
        {
            return await _context.Set<T>().Skip((page - 1) * size).Take(size).AsNoTracking().ToListAsync();

        }

        public async Task<List<T>> ListAllAsync()
        {
            if (typeof(T) == typeof(Usuario))
            {
                return await _context.Set<Usuario>()
                    .Include(u => u.IdRols) // 🔹 Incluye los roles del usuario
                    .Include(u => u.Sessions) // 🔹 Incluye las sesiones del usuario
                    .ToListAsync() as List<T>;
            }
            else if(typeof(T) == typeof(Role))
            {

                var sample = await _context.Set<Role>()
                    .Include(r => r.RolOpcionesIdOpcions) // 🔹 Incluye las opciones del rol
                    .ToListAsync() as List<T>;

                return await _context.Set<Role>()
                    .Include(r => r.RolOpcionesIdOpcions) // 🔹 Incluye las opciones del rol
                    .ToListAsync() as List<T>;
            }

            return await _context.Set<T>().ToListAsync();
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            try
            {
                _context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                // Puedes loggear el error para obtener más información
                Console.WriteLine($"Error al actualizar: {ex.Message}");
                Console.WriteLine($"Detalles internos: {ex.InnerException?.Message}");
                return false;
            }
        }

    }
}

