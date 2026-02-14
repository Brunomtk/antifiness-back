using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DTO.Workout;

namespace Services
{
    public interface IWorkoutService
    {
        // Exercise methods
        Task<ExercisesPagedDTO> GetExercisesAsync(ExerciseFiltersDTO filters);
        Task<ExerciseResponse?> GetExerciseByIdAsync(int id, int? empresaId = null);
        Task<ExerciseResponse> CreateExerciseAsync(CreateExerciseRequest request);
        Task<ExerciseResponse?> UpdateExerciseAsync(int id, UpdateExerciseRequest request);
        Task<bool> DeleteExerciseAsync(int id);

        // Admin catalog ops
        Task<List<ExerciseResponse>> GetAllExercisesAdminAsync(int? empresaId = null, bool includeInactive = false);
        Task<int> CopyExercisesToEmpresaAsync(int sourceEmpresaId, int targetEmpresaId, bool overwrite = false, bool includeInactive = false);

        // Workout methods
        Task<WorkoutsPagedDTO> GetWorkoutsAsync(WorkoutFiltersDTO filters);
        Task<WorkoutResponse?> GetWorkoutByIdAsync(int id, int? empresaId = null);
        Task<WorkoutResponse> CreateWorkoutAsync(CreateWorkoutRequest request);
        Task<WorkoutResponse?> UpdateWorkoutAsync(int id, UpdateWorkoutRequest request);
        Task<bool> DeleteWorkoutAsync(int id);
        Task<bool> ChangeWorkoutStatusAsync(int id, ChangeWorkoutStatusRequest request);
        Task<WorkoutsPagedDTO> GetTemplatesAsync(WorkoutFiltersDTO filters);
        Task<WorkoutResponse?> InstantiateTemplateAsync(int templateId, CreateWorkoutRequest? overrides = null);

        
        // Exercise substitutions
        Task<List<WorkoutExerciseSubstitutionResponse>> GetWorkoutExerciseSubstitutionsAsync(int workoutId, int workoutExerciseId);
        Task<WorkoutExerciseSubstitutionResponse> CreateWorkoutExerciseSubstitutionAsync(int workoutId, int workoutExerciseId, CreateWorkoutExerciseSubstitutionRequest request);
        Task<WorkoutExerciseSubstitutionResponse?> UpdateWorkoutExerciseSubstitutionAsync(int workoutId, int workoutExerciseId, int substitutionId, UpdateWorkoutExerciseSubstitutionRequest request);
        Task<bool> DeleteWorkoutExerciseSubstitutionAsync(int workoutId, int workoutExerciseId, int substitutionId);

// Progress methods
        Task<List<WorkoutProgressResponse>> GetWorkoutProgressAsync(int workoutId);
        Task<WorkoutProgressResponse> CreateWorkoutProgressAsync(int workoutId, CreateWorkoutProgressRequest request);

        // Stats methods
        Task<WorkoutStatsDTO> GetWorkoutStatsAsync(int? empresaId = null, int? clientId = null);
    }
}
