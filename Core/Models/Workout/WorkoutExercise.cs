using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Core.Models.Workout
{
    public class WorkoutExercise : BaseModel
    {
        [Required]
        public int WorkoutId { get; set; }

        [Required]
        public int ExerciseId { get; set; }

        [Required]
        public int Order { get; set; }

        public int? Sets { get; set; }

        public int? Reps { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Weight { get; set; }

        public int? RestTime { get; set; } // seconds

        [StringLength(500)]
        public string? Notes { get; set; }

        public bool IsCompleted { get; set; } = false;

        public int? CompletedSets { get; set; }

        // Navigation properties
        [ForeignKey("WorkoutId")]
        public virtual Workout Workout { get; set; } = null!;

        [ForeignKey("ExerciseId")]
        public virtual Exercise Exercise { get; set; } = null!;

        public virtual ICollection<WorkoutExerciseSubstitution> Substitutions { get; set; } = new List<WorkoutExerciseSubstitution>();
    }
}
