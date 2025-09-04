using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Core.DTO.Workout
{
    public class CreateExerciseRequest
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
        public string? Description { get; set; }

        [StringLength(2000, ErrorMessage = "Instruções devem ter no máximo 2000 caracteres")]
        public string? Instructions { get; set; }

        [Required(ErrorMessage = "Grupos musculares são obrigatórios")]
        public List<string> MuscleGroups { get; set; } = new();

        [Required(ErrorMessage = "Equipamentos são obrigatórios")]
        public List<string> Equipment { get; set; } = new();

        [Required(ErrorMessage = "Dificuldade é obrigatória")]
        public ExerciseDifficulty Difficulty { get; set; }

        [Required(ErrorMessage = "Categoria é obrigatória")]
        public ExerciseCategory Category { get; set; }

        [StringLength(1000, ErrorMessage = "Dicas devem ter no máximo 1000 caracteres")]
        public string? Tips { get; set; }

        [StringLength(1000, ErrorMessage = "Variações devem ter no máximo 1000 caracteres")]
        public string? Variations { get; set; }

        public List<string>? MediaUrls { get; set; }

        [Required(ErrorMessage = "EmpresaId é obrigatório")]
        public int EmpresaId { get; set; }
    }
}
