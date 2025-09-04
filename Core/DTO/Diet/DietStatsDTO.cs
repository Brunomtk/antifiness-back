namespace Core.DTO.Diet
{
    public class DietStatsDTO
    {
        public int TotalDiets { get; set; }
        public int ActiveDiets { get; set; }
        public int CompletedDiets { get; set; }
        public int PausedDiets { get; set; }
        public int CancelledDiets { get; set; }

        public double ActiveDietsPercentage { get; set; }
        public double CompletedDietsPercentage { get; set; }
        public double PausedDietsPercentage { get; set; }
        public double CancelledDietsPercentage { get; set; }

        public double AverageCaloriesPerDiet { get; set; }
        public double AverageMealsPerDiet { get; set; }
        public double AverageCompletionRate { get; set; }

        public int TotalMeals { get; set; }
        public int CompletedMeals { get; set; }
        public double MealCompletionPercentage { get; set; }

        public double AverageWeightLoss { get; set; }
        public double AverageEnergyLevel { get; set; }
        public double AverageSatisfactionLevel { get; set; }

        public int DietsThisMonth { get; set; }
        public int DietsLastMonth { get; set; }
        public double MonthlyGrowth { get; set; }
    }
}
