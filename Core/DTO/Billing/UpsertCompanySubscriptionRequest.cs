using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Billing
{
    public class UpsertCompanySubscriptionRequest
    {
        [Required]
        public int SubscriptionPlanId { get; set; }

        public string? Provider { get; set; }

        public string? ProviderSubscriptionId { get; set; }

        public bool CancelAtPeriodEnd { get; set; } = false;
    }
}
