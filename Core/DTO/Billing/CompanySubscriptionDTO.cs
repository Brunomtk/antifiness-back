using System;
using Core.Enums.Billing;

namespace Core.DTO.Billing
{
    public class CompanySubscriptionDTO
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }
        public int SubscriptionPlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public int PlanPriceCents { get; set; }
        public string PlanCurrency { get; set; } = "BRL";
        public BillingInterval PlanInterval { get; set; }
        public SubscriptionStatus Status { get; set; }
        public DateTime CurrentPeriodStartUtc { get; set; }
        public DateTime CurrentPeriodEndUtc { get; set; }
        public bool CancelAtPeriodEnd { get; set; }
        public string? Provider { get; set; }
        public string? ProviderSubscriptionId { get; set; }
        public DateTime? CanceledAtUtc { get; set; }
    }
}
