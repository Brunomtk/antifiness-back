using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Diet
{
    public class CreateDietProgressRequest
    {
        [Required(ErrorMessage = "Data é obrigatória")]
        public DateTime Date { get; set; }

        [Range(0, 1000, ErrorMessage = "Peso deve estar entre 0 e 1000kg")]
        public double? Weight { get; set; }

        [Range(0, 10000, ErrorMessage = "Calorias devem estar entre 0 e 10000")]
        public double? CaloriesConsumed { get; set; }

        [Range(0, 20, ErrorMessage = "Refeições completas devem estar entre 0 e 20")]
        public int MealsCompleted { get; set; } = 0;

        [Range(0, 20, ErrorMessage = "Total de refeições deve estar entre 0 e 20")]
        public int TotalMeals { get; set; } = 0;

        [StringLength(1000, ErrorMessage = "Observações devem ter no máximo 1000 caracteres")]
        public string? Notes { get; set; }

        [Range(1, 10, ErrorMessage = "Nível de energia deve estar entre 1 e 10")]
        public double? EnergyLevel { get; set; }

        [Range(1, 10, ErrorMessage = "Nível de fome deve estar entre 1 e 10")]
        public double? HungerLevel { get; set; }

        [Range(1, 10, ErrorMessage = "Nível de satisfação deve estar entre 1 e 10")]
        public double? SatisfactionLevel { get; set; }
    }
}
