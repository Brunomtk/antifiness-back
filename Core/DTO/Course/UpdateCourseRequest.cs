using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Core.DTO.Course
{
    public class UpdateCourseRequest
    {
        [MaxLength(200)]
        public string? Title { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? Thumbnail { get; set; }

        [MaxLength(100)]
        public string? Instructor { get; set; }

        public CourseCategory? Category { get; set; }

        public CourseLevel? Level { get; set; }

        public int? DurationMinutes { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Price { get; set; }

        [MaxLength(3)]
        public string? Currency { get; set; }

        public List<string>? Tags { get; set; }

        public CourseStatus? Status { get; set; }
    }
}
