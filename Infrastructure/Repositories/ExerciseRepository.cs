using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models.Workout;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ExerciseRepository : GenericRepository<Exercise>
    {
        public ExerciseRepository(DbContextClass context) : base(context)
        {
        }

        // Atalho para o DbSet<Exercise>
        private DbSet<Exercise> Exercises => _dbContext.Set<Exercise>();

        public async Task<Exercise?> GetByIdWithDetailsAsync(int id)
        {
            return await Exercises
                // .Include(e => e.Relacionamentos) // se precisar carregar algo junto
                .FirstOrDefaultAsync(e => e.Id == id && e.IsActive);
        }

        public async Task<List<Exercise>> GetActiveExercisesAsync()
        {
            return await Exercises
                .Where(e => e.IsActive)
                .OrderBy(e => e.Name)
                .ToListAsync();
        }

        public Task<bool> ExistsAsync(int id)
        {
            return Exercises.AnyAsync(e => e.Id == id && e.IsActive);
        }
    }
}
