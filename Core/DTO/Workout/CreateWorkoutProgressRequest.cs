using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Core.DTO.Workout
{
    public class CreateWorkoutProgressRequest
    {
        [Required(ErrorMessage = "ClientId é obrigatório")]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Date é obrigatória")]
        public DateTime Date { get; set; }

        public int? ActualDuration { get; set; }

        public int? ActualCalories { get; set; }

        [Range(1, 5, ErrorMessage = "Rating deve estar entre 1 e 5")]
        public int? Rating { get; set; }

        public WorkoutMood? Mood { get; set; }

        [Range(1, 5, ErrorMessage = "EnergyLevel deve estar entre 1 e 5")]
        public int? EnergyLevel { get; set; }

        public bool IsCompleted { get; set; } = false;

        public List<ExerciseProgressRequest>? ExerciseProgress { get; set; }

        [StringLength(1000, ErrorMessage = "Notes devem ter no máximo 1000 caracteres")]
        public string? Notes { get; set; }
    }

    public class ExerciseProgressRequest
    {
        public int ExerciseId { get; set; }
        public int? CompletedSets { get; set; }
        public int? CompletedReps { get; set; }
        public decimal? Weight { get; set; }
        public bool IsCompleted { get; set; }
    }
}
