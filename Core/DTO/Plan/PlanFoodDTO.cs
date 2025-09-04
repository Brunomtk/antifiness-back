// File: Core/DTO/PlanFoodDTO.cs
using Core.Enums;

namespace Core.DTO.Plan
{
    public class PlanFoodDTO
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public double Quantity { get; set; }

        public required string Unit { get; set; }

        public int Calories { get; set; }

        public MacroNutrientsDTO Macros { get; set; } = new MacroNutrientsDTO();

        public FoodCategory Category { get; set; }

        // FK para o plano de refeição
        public int PlanMealId { get; set; }
    }
}
