using Core.Enums;

namespace Core.DTO.Workout.Workout
{
    public class WorkoutResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public WorkoutType Type { get; set; }
        public WorkoutDifficulty Difficulty { get; set; }
        public WorkoutStatus Status { get; set; }
        public int? EstimatedDuration { get; set; }
        public int? EstimatedCalories { get; set; }
        public List<string>? Tags { get; set; }
        public bool IsTemplate { get; set; }
        public string? Notes { get; set; }
        public int EmpresaId { get; set; }
        public int? ClientId { get; set; }
        public List<WorkoutExerciseResponse> Exercises { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class WorkoutExerciseResponse
    {
        public int Id { get; set; }
        public int ExerciseId { get; set; }
        public string ExerciseName { get; set; } = string.Empty;
        public int Order { get; set; }
        public int? Sets { get; set; }
        public int? Reps { get; set; }
        public decimal? Weight { get; set; }
        public int? RestTime { get; set; }
        public string? Notes { get; set; }
        public bool IsCompleted { get; set; }
        public int? CompletedSets { get; set; }
    }
}
