// File: Core/DTO/CreatePlanFoodRequest.cs
using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Core.DTO.Plan
{
    public class CreatePlanFoodRequest
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public double Quantity { get; set; }

        [Required]
        public required string Unit { get; set; }

        [Required]
        public int Calories { get; set; }

        public MacroNutrientsDTO Macros { get; set; } = new MacroNutrientsDTO();

        [Required]
        public FoodCategory Category { get; set; }
    }
}
