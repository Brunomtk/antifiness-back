// File: Core/DTO/UpdatePlanFoodRequest.cs
using Core.Enums;

namespace Core.DTO.Plan
{
    public class UpdatePlanFoodRequest
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public double? Quantity { get; set; }
        public string? Unit { get; set; }
        public int? Calories { get; set; }
        public MacroNutrientsDTO? Macros { get; set; }
        public FoodCategory? Category { get; set; }
    }
}
