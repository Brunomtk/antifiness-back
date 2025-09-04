using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.DTO.Diet;
using Core.Enums;
using Core.Models.Diet;
using Infrastructure.Repositories;

namespace Services
{
    public interface IDietService
    {
        Task<DietsPagedDTO> GetAllDietsAsync(int pageNumber, int pageSize, DietFiltersDTO? filters = null);
        Task<DietResponse?> GetDietByIdAsync(int id);
        Task<DietResponse> CreateDietAsync(CreateDietRequest request);
        Task<DietResponse?> UpdateDietAsync(int id, UpdateDietRequest request);
        Task<bool> DeleteDietAsync(int id);
        Task<DietStatsDTO> GetDietStatsAsync();

        // Meals
        Task<List<DietMealResponse>> GetDietMealsAsync(int dietId);
        Task<DietMealResponse> CreateDietMealAsync(int dietId, CreateDietMealRequest request);
        Task<DietMealResponse?> UpdateDietMealAsync(int mealId, UpdateDietMealRequest request);
        Task<bool> DeleteDietMealAsync(int mealId);
        Task<bool> CompleteMealAsync(int mealId);

        // Foods
        Task<List<FoodResponse>> GetAllFoodsAsync(string? search = null, FoodCategory? category = null);
        Task<FoodResponse?> GetFoodByIdAsync(int id);
        Task<FoodResponse> CreateFoodAsync(CreateFoodRequest request);
        Task<FoodResponse?> UpdateFoodAsync(int id, UpdateFoodRequest request);
        Task<bool> DeleteFoodAsync(int id);

        // Progress
        Task<List<DietProgressResponse>> GetDietProgressAsync(int dietId);
        Task<DietProgressResponse> CreateDietProgressAsync(int dietId, CreateDietProgressRequest request);
    }

    public class DietService : IDietService
    {
        private readonly DietRepository _dietRepository;
        private readonly FoodRepository _foodRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DietService(DietRepository dietRepository, FoodRepository foodRepository, IUnitOfWork unitOfWork)
        {
            _dietRepository = dietRepository;
            _foodRepository = foodRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<DietsPagedDTO> GetAllDietsAsync(int pageNumber, int pageSize, DietFiltersDTO? filters = null)
        {
            var query = (await _dietRepository.GetAll()).AsQueryable();

            if (filters != null)
            {
                if (!string.IsNullOrEmpty(filters.Search))
                {
                    query = query.Where(d =>
                        d.Name.Contains(filters.Search) ||
                        (d.Client != null && d.Client.Name.Contains(filters.Search)) ||
                        (d.Empresa != null && d.Empresa.Name.Contains(filters.Search)));
                }

                if (filters.Status.HasValue) query = query.Where(d => d.Status == filters.Status.Value);
                if (filters.ClientId.HasValue) query = query.Where(d => d.ClientId == filters.ClientId.Value);
                if (filters.EmpresaId.HasValue) query = query.Where(d => d.EmpresaId == filters.EmpresaId.Value);
                if (filters.StartDateFrom.HasValue) query = query.Where(d => d.StartDate >= filters.StartDateFrom.Value);
                if (filters.StartDateTo.HasValue) query = query.Where(d => d.StartDate <= filters.StartDateTo.Value);
                if (filters.EndDateFrom.HasValue) query = query.Where(d => d.EndDate >= filters.EndDateFrom.Value);
                if (filters.EndDateTo.HasValue) query = query.Where(d => d.EndDate <= filters.EndDateTo.Value);

                if (filters.HasEndDate.HasValue)
                {
                    query = filters.HasEndDate.Value
                        ? query.Where(d => d.EndDate.HasValue)
                        : query.Where(d => !d.EndDate.HasValue);
                }

                if (filters.MinCalories.HasValue) query = query.Where(d => d.DailyCalories >= filters.MinCalories.Value);
                if (filters.MaxCalories.HasValue) query = query.Where(d => d.DailyCalories <= filters.MaxCalories.Value);
            }

            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var diets = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(MapToDietResponse)
                .ToList();

            return new DietsPagedDTO
            {
                Diets = diets,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages
            };
        }

        public async Task<DietResponse?> GetDietByIdAsync(int id)
        {
            var diet = (await _dietRepository.GetAll()).FirstOrDefault(d => d.Id == id);
            return diet != null ? MapToDietResponse(diet) : null;
        }

        public async Task<DietResponse> CreateDietAsync(CreateDietRequest request)
        {
            var diet = new Diet
            {
                Name = request.Name,
                Description = request.Description,
                ClientId = request.ClientId,
                EmpresaId = request.EmpresaId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = request.Status,
                DailyCalories = request.DailyCalories,
                DailyProtein = request.DailyProtein,
                DailyCarbs = request.DailyCarbs,
                DailyFat = request.DailyFat,
                DailyFiber = request.DailyFiber,
                DailySodium = request.DailySodium,
                Restrictions = request.Restrictions,
                Notes = request.Notes
            };

            _dietRepository.Add(diet);
            await _unitOfWork.SaveAsync();

            var created = (await _dietRepository.GetAll()).First(d => d.Id == diet.Id);
            return MapToDietResponse(created);
        }

        public async Task<DietResponse?> UpdateDietAsync(int id, UpdateDietRequest request)
        {
            var diet = (await _dietRepository.GetAll()).FirstOrDefault(d => d.Id == id);
            if (diet == null) return null;

            if (!string.IsNullOrEmpty(request.Name)) diet.Name = request.Name;
            if (request.Description != null) diet.Description = request.Description;
            if (request.ClientId.HasValue) diet.ClientId = request.ClientId.Value;
            if (request.EmpresaId.HasValue) diet.EmpresaId = request.EmpresaId.Value;
            if (request.StartDate.HasValue) diet.StartDate = request.StartDate.Value;
            if (request.EndDate.HasValue) diet.EndDate = request.EndDate.Value;
            if (request.Status.HasValue) diet.Status = request.Status.Value;
            if (request.DailyCalories.HasValue) diet.DailyCalories = request.DailyCalories.Value;
            if (request.DailyProtein.HasValue) diet.DailyProtein = request.DailyProtein.Value;
            if (request.DailyCarbs.HasValue) diet.DailyCarbs = request.DailyCarbs.Value;
            if (request.DailyFat.HasValue) diet.DailyFat = request.DailyFat.Value;
            if (request.DailyFiber.HasValue) diet.DailyFiber = request.DailyFiber.Value;
            if (request.DailySodium.HasValue) diet.DailySodium = request.DailySodium.Value;
            if (request.Restrictions != null) diet.Restrictions = request.Restrictions;
            if (request.Notes != null) diet.Notes = request.Notes;

            diet.UpdatedDate = DateTime.UtcNow;

            _dietRepository.Update(diet);
            await _unitOfWork.SaveAsync();

            var updated = (await _dietRepository.GetAll()).First(d => d.Id == id);
            return MapToDietResponse(updated);
        }

        public async Task<bool> DeleteDietAsync(int id)
        {
            var diet = (await _dietRepository.GetAll()).FirstOrDefault(d => d.Id == id);
            if (diet == null) return false;

            _dietRepository.Delete(diet);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<DietStatsDTO> GetDietStatsAsync()
        {
            var allDiets = (await _dietRepository.GetAll()).ToList();
            var totalDiets = allDiets.Count;

            if (totalDiets == 0)
            {
                return new DietStatsDTO();
            }

            var activeDiets = allDiets.Count(d => d.Status == DietStatus.Active);
            var completedDiets = allDiets.Count(d => d.Status == DietStatus.Completed);
            var pausedDiets = allDiets.Count(d => d.Status == DietStatus.Paused);
            var cancelledDiets = allDiets.Count(d => d.Status == DietStatus.Cancelled);

            var totalMeals = allDiets.SelectMany(d => d.Meals ?? Enumerable.Empty<DietMeal>()).Count();
            var completedMeals = allDiets.SelectMany(d => d.Meals ?? Enumerable.Empty<DietMeal>()).Count(m => m.IsCompleted);

            var now = DateTime.UtcNow;
            var currentMonth = now.Month;
            var currentYear = now.Year;
            var lastMonth = currentMonth == 1 ? 12 : currentMonth - 1;
            var lastMonthYear = currentMonth == 1 ? currentYear - 1 : currentYear;

            var dietsThisMonth = allDiets.Count(d => d.CreatedDate.Month == currentMonth && d.CreatedDate.Year == currentYear);
            var dietsLastMonth = allDiets.Count(d => d.CreatedDate.Month == lastMonth && d.CreatedDate.Year == lastMonthYear);

            var monthlyGrowth = dietsLastMonth > 0 ? ((double)(dietsThisMonth - dietsLastMonth) / dietsLastMonth) * 100 : 0.0;

            var avgCalories = allDiets.Where(d => d.DailyCalories.HasValue).DefaultIfEmpty().Average(d => d?.DailyCalories ?? 0);
            var avgMeals = totalDiets > 0 ? (double)totalMeals / totalDiets : 0.0;
            var avgCompletion = totalMeals > 0 ? ((double)completedMeals / totalMeals) * 100 : 0.0;

            var progressEntries = allDiets.SelectMany(d => d.Progress ?? Enumerable.Empty<DietProgress>()).ToList();
            var avgWeightLoss = progressEntries.Where(p => p.Weight.HasValue)
                .GroupBy(p => p.DietId)
                .Select(g =>
                {
                    var weights = g.Where(p => p.Weight.HasValue).OrderBy(p => p.Date).ToList();
                    if (weights.Count < 2) return 0.0;
                    return weights.First().Weight!.Value - weights.Last().Weight!.Value;
                })
                .Where(w => w > 0)
                .DefaultIfEmpty(0.0)
                .Average();

            var avgEnergy = progressEntries.Where(p => p.EnergyLevel.HasValue).DefaultIfEmpty().Average(p => p?.EnergyLevel ?? 0.0);
            var avgSatisfaction = progressEntries.Where(p => p.SatisfactionLevel.HasValue).DefaultIfEmpty().Average(p => p?.SatisfactionLevel ?? 0.0);

            return new DietStatsDTO
            {
                TotalDiets = totalDiets,
                ActiveDiets = activeDiets,
                CompletedDiets = completedDiets,
                PausedDiets = pausedDiets,
                CancelledDiets = cancelledDiets,
                ActiveDietsPercentage = (double)activeDiets / totalDiets * 100,
                CompletedDietsPercentage = (double)completedDiets / totalDiets * 100,
                PausedDietsPercentage = (double)pausedDiets / totalDiets * 100,
                CancelledDietsPercentage = (double)cancelledDiets / totalDiets * 100,
                AverageCaloriesPerDiet = avgCalories,
                AverageMealsPerDiet = avgMeals,
                AverageCompletionRate = avgCompletion,
                TotalMeals = totalMeals,
                CompletedMeals = completedMeals,
                MealCompletionPercentage = totalMeals > 0 ? ((double)completedMeals / totalMeals) * 100 : 0.0,
                AverageWeightLoss = avgWeightLoss,
                AverageEnergyLevel = avgEnergy,
                AverageSatisfactionLevel = avgSatisfaction,
                DietsThisMonth = dietsThisMonth,
                DietsLastMonth = dietsLastMonth,
                MonthlyGrowth = monthlyGrowth
            };
        }

        // ===== Meals =====
        
        public async Task<List<DietMealResponse>> GetDietMealsAsync(int dietId)
        {
            var diet = await (_dietRepository as Infrastructure.Repositories.DietRepository)!.GetByIdWithMealsAsync(dietId);
            if (diet == null) return new List<DietMealResponse>();
            return (diet.Meals ?? new List<DietMeal>()).Select(MapToDietMealResponse).ToList();
        }
    

        public async Task<DietMealResponse> CreateDietMealAsync(int dietId, CreateDietMealRequest request)
        {
            var diet = (await _dietRepository.GetAll()).FirstOrDefault(d => d.Id == dietId);
            if (diet == null) throw new InvalidOperationException("Dieta não encontrada.");

            diet.Meals ??= new List<DietMeal>();

            var meal = new DietMeal
            {
                DietId = dietId,
                Name = request.Name,
                Type = request.Type,
                ScheduledTime = request.ScheduledTime,
                Instructions = request.Instructions,
                Foods = new List<DietMealFood>()
            };

            double totalCalories = 0, totalProtein = 0, totalCarbs = 0, totalFat = 0;

            var allFoods = await _foodRepository.GetAll();

            foreach (var foodRequest in request.Foods)
            {
                var food = allFoods.FirstOrDefault(f => f.Id == foodRequest.FoodId);
                if (food != null)
                {
                    var multiplier = foodRequest.Quantity / 100.0;

                    var mealFood = new DietMealFood
                    {
                        FoodId = foodRequest.FoodId,
                        Quantity = foodRequest.Quantity,
                        Unit = foodRequest.Unit,
                        Calories = food.CaloriesPer100g * multiplier,
                        Protein = food.ProteinPer100g * multiplier,
                        Carbs = food.CarbsPer100g * multiplier,
                        Fat = food.FatPer100g * multiplier
                    };

                    meal.Foods.Add(mealFood);

                    totalCalories += mealFood.Calories ?? 0;
                    totalProtein += mealFood.Protein ?? 0;
                    totalCarbs += mealFood.Carbs ?? 0;
                    totalFat += mealFood.Fat ?? 0;
                }
            }

            meal.TotalCalories = totalCalories;
            meal.TotalProtein = totalProtein;
            meal.TotalCarbs = totalCarbs;
            meal.TotalFat = totalFat;

            diet.Meals.Add(meal);

            _dietRepository.Update(diet);
            await _unitOfWork.SaveAsync();

            return MapToDietMealResponse(meal);
        }

        public async Task<DietMealResponse?> UpdateDietMealAsync(int mealId, UpdateDietMealRequest request)
        {
            var diet = (await _dietRepository.GetAll()).FirstOrDefault(d => (d.Meals ?? new List<DietMeal>()).Any(m => m.Id == mealId));
            if (diet == null) return null;

            var meal = diet.Meals!.First(m => m.Id == mealId);

            if (!string.IsNullOrEmpty(request.Name)) meal.Name = request.Name;
            if (request.Type.HasValue) meal.Type = request.Type.Value;
            if (request.ScheduledTime.HasValue) meal.ScheduledTime = request.ScheduledTime.Value;
            if (request.Instructions != null) meal.Instructions = request.Instructions;

            if (request.IsCompleted.HasValue)
            {
                meal.IsCompleted = request.IsCompleted.Value;
                if (request.IsCompleted.Value && !meal.CompletedAt.HasValue)
                    meal.CompletedAt = DateTime.UtcNow;
                else if (!request.IsCompleted.Value)
                    meal.CompletedAt = null;
            }

            if (request.Foods != null)
            {
                meal.Foods.Clear();

                double totalCalories = 0, totalProtein = 0, totalCarbs = 0, totalFat = 0;
                var allFoods = await _foodRepository.GetAll();

                foreach (var foodRequest in request.Foods)
                {
                    var food = allFoods.FirstOrDefault(f => f.Id == foodRequest.FoodId);
                    if (food != null)
                    {
                        var multiplier = foodRequest.Quantity / 100.0;

                        var mealFood = new DietMealFood
                        {
                            FoodId = foodRequest.FoodId,
                            Quantity = foodRequest.Quantity,
                            Unit = foodRequest.Unit,
                            Calories = food.CaloriesPer100g * multiplier,
                            Protein = food.ProteinPer100g * multiplier,
                            Carbs = food.CarbsPer100g * multiplier,
                            Fat = food.FatPer100g * multiplier
                        };

                        meal.Foods.Add(mealFood);

                        totalCalories += mealFood.Calories ?? 0;
                        totalProtein += mealFood.Protein ?? 0;
                        totalCarbs += mealFood.Carbs ?? 0;
                        totalFat += mealFood.Fat ?? 0;
                    }
                }

                meal.TotalCalories = totalCalories;
                meal.TotalProtein = totalProtein;
                meal.TotalCarbs = totalCarbs;
                meal.TotalFat = totalFat;
            }

            meal.UpdatedDate = DateTime.UtcNow;

            _dietRepository.Update(diet);
            await _unitOfWork.SaveAsync();

            return MapToDietMealResponse(meal);
        }

        public async Task<bool> DeleteDietMealAsync(int mealId)
        {
            var diet = (await _dietRepository.GetAll()).FirstOrDefault(d => (d.Meals ?? new List<DietMeal>()).Any(m => m.Id == mealId));
            if (diet == null) return false;

            var meal = diet.Meals!.First(m => m.Id == mealId);
            diet.Meals!.Remove(meal);

            _dietRepository.Update(diet);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> CompleteMealAsync(int mealId)
        {
            var diet = (await _dietRepository.GetAll()).FirstOrDefault(d => (d.Meals ?? new List<DietMeal>()).Any(m => m.Id == mealId));
            if (diet == null) return false;

            var meal = diet.Meals!.First(m => m.Id == mealId);
            meal.IsCompleted = true;
            meal.CompletedAt = DateTime.UtcNow;
            meal.UpdatedDate = DateTime.UtcNow;

            _dietRepository.Update(diet);
            await _unitOfWork.SaveAsync();
            return true;
        }

        // ===== Foods =====
        public async Task<List<FoodResponse>> GetAllFoodsAsync(string? search = null, FoodCategory? category = null)
        {
            var foods = await _foodRepository.GetAll();
            var query = foods.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(f => f.Name.Contains(search) || (f.Description != null && f.Description.Contains(search)));

            if (category.HasValue)
                query = query.Where(f => f.Category == category.Value);

            return query.Select(MapToFoodResponse).ToList();
        }

        public async Task<FoodResponse?> GetFoodByIdAsync(int id)
        {
            var foods = await _foodRepository.GetAll();
            var food = foods.FirstOrDefault(f => f.Id == id);
            return food != null ? MapToFoodResponse(food) : null;
        }

        public async Task<FoodResponse> CreateFoodAsync(CreateFoodRequest request)
        {
            var food = new Food
            {
                Name = request.Name,
                Description = request.Description,
                Category = request.Category,
                CaloriesPer100g = request.CaloriesPer100g,
                ProteinPer100g = request.ProteinPer100g,
                CarbsPer100g = request.CarbsPer100g,
                FatPer100g = request.FatPer100g,
                FiberPer100g = request.FiberPer100g,
                SodiumPer100g = request.SodiumPer100g,
                Allergens = request.Allergens,
                CommonPortions = request.CommonPortions,
                IsActive = request.IsActive
            };

            _foodRepository.Add(food);
            await _unitOfWork.SaveAsync();

            return MapToFoodResponse(food);
        }

        public async Task<FoodResponse?> UpdateFoodAsync(int id, UpdateFoodRequest request)
        {
            var foods = await _foodRepository.GetAll();
            var food = foods.FirstOrDefault(f => f.Id == id);
            if (food == null) return null;

            if (!string.IsNullOrEmpty(request.Name)) food.Name = request.Name;
            if (request.Description != null) food.Description = request.Description;
            if (request.Category.HasValue) food.Category = request.Category.Value;
            if (request.CaloriesPer100g.HasValue) food.CaloriesPer100g = request.CaloriesPer100g.Value;
            if (request.ProteinPer100g.HasValue) food.ProteinPer100g = request.ProteinPer100g.Value;
            if (request.CarbsPer100g.HasValue) food.CarbsPer100g = request.CarbsPer100g.Value;
            if (request.FatPer100g.HasValue) food.FatPer100g = request.FatPer100g.Value;
            if (request.FiberPer100g.HasValue) food.FiberPer100g = request.FiberPer100g.Value;
            if (request.SodiumPer100g.HasValue) food.SodiumPer100g = request.SodiumPer100g.Value;
            if (request.Allergens != null) food.Allergens = request.Allergens;
            if (request.CommonPortions != null) food.CommonPortions = request.CommonPortions;
            if (request.IsActive.HasValue) food.IsActive = request.IsActive.Value;

            food.UpdatedDate = DateTime.UtcNow;

            _foodRepository.Update(food);
            await _unitOfWork.SaveAsync();

            return MapToFoodResponse(food);
        }

        public async Task<bool> DeleteFoodAsync(int id)
        {
            var foods = await _foodRepository.GetAll();
            var food = foods.FirstOrDefault(f => f.Id == id);
            if (food == null) return false;

            food.IsActive = false;
            food.UpdatedDate = DateTime.UtcNow;

            _foodRepository.Update(food);
            await _unitOfWork.SaveAsync();
            return true;
        }

        // ===== Progress =====
        public async Task<List<DietProgressResponse>> GetDietProgressAsync(int dietId)
        {
            var diet = await _dietRepository.GetByIdWithProgressAsync(dietId);
            if (diet == null) return new List<DietProgressResponse>();

            var progress = (diet.Progress ?? new List<DietProgress>())
                .OrderByDescending(p => p.Date)
                .Select(MapToDietProgressResponse)
                .ToList();

            return progress;
        }

        public async Task<DietProgressResponse> CreateDietProgressAsync(int dietId, CreateDietProgressRequest request)
        {
            var diet = (await _dietRepository.GetAll()).FirstOrDefault(d => d.Id == dietId);
            if (diet == null) throw new InvalidOperationException("Dieta não encontrada.");

            diet.Progress ??= new List<DietProgress>();

            var progress = new DietProgress
            {
                DietId = dietId,
                Date = request.Date,
                Weight = request.Weight,
                CaloriesConsumed = request.CaloriesConsumed,
                MealsCompleted = request.MealsCompleted,
                TotalMeals = request.TotalMeals,
                Notes = request.Notes,
                EnergyLevel = request.EnergyLevel,
                HungerLevel = request.HungerLevel,
                SatisfactionLevel = request.SatisfactionLevel
            };

            diet.Progress.Add(progress);

            _dietRepository.Update(diet);
            await _unitOfWork.SaveAsync();

            return MapToDietProgressResponse(progress);
        }

        // ===== Mapping =====
        private DietResponse MapToDietResponse(Diet diet)
        {
            var meals = diet.Meals ?? new List<DietMeal>();
            var completedMeals = meals.Count(m => m.IsCompleted);
            var totalMeals = meals.Count;

            return new DietResponse
            {
                Id = diet.Id,
                Name = diet.Name,
                Description = diet.Description,
                ClientId = diet.ClientId,
                ClientName = diet.Client?.Name ?? "",
                EmpresaId = diet.EmpresaId,
                EmpresaName = diet.Empresa?.Name ?? "",
                StartDate = diet.StartDate,
                EndDate = diet.EndDate,
                Status = diet.Status,
                StatusDescription = GetStatusDescription(diet.Status),
                DailyCalories = diet.DailyCalories,
                DailyProtein = diet.DailyProtein,
                DailyCarbs = diet.DailyCarbs,
                DailyFat = diet.DailyFat,
                DailyFiber = diet.DailyFiber,
                DailySodium = diet.DailySodium,
                Restrictions = diet.Restrictions,
                Notes = diet.Notes,
                TotalMeals = totalMeals,
                CompletedMeals = completedMeals,
                CompletionPercentage = totalMeals > 0 ? (double)completedMeals / totalMeals * 100 : 0,
                CreatedAt = diet.CreatedDate,
                UpdatedAt = diet.UpdatedDate,
                Meals = meals.Select(MapToDietMealResponse).ToList()
            };
        }

        private DietMealResponse MapToDietMealResponse(DietMeal meal)
        {
            return new DietMealResponse
            {
                Id = meal.Id,
                DietId = meal.DietId,
                Name = meal.Name,
                Type = meal.Type,
                TypeDescription = GetMealTypeDescription(meal.Type),
                ScheduledTime = meal.ScheduledTime,
                Instructions = meal.Instructions,
                TotalCalories = meal.TotalCalories,
                TotalProtein = meal.TotalProtein,
                TotalCarbs = meal.TotalCarbs,
                TotalFat = meal.TotalFat,
                IsCompleted = meal.IsCompleted,
                CompletedAt = meal.CompletedAt,
                CreatedAt = meal.CreatedDate,
                UpdatedAt = meal.UpdatedDate,
                Foods = (meal.Foods ?? new List<DietMealFood>()).Select(MapToDietMealFoodResponse).ToList()
            };
        }

        private DietMealFoodResponse MapToDietMealFoodResponse(DietMealFood mealFood)
        {
            return new DietMealFoodResponse
            {
                Id = mealFood.Id,
                MealId = mealFood.MealId,
                FoodId = mealFood.FoodId,
                FoodName = mealFood.Food?.Name ?? "",
                Quantity = mealFood.Quantity,
                Unit = mealFood.Unit,
                Calories = mealFood.Calories,
                Protein = mealFood.Protein,
                Carbs = mealFood.Carbs,
                Fat = mealFood.Fat
            };
        }

        private FoodResponse MapToFoodResponse(Food food)
        {
            return new FoodResponse
            {
                Id = food.Id,
                Name = food.Name,
                Description = food.Description,
                Category = food.Category,
                CategoryDescription = GetFoodCategoryDescription(food.Category),
                CaloriesPer100g = food.CaloriesPer100g,
                ProteinPer100g = food.ProteinPer100g,
                CarbsPer100g = food.CarbsPer100g,
                FatPer100g = food.FatPer100g,
                FiberPer100g = food.FiberPer100g,
                SodiumPer100g = food.SodiumPer100g,
                Allergens = food.Allergens,
                CommonPortions = food.CommonPortions,
                IsActive = food.IsActive,
                CreatedAt = food.CreatedDate,
                UpdatedAt = food.UpdatedDate
            };
        }

        private DietProgressResponse MapToDietProgressResponse(DietProgress progress)
        {
            return new DietProgressResponse
            {
                Id = progress.Id,
                DietId = progress.DietId,
                Date = progress.Date,
                Weight = progress.Weight,
                CaloriesConsumed = progress.CaloriesConsumed,
                MealsCompleted = progress.MealsCompleted,
                TotalMeals = progress.TotalMeals,
                CompletionPercentage = progress.TotalMeals > 0 ? (double)progress.MealsCompleted / progress.TotalMeals * 100 : 0,
                Notes = progress.Notes,
                EnergyLevel = progress.EnergyLevel,
                HungerLevel = progress.HungerLevel,
                SatisfactionLevel = progress.SatisfactionLevel,
                CreatedAt = progress.CreatedDate,
                UpdatedAt = progress.UpdatedDate
            };
        }

        private string GetStatusDescription(DietStatus status) => status switch
        {
            DietStatus.Draft => "Rascunho",
            DietStatus.Active => "Ativa",
            DietStatus.Paused => "Pausada",
            DietStatus.Completed => "Concluída",
            DietStatus.Cancelled => "Cancelada",
            _ => "Desconhecido"
        };

        private string GetMealTypeDescription(MealType type) => type switch
        {
            MealType.Breakfast => "Café da Manhã",
            MealType.MorningSnack => "Lanche da Manhã",
            MealType.Lunch => "Almoço",
            MealType.AfternoonSnack => "Lanche da Tarde",
            MealType.Dinner => "Jantar",
            MealType.EveningSnack => "Ceia",
            _ => "Outro"
        };

        private string GetFoodCategoryDescription(FoodCategory category) => category switch
        {
            FoodCategory.Fruits => "Frutas",
            FoodCategory.Vegetables => "Vegetais",
            FoodCategory.Proteins => "Proteínas",
            FoodCategory.Dairy => "Laticínios",
            FoodCategory.Fats => "Gorduras",
            FoodCategory.Beverages => "Bebidas",
            FoodCategory.Others => "Outros",
            _ => category.ToString()
        };
    }
}
