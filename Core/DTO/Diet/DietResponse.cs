using Core.Enums;

using Core.DTO.Nutrition;

namespace Core.DTO.Diet
{
    public class DietResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public int EmpresaId { get; set; }
        public string EmpresaName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DietStatus Status { get; set; }
        public string StatusDescription { get; set; } = string.Empty;

        // Metas nutricionais diárias
        public double? DailyCalories { get; set; }
        public double? DailyProtein { get; set; }
        public double? DailyCarbs { get; set; }
        public double? DailyFat { get; set; }
        public double? DailyFiber { get; set; }
        public double? DailySodium { get; set; }

        public string? Restrictions { get; set; }
        public string? Notes { get; set; }

        public int TotalMeals { get; set; }
        public int CompletedMeals { get; set; }
        public double CompletionPercentage { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public DietMicronutrientsDTO? Micronutrients { get; set; }
        public List<DietMealResponse> Meals { get; set; } = new List<DietMealResponse>();
        public List<DietSupplementResponse> Supplements { get; set; } = new List<DietSupplementResponse>();
    }
}
