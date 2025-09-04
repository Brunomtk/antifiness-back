using Core.Enums;

namespace Core.DTO.Course
{
    public class CourseResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Thumbnail { get; set; }
        public string Instructor { get; set; } = string.Empty;
        public CourseCategory Category { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public CourseLevel Level { get; set; }
        public string LevelName { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
        public int EmpresasId { get; set; }
        public string? EmpresasName { get; set; }
        public CourseStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public DateTime? PublishedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int LessonsCount { get; set; }
        public int EnrollmentsCount { get; set; }
        public decimal AverageRating { get; set; }
        public int ReviewsCount { get; set; }
    }
}
