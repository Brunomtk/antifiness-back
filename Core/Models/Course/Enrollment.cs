using System;
using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Core.Models.Course
{
    public class Enrollment : BaseModel
    {
        [Required]
        public int EmpresasId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? CompletionDate { get; set; }

        [Required]
        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;

        public decimal ProgressPercentage { get; set; } = 0;

        // Navigation properties
        public Empresas? Empresas { get; set; }
        public Course? Course { get; set; }
        public User? User { get; set; }
        public ICollection<Progress> Progress { get; set; } = new List<Progress>();
    }
}
