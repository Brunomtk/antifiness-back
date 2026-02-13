using Core.Enums.Billing;
using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Billing
{
    public class CreateSubscriptionPlanRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public int PriceCents { get; set; }

        [Required]
        public string Currency { get; set; } = "BRL";

        public BillingInterval Interval { get; set; } = BillingInterval.Monthly;

        public int TrialDays { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }
}
