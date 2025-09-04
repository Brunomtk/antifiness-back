using Core.Enums;

namespace Core.DTO.Workout.Exercise
{
    public class ExerciseResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Instructions { get; set; }
        public List<string> MuscleGroups { get; set; } = new();
        public List<string> Equipment { get; set; } = new();
        public ExerciseDifficulty Difficulty { get; set; }
        public ExerciseCategory Category { get; set; }
        public string? Tips { get; set; }
        public string? Variations { get; set; }
        public List<string>? MediaUrls { get; set; }
        public bool IsActive { get; set; }
        public int EmpresaId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
