using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Core.DTO.Workout.Exercise
{
    public class UpdateExerciseRequest
    {
        [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string? Name { get; set; }

        [StringLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
        public string? Description { get; set; }

        [StringLength(2000, ErrorMessage = "Instruções devem ter no máximo 2000 caracteres")]
        public string? Instructions { get; set; }

        public List<string>? MuscleGroups { get; set; }

        public List<string>? Equipment { get; set; }

        public ExerciseDifficulty? Difficulty { get; set; }

        public ExerciseCategory? Category { get; set; }

        [StringLength(1000, ErrorMessage = "Dicas devem ter no máximo 1000 caracteres")]
        public string? Tips { get; set; }

        [StringLength(1000, ErrorMessage = "Variações devem ter no máximo 1000 caracteres")]
        public string? Variations { get; set; }

        public List<string>? MediaUrls { get; set; }

        public bool? IsActive { get; set; }
    }
}
