using Core.Models.Nutrition;

namespace Infrastructure.Repositories
{
    public class FoodMicronutrientRepository : GenericRepository<FoodMicronutrient>
    {
        public FoodMicronutrientRepository(DbContextClass context) : base(context) { }
    }
}
