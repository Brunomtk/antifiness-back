using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.DTO.Nutrition;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public interface INutritionService
    {
        Task<List<MicronutrientDTO>> GetMicronutrientTypesAsync();
        Task<List<FoodMicronutrientDTO>> GetFoodMicronutrientsAsync(int foodId);
        Task SetFoodMicronutrientsAsync(int foodId, IEnumerable<FoodMicronutrientDTO> micros);
        Task<CalcMicrosResultDTO> CalculateMicrosAsync(CalcMicrosInputDTO input);
        Task<CalcMicrosResultDTO> CalculateMicrosForDietAsync(int dietId);
    }

    public class NutritionService : INutritionService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbContextClass _ctx;

        public NutritionService(IUnitOfWork unitOfWork, DbContextClass ctx)
        {
            _uow = unitOfWork;
            _ctx = ctx;
        }

        public async Task<List<MicronutrientDTO>> GetMicronutrientTypesAsync()
        {
            var list = await _ctx.MicronutrientTypes.AsNoTracking().ToListAsync();
            return list.Select(x => new MicronutrientDTO
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Unit = x.Unit
            }).ToList();
        }

        public async Task<List<FoodMicronutrientDTO>> GetFoodMicronutrientsAsync(int foodId)
        {
            var list = await _ctx.FoodMicronutrients
                .Include(x => x.MicronutrientType)
                .Where(x => x.FoodId == foodId)
                .AsNoTracking()
                .ToListAsync();

            return list.Select(x => new FoodMicronutrientDTO
            {
                MicronutrientTypeId = x.MicronutrientTypeId,
                AmountPer100g = x.AmountPer100g
            }).ToList();
        }

        public async Task SetFoodMicronutrientsAsync(int foodId, IEnumerable<FoodMicronutrientDTO> micros)
        {
            var current = await _ctx.FoodMicronutrients.Where(x => x.FoodId == foodId).ToListAsync();
            if (current.Count > 0)
                _ctx.FoodMicronutrients.RemoveRange(current);

            var entities = micros.Select(m => new Core.Models.Nutrition.FoodMicronutrient
            {
                FoodId = foodId,
                MicronutrientTypeId = m.MicronutrientTypeId,
                AmountPer100g = m.AmountPer100g
            }).ToList();

            if (entities.Count > 0)
                await _ctx.FoodMicronutrients.AddRangeAsync(entities);

            await _uow.SaveAsync();
        }

        public async Task<CalcMicrosResultDTO> CalculateMicrosAsync(CalcMicrosInputDTO input)
        {
            var ids = input.Items.Select(i => i.FoodId).Distinct().ToList();

            var rows = await _ctx.FoodMicronutrients
                .Include(fm => fm.MicronutrientType)
                .Where(fm => ids.Contains(fm.FoodId))
                .ToListAsync();

            var totals = rows
                .Join(input.Items, fm => fm.FoodId, it => it.FoodId,
                    (fm, it) => new
                    {
                        fm.MicronutrientTypeId,
                        fm.MicronutrientType.Code,
                        fm.MicronutrientType.Name,
                        fm.MicronutrientType.Unit,
                        Amount = fm.AmountPer100g * (it.PortionGrams / 100m)
                    })
                .GroupBy(x => new { x.MicronutrientTypeId, x.Code, x.Name, x.Unit })
                .Select(g => new CalcMicrosResultDTO.MicronutrientTotal(
                    g.Key.MicronutrientTypeId, g.Key.Code, g.Key.Name, g.Key.Unit, g.Sum(z => z.Amount)))
                .OrderBy(x => x.Name)
                .ToList();

            return new CalcMicrosResultDTO { Totals = totals };
        }

        public async Task<CalcMicrosResultDTO> CalculateMicrosForDietAsync(int dietId)
        {
            var items = await _ctx.DietMealFoods
                .Include(dmf => dmf.Meal)
                .Where(dmf => dmf.Meal.DietId == dietId)
                .Select(dmf => new CalcMicrosInputDTO.Item
                {
                    FoodId = dmf.FoodId,
                    PortionGrams = (decimal)dmf.Quantity
                })
                .ToListAsync();

            return await CalculateMicrosAsync(new CalcMicrosInputDTO { Items = items });
        }
    }
}
