namespace Core.DTO.Diet
{
    public class DietProgressResponse
    {
        public int Id { get; set; }
        public int DietId { get; set; }
        public DateTime Date { get; set; }
        public double? Weight { get; set; }
        public double? CaloriesConsumed { get; set; }
        public int MealsCompleted { get; set; }
        public int TotalMeals { get; set; }
        public double CompletionPercentage { get; set; }
        public string? Notes { get; set; }
        public double? EnergyLevel { get; set; }
        public double? HungerLevel { get; set; }
        public double? SatisfactionLevel { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
