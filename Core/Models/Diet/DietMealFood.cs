using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Core.Models.Diet
{
    public class DietMealFood : BaseModel
    {
        [Required]
        public int MealId { get; set; }

        [Required]
        public int FoodId { get; set; }

        [Required]
        public double Quantity { get; set; }

        [StringLength(50)]
        public string Unit { get; set; } = "g";

        public double? Calories { get; set; }
        public double? Protein { get; set; }
        public double? Carbs { get; set; }
        public double? Fat { get; set; }

        // Relacionamentos
        [ForeignKey("MealId")]
        public virtual DietMeal Meal { get; set; } = null!;

        [ForeignKey("FoodId")]
        public virtual Food Food { get; set; } = null!;

        public virtual ICollection<DietMealFoodSubstitution> Substitutions { get; set; } = new List<DietMealFoodSubstitution>();
    }
}
