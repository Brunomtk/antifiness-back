using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Models.Course
{
    public class Lesson : BaseModel
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

        public string? Resources { get; set; } // JSON array

        [MaxLength(500)]
        public string? VideoUrl { get; set; }

        [Required]
        public int CourseId { get; set; }

        // Navigation properties
        public Course? Course { get; set; }
        public ICollection<Progress> Progress { get; set; } = new List<Progress>();
    }
}
