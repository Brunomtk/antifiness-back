using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Client
{
    public class AddAchievementRequest
    {
        [Required(ErrorMessage = "Título é obrigatório")]
        [StringLength(200, ErrorMessage = "Título deve ter no máximo 200 caracteres")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tipo é obrigatório")]
        public string Type { get; set; } = string.Empty;

        [Required(ErrorMessage = "Categoria é obrigatória")]
        public string Category { get; set; } = string.Empty;
    }
}
