using Core.Enums;

namespace Core.DTO.Diet
{
    public class DietMealResponse
    {
        public int Id { get; set; }
        public int DietId { get; set; }
        public string Name { get; set; } = string.Empty;
        public MealType Type { get; set; }
        public string TypeDescription { get; set; } = string.Empty;
        public TimeSpan? ScheduledTime { get; set; }
        public string? Instructions { get; set; }
        public double? TotalCalories { get; set; }
        public double? TotalProtein { get; set; }
        public double? TotalCarbs { get; set; }
        public double? TotalFat { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<DietMealFoodResponse> Foods { get; set; } = new List<DietMealFoodResponse>();
    }
}
