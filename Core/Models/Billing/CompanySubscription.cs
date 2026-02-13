using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums.Billing;
using Core.Models;

namespace Core.Models.Billing
{
    [Table("CompanySubscriptions")]
    public class CompanySubscription : BaseModel
    {
        [Required]
        public int EmpresaId { get; set; }

        [ForeignKey(nameof(EmpresaId))]
        public Empresas? Empresa { get; set; }

        [Required]
        public int SubscriptionPlanId { get; set; }

        [ForeignKey(nameof(SubscriptionPlanId))]
        public SubscriptionPlan? Plan { get; set; }

        public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Trialing;

        public DateTime CurrentPeriodStartUtc { get; set; } = DateTime.UtcNow;

        public DateTime CurrentPeriodEndUtc { get; set; } = DateTime.UtcNow;

        public bool CancelAtPeriodEnd { get; set; } = false;

        public string? Provider { get; set; }

        public string? ProviderSubscriptionId { get; set; }

        public DateTime? CanceledAtUtc { get; set; }
    }
}
