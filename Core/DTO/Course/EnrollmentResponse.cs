using Core.Enums;

namespace Core.DTO.Course
{
    public class EnrollmentResponse
    {
        public int Id { get; set; }
        public int EmpresasId { get; set; }
        public string? EmpresasName { get; set; }
        public int CourseId { get; set; }
        public string? CourseTitle { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public EnrollmentStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public decimal ProgressPercentage { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int CompletedLessons { get; set; }
        public int TotalLessons { get; set; }
    }
}
