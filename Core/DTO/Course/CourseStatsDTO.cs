namespace Core.DTO.Course
{
    public class CourseStatsDTO
    {
        public int TotalCourses { get; set; }
        public int PublishedCourses { get; set; }
        public int DraftCourses { get; set; }
        public int TotalEnrollments { get; set; }
        public int ActiveEnrollments { get; set; }
        public int CompletedEnrollments { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public decimal CompletionRate { get; set; }
        public List<CategoryStats> PopularCategories { get; set; } = new();
        public List<MonthlyEnrollment> MonthlyEnrollments { get; set; } = new();
    }

    public class CategoryStats
    {
        public string Category { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class MonthlyEnrollment
    {
        public string Month { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Revenue { get; set; }
    }
}
