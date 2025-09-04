using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models.Diet;
using Infrastructure;                 // DbContextClass
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class FoodRepository : GenericRepository<Food>
    {
        private readonly DbContextClass _context;

        public FoodRepository(DbContextClass context) : base(context)
        {
            _context = context;
        }

        // Retorna apenas alimentos ativos (sem override)
        public async Task<IEnumerable<Food>> GetAllActiveAsync()
        {
            return await _context.Set<Food>()
                .Where(f => f.IsActive)
                .OrderBy(f => f.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Food>> GetByCategoryAsync(Core.Enums.FoodCategory category)
        {
            return await _context.Set<Food>()
                .Where(f => f.Category == category && f.IsActive)
                .OrderBy(f => f.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Food>> SearchByNameAsync(string name)
        {
            return await _context.Set<Food>()
                .Where(f => f.Name.Contains(name) && f.IsActive)
                .OrderBy(f => f.Name)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
