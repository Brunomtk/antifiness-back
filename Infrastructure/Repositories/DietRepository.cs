using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models.Diet;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class DietRepository : GenericRepository<Diet>
    {
        private readonly DbContextClass _context;

        public DietRepository(DbContextClass context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Diet>> GetAllDetailedAsync()
        {
            return await _context.Set<Diet>()
                .Include(d => d.Client)
                .Include(d => d.Empresa)
                .Include(d => d.Meals).ThenInclude(m => m.Foods).ThenInclude(f => f.Food)
                .Include(d => d.Progress)
                .OrderByDescending(d => d.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Diet?> GetByIdDetailedAsync(int id)
        {
            return await _context.Set<Diet>()
                .Include(d => d.Client)
                .Include(d => d.Empresa)
                .Include(d => d.Meals).ThenInclude(m => m.Foods).ThenInclude(f => f.Food)
                .Include(d => d.Progress)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<Diet>> GetByClientIdAsync(int clientId)
        {
            return await _context.Set<Diet>()
                .Include(d => d.Client)
                .Include(d => d.Empresa)
                .Include(d => d.Meals)
                .Include(d => d.Progress)
                .Where(d => d.ClientId == clientId)
                .OrderByDescending(d => d.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Diet>> GetByEmpresaIdAsync(int empresaId)
        {
            return await _context.Set<Diet>()
                .Include(d => d.Client)
                .Include(d => d.Empresa)
                .Include(d => d.Meals)
                .Include(d => d.Progress)
                .Where(d => d.EmpresaId == empresaId)
                .OrderByDescending(d => d.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Diet?> GetCurrentByClientIdAsync(int clientId)
        {
            return await _context.Set<Diet>() 
                .Where(d => d.ClientId == clientId)
                .OrderByDescending(d => d.CreatedDate)
                .Include(d => d.Client)
                .Include(d => d.Meals)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<Diet?> GetByIdWithMealsAsync(int id)
        {
            return await _context.Set<Diet>()
                .Include(d => d.Meals)
                .ThenInclude(m => m.Foods)
                .ThenInclude(f => f.Food)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);
        }
    
        public async Task<Diet?> GetByIdWithProgressAsync(int id)
        {
            return await _context.Set<Diet>()
                .Include(d => d.Progress)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);
        }

    }
}
