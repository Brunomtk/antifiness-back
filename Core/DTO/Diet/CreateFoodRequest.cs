using Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Diet
{
    public class CreateFoodRequest
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Categoria é obrigatória")]
        public FoodCategory Category { get; set; }

        [Required(ErrorMessage = "Calorias por 100g são obrigatórias")]
        [Range(0, 1000, ErrorMessage = "Calorias devem estar entre 0 e 1000")]
        public double CaloriesPer100g { get; set; }

        [Range(0, 100, ErrorMessage = "Proteína deve estar entre 0 e 100g")]
        public double? ProteinPer100g { get; set; }

        [Range(0, 100, ErrorMessage = "Carboidratos devem estar entre 0 e 100g")]
        public double? CarbsPer100g { get; set; }

        [Range(0, 100, ErrorMessage = "Gordura deve estar entre 0 e 100g")]
        public double? FatPer100g { get; set; }

        [Range(0, 50, ErrorMessage = "Fibra deve estar entre 0 e 50g")]
        public double? FiberPer100g { get; set; }

        [Range(0, 10000, ErrorMessage = "Sódio deve estar entre 0 e 10000mg")]
        public double? SodiumPer100g { get; set; }

        [StringLength(500, ErrorMessage = "Alérgenos devem ter no máximo 500 caracteres")]
        public string? Allergens { get; set; }

        [StringLength(200, ErrorMessage = "Porções comuns devem ter no máximo 200 caracteres")]
        public string? CommonPortions { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
