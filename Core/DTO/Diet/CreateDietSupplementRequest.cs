using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Diet
{
    public class CreateDietSupplementRequest
    {
        [Required]
        [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Quantidade deve ter no máximo 200 caracteres")]
        public string? Quantity { get; set; }

        [StringLength(1000, ErrorMessage = "Como usar deve ter no máximo 1000 caracteres")]
        public string? Instructions { get; set; }
    }
}
