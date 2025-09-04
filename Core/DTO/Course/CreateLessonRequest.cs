using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Course
{
    public class CreateLessonRequest
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public int DurationMinutes { get; set; }

        [Required]
        public int Order { get; set; }

        public List<string>? Resources { get; set; }

        [MaxLength(500)]
        public string? VideoUrl { get; set; }
    }
}
