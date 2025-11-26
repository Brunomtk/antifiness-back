using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Diet
{
    public class UpdateDietMealFoodSubstitutionRequest
    {
        public int? FoodId { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
