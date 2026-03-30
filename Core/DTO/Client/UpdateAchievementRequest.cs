using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Client
{
    public class UpdateAchievementRequest
    {
        [StringLength(200, ErrorMessage = "Título deve ter no máximo 200 caracteres")]
        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Type { get; set; }
        public string? Category { get; set; }
    }
}
