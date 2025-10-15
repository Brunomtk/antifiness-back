using Core.Models;

namespace Core.Models.Nutrition
{
    public class FoodMicronutrient : BaseModel
    {
        public int FoodId { get; set; }
        public Core.Models.Diet.Food Food { get; set; } = default!;

        public int MicronutrientTypeId { get; set; }
        public MicronutrientType MicronutrientType { get; set; } = default!;

        // Quantidade por 100g do alimento
        public decimal AmountPer100g { get; set; }
    }
}
