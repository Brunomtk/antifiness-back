namespace Core.DTO.Workout
{
    public class WorkoutStatsDTO
    {
        public int TotalExercises { get; set; }
        public int ActiveExercises { get; set; }
        public int TotalWorkouts { get; set; }
        public int CompletedWorkouts { get; set; }
        public int TemplateWorkouts { get; set; }
        public double CompletionRate { get; set; }
        public int TotalDuration { get; set; }
        public int TotalCalories { get; set; }
        public double AverageRating { get; set; }
        public int PersonalRecords { get; set; }
        public List<WorkoutTypeStats> WorkoutsByType { get; set; } = new();
        public List<MuscleGroupStats> MuscleGroupDistribution { get; set; } = new();
    }

    public class WorkoutTypeStats
    {
        public string Type { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class MuscleGroupStats
    {
        public string MuscleGroup { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}
