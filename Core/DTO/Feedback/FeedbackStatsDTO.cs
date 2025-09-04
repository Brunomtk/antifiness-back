namespace Core.DTO.Feedback
{
    public class FeedbackStatsDTO
    {
        public int TotalFeedbacks { get; set; }
        public int PendingFeedbacks { get; set; }
        public int ResolvedFeedbacks { get; set; }
        public double AverageRating { get; set; }
        public Dictionary<string, int> FeedbacksByType { get; set; } = new();
        public Dictionary<string, int> FeedbacksByCategory { get; set; } = new();
        public Dictionary<string, int> FeedbacksByStatus { get; set; } = new();
    }
}
