using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Core.Models.Course
{
    public class Course : BaseModel
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
        public decimal Price { get; set; }

        [MaxLength(3)]
        public string Currency { get; set; } = "USD";

        public string? Tags { get; set; } // JSON array

        [Required]
        public int EmpresasId { get; set; }

        [Required]
        public CourseStatus Status { get; set; } = CourseStatus.Draft;

        public DateTime? PublishedDate { get; set; }

        // Navigation properties
        public Empresas? Empresas { get; set; }
        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
