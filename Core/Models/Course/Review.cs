using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Models.Course
{
    public class Review : BaseModel
    {
        [Required]
        public int CourseId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        [Required]
        public DateTime ReviewDate { get; set; }

        // Navigation properties
        public Course? Course { get; set; }
        public User? User { get; set; }
    }
}
