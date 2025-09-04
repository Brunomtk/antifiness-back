using Core.Enums;

namespace Core.DTO.Workout.Workout
{
    public class WorkoutFiltersDTO
    {
        public List<WorkoutType>? Type { get; set; }
        public List<WorkoutDifficulty>? Difficulty { get; set; }
        public List<WorkoutStatus>? Status { get; set; }
        public List<string>? Tags { get; set; }
        public string? Search { get; set; }
        public DateTime? DateRangeStart { get; set; }
        public DateTime? DateRangeEnd { get; set; }
        public int? ClientId { get; set; }
        public int? EmpresaId { get; set; }
        public bool? IsTemplate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "CreatedAt";
        public string SortOrder { get; set; } = "desc";
    }
}
