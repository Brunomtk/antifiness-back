using Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Diet
{
    public class CreateDietMealRequest
    {
        [Required(ErrorMessage = "Nome da refeição é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Name { get; set; } = string.Empty;
        public MealType? Type { get; set; }

        public TimeSpan? ScheduledTime { get; set; }

        [StringLength(500, ErrorMessage = "Instruções devem ter no máximo 500 caracteres")]
        public string? Instructions { get; set; }

        public List<CreateDietMealFoodRequest> Foods { get; set; } = new List<CreateDietMealFoodRequest>();
    }
}
