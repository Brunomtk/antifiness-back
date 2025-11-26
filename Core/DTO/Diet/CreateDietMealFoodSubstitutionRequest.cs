using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Diet
{
    public class CreateDietMealFoodSubstitutionRequest
    {
        [Required]
        public int FoodId { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
