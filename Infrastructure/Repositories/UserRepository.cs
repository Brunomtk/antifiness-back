// File: Infrastructure/Repositories/UserRepository.cs

using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<List<User>> GetAllAsync();
    }

    public sealed class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly DbContextClass _dbContext;

        public UserRepository(DbContextClass dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<User?> GetByIdAsync(int id)
        {
            // Uses EF Core's FindAsync to load by primary key
            return _dbContext.Set<User>().FindAsync(id).AsTask();
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            // Query by email
            return _dbContext.Set<User>()
                             .FirstOrDefaultAsync(u => u.Email == email);
        }

        public Task<List<User>> GetAllAsync()
        {
            // Return all users as a List<User>
            return _dbContext.Set<User>().ToListAsync();
        }
    }
}
