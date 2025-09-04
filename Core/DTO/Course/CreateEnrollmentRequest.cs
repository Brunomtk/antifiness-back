using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Course
{
    public class CreateEnrollmentRequest
    {
        [Required]
        public int EmpresasId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public int UserId { get; set; }

        public DateTime? StartDate { get; set; }
    }
}
