// File: Core/Models/PlanFood.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums;

namespace Core.Models.Plan
{
    [Table("PlanFoods")]
    public class PlanFood : BaseModel
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public double Quantity { get; set; }

        [Required]
        public string Unit { get; set; }

        [Required]
        public int Calories { get; set; }

        [Required]
        public required MacroNutrients Macros { get; set; }

        [Required]
        public FoodCategory Category { get; set; }

        // FK para PlanMeal (ambos são int)
        [Required]
        public required int PlanMealId { get; set; }

        [ForeignKey(nameof(PlanMealId))]
        public PlanMeal? PlanMeal { get; set; }
    }
}
