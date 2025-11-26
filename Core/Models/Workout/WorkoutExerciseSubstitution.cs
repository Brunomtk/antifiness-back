using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Workout
{
    public class WorkoutExerciseSubstitution : BaseModel
    {
        [Required]
        public int WorkoutExerciseId { get; set; }

        [Required]
        public int ExerciseId { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [ForeignKey("WorkoutExerciseId")]
        public virtual WorkoutExercise WorkoutExercise { get; set; } = null!;

        [ForeignKey("ExerciseId")]
        public virtual Exercise Exercise { get; set; } = null!;
    }
}
