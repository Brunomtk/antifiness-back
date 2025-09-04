using Core.Enums;

namespace Core.DTO.Workout
{
    public class WorkoutProgressResponse
    {
        public int Id { get; set; }
        public int WorkoutId { get; set; }
        public string WorkoutName { get; set; } = string.Empty;
        public int ClientId { get; set; }
        public DateTime Date { get; set; }
        public int? ActualDuration { get; set; }
        public int? ActualCalories { get; set; }
        public int? Rating { get; set; }
        public WorkoutMood? Mood { get; set; }
        public int? EnergyLevel { get; set; }
        public bool IsCompleted { get; set; }
        public List<ExerciseProgressResponse>? ExerciseProgress { get; set; }
        public string? Notes { get; set; }
        public bool HasPersonalRecord { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ExerciseProgressResponse
    {
        public int ExerciseId { get; set; }
        public string ExerciseName { get; set; } = string.Empty;
        public int? CompletedSets { get; set; }
        public int? CompletedReps { get; set; }
        public decimal? Weight { get; set; }
        public bool IsCompleted { get; set; }
    }
}
