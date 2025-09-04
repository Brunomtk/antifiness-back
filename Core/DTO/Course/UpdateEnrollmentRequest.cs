using Core.Enums;

namespace Core.DTO.Course
{
    public class UpdateEnrollmentRequest
    {
        public EnrollmentStatus? Status { get; set; }
        public DateTime? CompletionDate { get; set; }
        public decimal? ProgressPercentage { get; set; }
    }
}
