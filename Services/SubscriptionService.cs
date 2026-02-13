using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.DTO.Billing;
using Core.Enums.Billing;
using Core.Models.Billing;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Billing;

namespace Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICompanySubscriptionRepository _companySubscriptionRepo;

        public SubscriptionService(IUnitOfWork unitOfWork, ICompanySubscriptionRepository companySubscriptionRepo)
        {
            _unitOfWork = unitOfWork;
            _companySubscriptionRepo = companySubscriptionRepo;
        }

        public async Task<IList<SubscriptionPlanDTO>> GetPlansAsync(bool includeInactive = false)
        {
            var plans = (await _unitOfWork.SubscriptionPlans.GetAll()).ToList();
            if (!includeInactive)
            {
                plans = plans.Where(p => p.IsActive).ToList();
            }

            return plans
                .OrderBy(p => p.PriceCents)
                .ThenBy(p => p.Name)
                .Select(MapPlan)
                .ToList();
        }

        public async Task<SubscriptionPlanDTO> CreatePlanAsync(CreateSubscriptionPlanRequest request)
        {
            var entity = new SubscriptionPlan
            {
                Name = request.Name,
                Description = request.Description,
                PriceCents = request.PriceCents,
                Currency = request.Currency,
                Interval = request.Interval,
                TrialDays = request.TrialDays,
                IsActive = request.IsActive
            };

            await _unitOfWork.SubscriptionPlans.Add(entity);
            await _unitOfWork.SaveAsync();

            return MapPlan(entity);
        }

        public async Task<CompanySubscriptionDTO?> GetCompanySubscriptionAsync(int empresaId)
        {
            var sub = await _companySubscriptionRepo.GetByEmpresaIdAsync(empresaId);
            if (sub == null)
            {
                return null;
            }

            if (sub.Plan == null)
            {
                sub.Plan = await _unitOfWork.SubscriptionPlans.GetById(sub.SubscriptionPlanId);
            }

            return MapSubscription(sub);
        }

        public async Task<CompanySubscriptionDTO> UpsertCompanySubscriptionAsync(int empresaId, UpsertCompanySubscriptionRequest request)
        {
            var plan = await _unitOfWork.SubscriptionPlans.GetById(request.SubscriptionPlanId);
            if (plan == null || !plan.IsActive)
            {
                throw new InvalidOperationException("Plano de assinatura inválido ou inativo.");
            }

            var existing = await _companySubscriptionRepo.GetByEmpresaIdAsync(empresaId);

            var now = DateTime.UtcNow;
            var start = now;
            var end = ComputePeriodEndUtc(now, plan.Interval);

            SubscriptionStatus status;
            if (plan.TrialDays > 0)
            {
                status = SubscriptionStatus.Trialing;
                end = now.AddDays(plan.TrialDays);
            }
            else
            {
                status = SubscriptionStatus.Active;
            }

            if (existing == null)
            {
                var entity = new CompanySubscription
                {
                    EmpresaId = empresaId,
                    SubscriptionPlanId = plan.Id,
                    Status = status,
                    CurrentPeriodStartUtc = start,
                    CurrentPeriodEndUtc = end,
                    CancelAtPeriodEnd = request.CancelAtPeriodEnd,
                    Provider = request.Provider,
                    ProviderSubscriptionId = request.ProviderSubscriptionId
                };

                await _unitOfWork.CompanySubscriptions.Add(entity);
                await _unitOfWork.SaveAsync();

                entity.Plan = plan;
                return MapSubscription(entity);
            }

            existing.SubscriptionPlanId = plan.Id;
            existing.Status = status;
            existing.CurrentPeriodStartUtc = start;
            existing.CurrentPeriodEndUtc = end;
            existing.CancelAtPeriodEnd = request.CancelAtPeriodEnd;
            existing.Provider = request.Provider;
            existing.ProviderSubscriptionId = request.ProviderSubscriptionId;
            existing.CanceledAtUtc = null;

            _unitOfWork.CompanySubscriptions.Update(existing);
            await _unitOfWork.SaveAsync();

            existing.Plan = plan;
            return MapSubscription(existing);
        }

        public async Task<bool> CancelAtPeriodEndAsync(int empresaId, bool cancelAtPeriodEnd)
        {
            var existing = await _companySubscriptionRepo.GetByEmpresaIdAsync(empresaId);
            if (existing == null)
            {
                return false;
            }

            existing.CancelAtPeriodEnd = cancelAtPeriodEnd;
            if (cancelAtPeriodEnd)
            {
                existing.Status = SubscriptionStatus.Canceled;
                existing.CanceledAtUtc = DateTime.UtcNow;
            }

            _unitOfWork.CompanySubscriptions.Update(existing);
            await _unitOfWork.SaveAsync();
            return true;
        }

        private static DateTime ComputePeriodEndUtc(DateTime startUtc, BillingInterval interval)
        {
            return interval switch
            {
                BillingInterval.Monthly => startUtc.AddMonths(1),
                BillingInterval.Yearly => startUtc.AddYears(1),
                _ => startUtc.AddMonths(1)
            };
        }

        private static SubscriptionPlanDTO MapPlan(SubscriptionPlan p)
        {
            return new SubscriptionPlanDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                PriceCents = p.PriceCents,
                Currency = p.Currency,
                Interval = p.Interval,
                TrialDays = p.TrialDays,
                IsActive = p.IsActive
            };
        }

        private static CompanySubscriptionDTO MapSubscription(CompanySubscription s)
        {
            var plan = s.Plan;
            return new CompanySubscriptionDTO
            {
                Id = s.Id,
                EmpresaId = s.EmpresaId,
                SubscriptionPlanId = s.SubscriptionPlanId,
                PlanName = plan?.Name ?? string.Empty,
                PlanPriceCents = plan?.PriceCents ?? 0,
                PlanCurrency = plan?.Currency ?? "BRL",
                PlanInterval = plan?.Interval ?? BillingInterval.Monthly,
                Status = s.Status,
                CurrentPeriodStartUtc = s.CurrentPeriodStartUtc,
                CurrentPeriodEndUtc = s.CurrentPeriodEndUtc,
                CancelAtPeriodEnd = s.CancelAtPeriodEnd,
                Provider = s.Provider,
                ProviderSubscriptionId = s.ProviderSubscriptionId,
                CanceledAtUtc = s.CanceledAtUtc
            };
        }
    }
}
