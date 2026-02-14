using Core.Models.Nutrition;
using Core.Enums;
using System.ComponentModel.DataAnnotations;

using Core.Models.Nutrition;

namespace Core.Models.Diet
{
    public class Food : BaseModel
    {
        /// <summary>
        /// Empresa dona do cadastro do alimento.
        /// Nullable para compatibilidade com bases antigas (alimentos "globais").
        /// No fluxo normal, a API força/filtra por EmpresaId.
        /// </summary>
        public int? EmpresaId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public FoodCategory? Category { get; set; }

        // Valores nutricionais por 100g
        [Required]
        public double CaloriesPer100g { get; set; }

        public double? ProteinPer100g { get; set; }
        public double? CarbsPer100g { get; set; }
        public double? FatPer100g { get; set; }
        public double? FiberPer100g { get; set; }
        public double? SodiumPer100g { get; set; }

        [StringLength(500)]
        public string? Allergens { get; set; }

        [StringLength(200)]
        public string? CommonPortions { get; set; }

        public bool IsActive { get; set; } = true;

        
        // Micronutrientes por 100g (OWNED no mesmo registro)
        public MicronutrientProfile? Micros { get; set; }
// Relacionamentos
        public virtual ICollection<DietMealFood> MealFoods { get; set; } = new List<DietMealFood>();
        public virtual ICollection<FoodMicronutrient> Micronutrients { get; set; } = new List<FoodMicronutrient>();
    }
}
