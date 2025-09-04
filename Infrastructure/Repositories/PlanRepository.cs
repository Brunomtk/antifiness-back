// File: Infrastructure/Repositories/PlanRepositories.cs
using Core.Models.Plan;
using Infrastructure;                           // DbContextClass
using Infrastructure.ServiceExtension;          // GetPagedAsync, PagedResult<T>
using Microsoft.EntityFrameworkCore;
using Saller.Infrastructure.ServiceExtension;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IPlanRepository : IGenericRepository<Plan>
    {
        Task<Plan?> GetByIdAsync(int id);
        Task<PagedResult<Plan>> GetAllPagedAsync(int page, int pageSize);
    }

    public interface IPlanGoalRepository : IGenericRepository<PlanGoal>
    {
        Task<PlanGoal?> GetByIdAsync(int id);
    }

    public interface IPlanMealRepository : IGenericRepository<PlanMeal>
    {
        Task<PlanMeal?> GetByIdAsync(int id);
    }

    public interface IPlanFoodRepository : IGenericRepository<PlanFood>
    {
        Task<PlanFood?> GetByIdAsync(int id);
    }

    public interface IPlanProgressRepository : IGenericRepository<PlanProgress>
    {
        Task<PlanProgress?> GetByIdAsync(int id);
    }

    public sealed class PlanRepository : GenericRepository<Plan>, IPlanRepository
    {
        private readonly DbContextClass _db;
        public PlanRepository(DbContextClass db) : base(db) => _db = db;

        public async Task<Plan?> GetByIdAsync(int id)
            => await _db.Plans
                        .Include(p => p.Goals)
                        .Include(p => p.Meals)
                            .ThenInclude(m => m.Foods)
                        .Include(p => p.ProgressEntries)
                        .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<PagedResult<Plan>> GetAllPagedAsync(int page, int pageSize)
        {
            var query = _db.Plans
                           .Include(p => p.Goals)
                           .Include(p => p.Meals)
                               .ThenInclude(m => m.Foods)
                           .Include(p => p.ProgressEntries)
                           .AsQueryable();

            return await query.GetPagedAsync(page, pageSize);
        }
    }

    public sealed class PlanGoalRepository : GenericRepository<PlanGoal>, IPlanGoalRepository
    {
        private readonly DbContextClass _db;
        public PlanGoalRepository(DbContextClass db) : base(db) => _db = db;

        public async Task<PlanGoal?> GetByIdAsync(int id)
            => await _db.PlanGoals
                        .FirstOrDefaultAsync(g => g.Id == id);
    }

    public sealed class PlanMealRepository : GenericRepository<PlanMeal>, IPlanMealRepository
    {
        private readonly DbContextClass _db;
        public PlanMealRepository(DbContextClass db) : base(db) => _db = db;

        public async Task<PlanMeal?> GetByIdAsync(int id)
            => await _db.PlanMeals
                        .Include(m => m.Foods)
                        .FirstOrDefaultAsync(m => m.Id == id);
    }

    public sealed class PlanFoodRepository : GenericRepository<PlanFood>, IPlanFoodRepository
    {
        private readonly DbContextClass _db;
        public PlanFoodRepository(DbContextClass db) : base(db) => _db = db;

        public async Task<PlanFood?> GetByIdAsync(int id)
            => await _db.PlanFoods
                        .FirstOrDefaultAsync(f => f.Id == id);
    }

    public sealed class PlanProgressRepository : GenericRepository<PlanProgress>, IPlanProgressRepository
    {
        private readonly DbContextClass _db;
        public PlanProgressRepository(DbContextClass db) : base(db) => _db = db;

        public async Task<PlanProgress?> GetByIdAsync(int id)
            => await _db.PlanProgress
                        .FirstOrDefaultAsync(p => p.Id == id);
    }
}
