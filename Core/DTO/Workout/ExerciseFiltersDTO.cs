using Core.Enums;

namespace Core.DTO.Workout
{
    public class ExerciseFiltersDTO
    {
        public List<string>? MuscleGroups { get; set; }
        public List<string>? Equipment { get; set; }
        public List<ExerciseDifficulty>? Difficulty { get; set; }
        public List<ExerciseCategory>? Category { get; set; }
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
        public int? EmpresaId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "CreatedAt";
        public string SortOrder { get; set; } = "desc";
    }
}
