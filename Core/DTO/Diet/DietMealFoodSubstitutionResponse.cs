using System;

namespace Core.DTO.Diet
{
    public class DietMealFoodSubstitutionResponse
    {
        public int Id { get; set; }
        public int MealFoodId { get; set; }
        public int FoodId { get; set; }
        public string FoodName { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
