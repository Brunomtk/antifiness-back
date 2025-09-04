using System.Collections.Generic;

namespace Core.DTO.Client
{
    public class ClientStatsDTO
    {
        public int TotalClients { get; set; }
        public int ActiveClients { get; set; }
        public int InactiveClients { get; set; }
        public int PausedClients { get; set; }
        public int NewClientsThisMonth { get; set; }
        public int ClientsWithGoalsAchieved { get; set; }
        public double AverageWeightLoss { get; set; }
        public double RetentionRate { get; set; }
        public double MonthlyGrowthPercentage { get; set; }
        public int ClientsWithNutritionist { get; set; }
        public int ClientsWithActivePlan { get; set; }
        public Dictionary<string, int>? ClientsByActivityLevel { get; set; }
        public Dictionary<string, int>? ClientsByGoalType { get; set; }
        public Dictionary<string, int>? ClientsByAgeGroup { get; set; }
    }
}
