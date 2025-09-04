using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Core.DTO.Workout
{
    public class UpdateWorkoutRequest
    {
        [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string? Name { get; set; }

        [StringLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
        public string? Description { get; set; }

        public WorkoutType? Type { get; set; }

        public WorkoutDifficulty? Difficulty { get; set; }

        public int? EstimatedDuration { get; set; }

        public int? EstimatedCalories { get; set; }

        public List<string>? Tags { get; set; }

        public bool? IsTemplate { get; set; }

        public string? Notes { get; set; }

        public int? ClientId { get; set; }

        public List<UpdateWorkoutExerciseRequest>? Exercises { get; set; }
    }

    public class UpdateWorkoutExerciseRequest
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "ExerciseId é obrigatório")]
        public int ExerciseId { get; set; }

        [Required(ErrorMessage = "Order é obrigatório")]
        public int Order { get; set; }

        public int? Sets { get; set; }

        public int? Reps { get; set; }

        public decimal? Weight { get; set; }

        public int? RestTime { get; set; }

        public string? Notes { get; set; }
    }
}
