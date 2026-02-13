namespace Core.DTO.User
{
    public class UserStats
    {
        public int TotalUsers { get; set; }
        public int TotalAdmins { get; set; }
        public int TotalCompanies { get; set; }
        public int TotalClients { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public int PendingUsers { get; set; }
        public int NewUsersThisMonth { get; set; }
        public int VerifiedAdmins { get; set; }
        public int ClientsWithNutritionist { get; set; }
        public double GrowthPercentage { get; set; }
    }
}
