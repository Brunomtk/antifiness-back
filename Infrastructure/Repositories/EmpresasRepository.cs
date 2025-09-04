using Core.DTO;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Saller.Infrastructure.ServiceExtension;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IEmpresasRepository : IGenericRepository<Empresas>
    {
        Task<Empresas?> GetByIdAsync(int id);
        Task<Empresas?> GetByUserIdAsync(int userId);
        Task<Empresas?> GetByNameAsync(string name);
        Task<Empresas?> GetByCnpjAsync(string cnpj);
        Task<Empresas?> GetByEmailAsync(string email);
        Task<PagedResult<Empresas>> GetAllPagedAsync(FiltersDTO filters);
        Task<List<Empresas>> GetAllAsync();
        // Métodos específicos para empresas podem ser adicionados aqui
    }

    public sealed class EmpresasRepository : GenericRepository<Empresas>, IEmpresasRepository
    {
        private readonly DbContextClass _dbContext;

        public EmpresasRepository(DbContextClass dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Empresas?> GetByIdAsync(int id)
        {
            return await _dbContext.Set<Empresas>()
                                   .Include(e => e.User)
                                   .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Empresas?> GetByUserIdAsync(int userId)
        {
            return await _dbContext.Set<Empresas>()
                                   .Include(e => e.User)
                                   .FirstOrDefaultAsync(e => e.UserId == userId);
        }

        public async Task<Empresas?> GetByNameAsync(string name)
        {
            return await _dbContext.Set<Empresas>()
                                   .FirstOrDefaultAsync(e => e.Name == name);
        }

        public async Task<Empresas?> GetByCnpjAsync(string cnpj)
        {
            return await _dbContext.Set<Empresas>()
                                   .FirstOrDefaultAsync(e => e.CNPJ == cnpj);
        }

        public async Task<Empresas?> GetByEmailAsync(string email)
        {
            return await _dbContext.Set<Empresas>()
                                   .Include(e => e.User)
                                   .FirstOrDefaultAsync(e => e.Email == email);
        }

        public async Task<List<Empresas>> GetAllAsync()
        {
            return await _dbContext.Set<Empresas>()
                                   .Include(e => e.User)
                                   .ToListAsync();
        }

        public async Task<PagedResult<Empresas>> GetAllPagedAsync(FiltersDTO filters)
        {
            var query = _dbContext.Set<Empresas>()
                                  .Include(e => e.User)
                                  .AsQueryable();

            if (!string.IsNullOrEmpty(filters.Name))
            {
                var pattern = $"%{filters.Name.ToLower()}%";
                query = query.Where(e => EF.Functions.Like(e.Name.ToLower(), pattern) ||
                                        EF.Functions.Like(e.Email.ToLower(), pattern) ||
                                        (e.CNPJ != null && EF.Functions.Like(e.CNPJ, pattern)));
            }

            return await query.GetPagedAsync(filters.pageNumber, filters.pageSize);
        }

        // Implementações específicas para empresas podem ser adicionadas aqui
    }
}
