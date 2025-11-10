using Infrastructure;

namespace Infrastructure.Repositories
{
    // Helper concrete repository for simple generic usage
    public class GenericRepositoryImpl<T> : GenericRepository<T> where T : class
    {
        public GenericRepositoryImpl(DbContextClass context) : base(context) { }
    }
}
