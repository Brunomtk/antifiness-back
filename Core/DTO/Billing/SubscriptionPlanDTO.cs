using Core.Enums.Billing;

namespace Core.DTO.Billing
{
    public class SubscriptionPlanDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int PriceCents { get; set; }
        public string Currency { get; set; } = "BRL";
        public BillingInterval Interval { get; set; }
        public int TrialDays { get; set; }
        public bool IsActive { get; set; }
    }
}
