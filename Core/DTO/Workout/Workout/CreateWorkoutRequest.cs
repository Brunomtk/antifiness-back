using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Core.DTO.Workout.Workout
{
    public class CreateWorkoutRequest
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Tipo é obrigatório")]
        public WorkoutType Type { get; set; }

        [Required(ErrorMessage = "Dificuldade é obrigatória")]
        public WorkoutDifficulty Difficulty { get; set; }

        public int? EstimatedDuration { get; set; }

        public int? EstimatedCalories { get; set; }

        public List<string>? Tags { get; set; }

        public bool IsTemplate { get; set; } = false;

        public string? Notes { get; set; }

        [Required(ErrorMessage = "EmpresaId é obrigatório")]
        public int EmpresaId { get; set; }

        public int? ClientId { get; set; }

        public List<CreateWorkoutExerciseRequest> Exercises { get; set; } = new();
    }

    public class CreateWorkoutExerciseRequest
    {
        [Required(ErrorMessage = "ExerciseId é obrigatório")]
        public int ExerciseId { get; set; }

        [Required(ErrorMessage = "Ordem é obrigatória")]
        public int Order { get; set; }

        public int? Sets { get; set; }

        public int? Reps { get; set; }

        public decimal? Weight { get; set; }

        public int? RestTime { get; set; }

        public string? Notes { get; set; }
    }
}
