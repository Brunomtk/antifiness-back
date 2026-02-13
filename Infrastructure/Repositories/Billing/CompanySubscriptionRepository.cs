using System.Threading.Tasks;
using Core.Models.Billing;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Billing
{
    public interface ICompanySubscriptionRepository
    {
        Task<CompanySubscription?> GetByEmpresaIdAsync(int empresaId);
    }

    public class CompanySubscriptionRepository : ICompanySubscriptionRepository
    {
        private readonly DbContextClass _context;

        public CompanySubscriptionRepository(DbContextClass context)
        {
            _context = context;
        }

        public Task<CompanySubscription?> GetByEmpresaIdAsync(int empresaId)
        {
            return _context.CompanySubscriptions
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.EmpresaId == empresaId);
        }
    }
}
