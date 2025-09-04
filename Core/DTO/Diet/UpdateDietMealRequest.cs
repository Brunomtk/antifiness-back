using Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Diet
{
    public class UpdateDietMealRequest
    {
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string? Name { get; set; }

        public MealType? Type { get; set; }

        public TimeSpan? ScheduledTime { get; set; }

        [StringLength(500, ErrorMessage = "Instruções devem ter no máximo 500 caracteres")]
        public string? Instructions { get; set; }

        public bool? IsCompleted { get; set; }

        public List<CreateDietMealFoodRequest>? Foods { get; set; }
    }
}
