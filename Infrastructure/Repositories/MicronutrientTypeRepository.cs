using Core.Models.Nutrition;

namespace Infrastructure.Repositories
{
    public class MicronutrientTypeRepository : GenericRepository<MicronutrientType>
    {
        public MicronutrientTypeRepository(DbContextClass context) : base(context) { }
    }
}
