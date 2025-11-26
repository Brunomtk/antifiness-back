using System;

namespace Core.DTO.Workout
{
    public class WorkoutExerciseSubstitutionResponse
    {
        public int Id { get; set; }
        public int WorkoutExerciseId { get; set; }
        public int ExerciseId { get; set; }
        public string ExerciseName { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
