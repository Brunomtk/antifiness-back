using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Course
{
    public class CreateReviewRequest
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }
    }
}
