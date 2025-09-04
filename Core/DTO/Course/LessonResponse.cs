namespace Core.DTO.Course
{
    public class LessonResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Content { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public int Order { get; set; }
        public List<string> Resources { get; set; } = new();
        public string? VideoUrl { get; set; }
        public int CourseId { get; set; }
        public string? CourseTitle { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedDate { get; set; }
    }
}
