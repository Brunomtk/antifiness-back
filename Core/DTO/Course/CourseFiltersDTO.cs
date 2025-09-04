using Core.Enums;

namespace Core.DTO.Course
{
    public class CourseFiltersDTO
    {
        public string? Search { get; set; }
        public CourseCategory? Category { get; set; }
        public CourseLevel? Level { get; set; }
        public CourseStatus? Status { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? EmpresasId { get; set; }
        public string? Instructor { get; set; }
        public List<string>? Tags { get; set; }
    }
}
