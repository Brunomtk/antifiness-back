// File: Infrastructure/Repositories/ClientRepositories.cs
using Core.Models.Client;
using Microsoft.EntityFrameworkCore;
using Saller.Infrastructure.ServiceExtension;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IClientRepository : IGenericRepository<Client>
    {
        Task<Client?> GetByIdAsync(int id);
        Task<PagedResult<Client>> GetAllPagedAsync(int page, int pageSize);
    }

    public interface IClientGoalRepository : IGenericRepository<ClientGoal>
    {
        Task<ClientGoal?> GetByIdAsync(int id);
        Task<PagedResult<ClientGoal>> GetByClientIdPagedAsync(int clientId, int page, int pageSize);
    }

    public interface IClientMeasurementRepository : IGenericRepository<ClientMeasurement>
    {
        Task<ClientMeasurement?> GetByIdAsync(int id);
        Task<PagedResult<ClientMeasurement>> GetByClientIdPagedAsync(int clientId, int page, int pageSize);
    }

    public sealed class ClientRepository : GenericRepository<Client>, IClientRepository
    {
        private readonly DbContextClass _db;
        public ClientRepository(DbContextClass db) : base(db) => _db = db;

        public async Task<Client?> GetByIdAsync(int id)
            => await _db.Set<Client>()
                        .Include(c => c.Goals)
                        .Include(c => c.Measurements)
                        .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<PagedResult<Client>> GetAllPagedAsync(int page, int pageSize)
        {
            var query = _db.Set<Client>()
                           .Include(c => c.Goals)
                           .Include(c => c.Measurements)
                           .AsQueryable();
            return await query.GetPagedAsync(page, pageSize);
        }
    }

    public sealed class ClientGoalRepository : GenericRepository<ClientGoal>, IClientGoalRepository
    {
        private readonly DbContextClass _db;
        public ClientGoalRepository(DbContextClass db) : base(db) => _db = db;

        public async Task<ClientGoal?> GetByIdAsync(int id)
            => await _db.Set<ClientGoal>().FirstOrDefaultAsync(g => g.Id == id);

        public async Task<PagedResult<ClientGoal>> GetByClientIdPagedAsync(int clientId, int page, int pageSize)
        {
            var query = _db.Set<ClientGoal>()
                           .Where(g => g.ClientId == clientId)
                           .AsQueryable();
            return await query.GetPagedAsync(page, pageSize);
        }
    }

    public sealed class ClientMeasurementRepository : GenericRepository<ClientMeasurement>, IClientMeasurementRepository
    {
        private readonly DbContextClass _db;
        public ClientMeasurementRepository(DbContextClass db) : base(db) => _db = db;

        public async Task<ClientMeasurement?> GetByIdAsync(int id)
            => await _db.Set<ClientMeasurement>().FirstOrDefaultAsync(m => m.Id == id);

        public async Task<PagedResult<ClientMeasurement>> GetByClientIdPagedAsync(int clientId, int page, int pageSize)
        {
            var query = _db.Set<ClientMeasurement>()
                           .Where(m => m.ClientId == clientId)
                           .AsQueryable();
            return await query.GetPagedAsync(page, pageSize);
        }
    }
}
