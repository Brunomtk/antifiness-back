namespace Core.DTO.Course
{
    public class ReviewResponse
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string? CourseTitle { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime ReviewDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
