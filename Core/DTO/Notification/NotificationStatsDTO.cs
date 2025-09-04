using System.Collections.Generic;

namespace Core.DTO.Notification
{
    public class NotificationStatsDTO
    {
        public int Total { get; set; }
        public int Unread { get; set; }
        public Dictionary<string, int> ByType { get; set; } = new();
        public Dictionary<string, int> ByCategory { get; set; } = new();
        public Dictionary<string, int> ByPriority { get; set; } = new();
        public double ReadRate { get; set; }
        public double AverageReadTime { get; set; }
        public int MostActiveHour { get; set; }
        public double[] WeeklyTrend { get; set; } = new double[7];
    }
}
