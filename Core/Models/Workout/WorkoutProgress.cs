using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums;

namespace Core.Models.Workout
{
    public class WorkoutProgress : BaseModel
    {
        [Required]
        public int WorkoutId { get; set; }

        [Required]
        public int ClientId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public int? ActualDuration { get; set; } // minutes

        public int? ActualCalories { get; set; }

        [Range(1, 5)]
        public int? Rating { get; set; }

        public WorkoutMood? Mood { get; set; }

        [Range(1, 5)]
        public int? EnergyLevel { get; set; }

        public bool IsCompleted { get; set; } = false;

        public string? ExerciseProgress { get; set; } // JSON with exercise-specific progress

        [StringLength(1000)]
        public string? Notes { get; set; }

        public bool HasPersonalRecord { get; set; } = false;

        // Navigation properties
        [ForeignKey("WorkoutId")]
        public virtual Workout Workout { get; set; } = null!;
    }
}
