using System.Collections.Generic;

namespace Core.DTO.Diet
{
    public class DietMealFoodResponse
    {
        public int Id { get; set; }
        public int MealId { get; set; }
        public int FoodId { get; set; }
        public string FoodName { get; set; } = string.Empty;
        public double Quantity { get; set; }
        public string Unit { get; set; } = "g";
        public double? Calories { get; set; }
        public double? Protein { get; set; }
        public double? Carbs { get; set; }
        public double? Fat { get; set; }

        public List<DietMealFoodSubstitutionResponse> Substitutions { get; set; } = new();
    }
}
