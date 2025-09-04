using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums;

namespace Core.Models.Workout
{
    public class Workout : BaseModel
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public WorkoutType Type { get; set; }

        [Required]
        public WorkoutDifficulty Difficulty { get; set; }

        [Required]
        public WorkoutStatus Status { get; set; } = WorkoutStatus.Draft;

        public int? EstimatedDuration { get; set; } // minutes

        public int? EstimatedCalories { get; set; }

        public string? Tags { get; set; } // JSON array

        public bool IsTemplate { get; set; } = false;

        public string? Notes { get; set; }

        public int EmpresaId { get; set; }

        public int? ClientId { get; set; }

        // Navigation properties
        public virtual ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
        public virtual ICollection<WorkoutProgress> WorkoutProgresses { get; set; } = new List<WorkoutProgress>();
    }
}
