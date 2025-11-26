using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Diet
{
    public class DietMealFoodSubstitution : BaseModel
    {
        [Required]
        public int MealFoodId { get; set; }

        [Required]
        public int FoodId { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [ForeignKey("MealFoodId")]
        public virtual DietMealFood MealFood { get; set; } = null!;

        [ForeignKey("FoodId")]
        public virtual Food Food { get; set; } = null!;
    }
}
