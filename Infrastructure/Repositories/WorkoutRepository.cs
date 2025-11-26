using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models.Workout;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class WorkoutRepository : GenericRepository<Workout>
    {
        private readonly DbContextClass _context;

        public WorkoutRepository(DbContextClass context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Treino atual do cliente (mais recente, ignorando templates)
        /// </summary>
        public async Task<Workout?> GetCurrentByClientIdAsync(int clientId)
        {
            return await _context.Set<Core.Models.Workout.Workout>()
                .Where(w => w.ClientId == clientId && !w.IsTemplate)
                .OrderByDescending(w => w.CreatedDate)
                .Include(w => w.WorkoutExercises)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Histórico de treinos do cliente (mais recentes primeiro), ignorando templates
        /// </summary>
        public async Task<IEnumerable<Workout>> GetByClientIdAsync(int clientId)
        {
            return await _context.Set<Core.Models.Workout.Workout>()
                .Where(w => w.ClientId == clientId && !w.IsTemplate)
                .Include(w => w.WorkoutExercises)
                .OrderByDescending(w => w.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }
        /// <summary>
        /// Retorna um treino com seus exercícios carregados
        /// </summary>
        public async Task<Workout?> GetWorkoutWithExercisesAsync(int id)
        {
            return await _context.Set<Workout>()
                .Include(w => w.WorkoutExercises)
                    .ThenInclude(we => we.Exercise)
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == id);
        }


        /// <summary>
        /// Retorna um treino com seus exercícios (sem carregar a entidade Exercise) para atualização (rastreamento ON)
        /// </summary>
        public async Task<Workout?> GetWorkoutWithExercisesForUpdateAsync(int id)
        {
            return await _context.Set<Workout>()
                .Include(w => w.WorkoutExercises)
                    .ThenInclude(we => we.Substitutions)
                .FirstOrDefaultAsync(w => w.Id == id);
        }


        public async Task<WorkoutExercise?> GetWorkoutExerciseWithSubstitutionsForUpdateAsync(int workoutId, int workoutExerciseId)
        {
            return await _context.Set<WorkoutExercise>()
                .Include(we => we.Substitutions)
                    .ThenInclude(s => s.Exercise)
                .Include(we => we.Exercise)
                .Where(we => we.WorkoutId == workoutId && we.Id == workoutExerciseId)
                .FirstOrDefaultAsync();
        }

    }
}