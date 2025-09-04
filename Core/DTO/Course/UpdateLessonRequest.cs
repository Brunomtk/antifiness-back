using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Course
{
    public class UpdateLessonRequest
    {
        [MaxLength(200)]
        public string? Title { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public string? Content { get; set; }

        public int? DurationMinutes { get; set; }

        public int? Order { get; set; }

        public List<string>? Resources { get; set; }

        [MaxLength(500)]
        public string? VideoUrl { get; set; }
    }
}
