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
        Task<ExerciseResponse?> GetExerciseByIdAsync(int id);
        Task<ExerciseResponse> CreateExerciseAsync(CreateExerciseRequest request);
        Task<ExerciseResponse?> UpdateExerciseAsync(int id, UpdateExerciseRequest request);
        Task<bool> DeleteExerciseAsync(int id);

        // Workout methods
        Task<WorkoutsPagedDTO> GetWorkoutsAsync(WorkoutFiltersDTO filters);
        Task<WorkoutResponse?> GetWorkoutByIdAsync(int id);
        Task<WorkoutResponse> CreateWorkoutAsync(CreateWorkoutRequest request);
        Task<WorkoutResponse?> UpdateWorkoutAsync(int id, UpdateWorkoutRequest request);
        Task<bool> DeleteWorkoutAsync(int id);
        Task<bool> ChangeWorkoutStatusAsync(int id, ChangeWorkoutStatusRequest request);
        Task<WorkoutsPagedDTO> GetTemplatesAsync(WorkoutFiltersDTO filters);
        Task<WorkoutResponse?> InstantiateTemplateAsync(int templateId, CreateWorkoutRequest? overrides = null);

        // Progress methods
        Task<List<WorkoutProgressResponse>> GetWorkoutProgressAsync(int workoutId);
        Task<WorkoutProgressResponse> CreateWorkoutProgressAsync(int workoutId, CreateWorkoutProgressRequest request);

        // Stats methods
        Task<WorkoutStatsDTO> GetWorkoutStatsAsync(int? empresaId = null, int? clientId = null);
    }
}
