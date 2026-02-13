using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DTO.Billing;

namespace Services
{
    public interface ISubscriptionService
    {
        Task<IList<SubscriptionPlanDTO>> GetPlansAsync(bool includeInactive = false);
        Task<SubscriptionPlanDTO> CreatePlanAsync(CreateSubscriptionPlanRequest request);

        Task<CompanySubscriptionDTO?> GetCompanySubscriptionAsync(int empresaId);
        Task<CompanySubscriptionDTO> UpsertCompanySubscriptionAsync(int empresaId, UpsertCompanySubscriptionRequest request);
        Task<bool> CancelAtPeriodEndAsync(int empresaId, bool cancelAtPeriodEnd);
    }
}
