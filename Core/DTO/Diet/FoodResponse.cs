using Core.DTO.Nutrition;
using Core.Enums;

namespace Core.DTO.Diet
{
    public class FoodResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public FoodCategory Category { get; set; }
        public string CategoryDescription { get; set; } = string.Empty;
        public double CaloriesPer100g { get; set; }
        public double? ProteinPer100g { get; set; }
        public double? CarbsPer100g { get; set; }
        public double? FatPer100g { get; set; }
        public double? FiberPer100g { get; set; }
        public double? SodiumPer100g { get; set; }
        public string? Allergens { get; set; }
        public string? CommonPortions { get; set; }
        public bool IsActive { get; set; }
        public DietMicronutrientsDTO? Micronutrients { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
