using Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Core.Models.Diet
{
    public class Food : BaseModel
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public FoodCategory Category { get; set; }

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

        // Relacionamentos
        public virtual ICollection<DietMealFood> MealFoods { get; set; } = new List<DietMealFood>();
    }
}
