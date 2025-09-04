using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Models.Course
{
    public class Progress : BaseModel
    {
        [Required]
        public int EnrollmentId { get; set; }

        [Required]
        public int LessonId { get; set; }

        [Required]
        public bool IsCompleted { get; set; } = false;

        public DateTime? CompletedDate { get; set; }

        public int WatchTimeMinutes { get; set; } = 0;

        // Navigation properties
        public Enrollment? Enrollment { get; set; }
        public Lesson? Lesson { get; set; }
    }
}
