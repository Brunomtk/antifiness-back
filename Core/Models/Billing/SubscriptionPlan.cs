using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums.Billing;

namespace Core.Models.Billing
{
    [Table("SubscriptionPlans")]
    public class SubscriptionPlan : BaseModel
    {
        [Required]
        public required string Name { get; set; }

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
