using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Core.DTO.Course
{
    public class CreateCourseRequest
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Thumbnail { get; set; }

        [Required]
        [MaxLength(100)]
        public string Instructor { get; set; } = string.Empty;

        [Required]
        public CourseCategory Category { get; set; }

        [Required]
        public CourseLevel Level { get; set; }

        public int DurationMinutes { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [MaxLength(3)]
        public string Currency { get; set; } = "USD";

        public List<string>? Tags { get; set; }

        [Required]
        public int EmpresasId { get; set; }
    }
}
