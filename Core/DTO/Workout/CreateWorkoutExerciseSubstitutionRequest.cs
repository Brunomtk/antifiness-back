using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Workout
{
    public class CreateWorkoutExerciseSubstitutionRequest
    {
        [Required]
        public int ExerciseId { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
