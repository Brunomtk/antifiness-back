namespace Core.DTO.Course
{
    public class ProgressResponse
    {
        public int Id { get; set; }
        public int EnrollmentId { get; set; }
        public int LessonId { get; set; }
        public string? LessonTitle { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int WatchTimeMinutes { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
