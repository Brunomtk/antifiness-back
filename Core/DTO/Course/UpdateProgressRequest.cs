using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Course
{
    public class UpdateProgressRequest
    {
        [Required]
        public int EnrollmentId { get; set; }

        [Required]
        public int LessonId { get; set; }

        [Required]
        public bool IsCompleted { get; set; }

        public int WatchTimeMinutes { get; set; } = 0;
    }
}
