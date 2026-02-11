// File: Services/PlanService.cs
using System.Linq;
using System.Threading.Tasks;
using Core.DTO;
using Core.DTO.Plan;
using Core.Enums;
using Core.Models.Plan;
using Infrastructure.Repositories;
using Saller.Infrastructure.ServiceExtension; // para PagedResult<T>

namespace Services
{
    public interface IPlanService
    {
        Task<PlanDTO?> CreatePlanAsync(CreatePlanRequest request);
        Task<PlanDTO?> GetPlanByIdAsync(int id, int? empresaId = null);
        Task<PagedResult<PlanDTO>> GetPlansPagedAsync(int page, int pageSize, int? empresaId = null);
        Task<bool> UpdatePlanAsync(int id, UpdatePlanRequest request);
        Task<bool> DeletePlanAsync(int id);
    }

    public sealed class PlanService : IPlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlanRepository _planRepo;

        public PlanService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _planRepo = unitOfWork.Plans;
        }

        public async Task<PlanDTO?> CreatePlanAsync(CreatePlanRequest request)
        {
            var entity = new Plan
            {
                Name = request.Name,
                Description = request.Description,
                Type = request.Type,
                Duration = request.Duration,
                TargetCalories = request.TargetCalories,
                TargetWeight = request.TargetWeight,
                Status = PlanStatus.Draft,
                ClientId = request.ClientId,
                NutritionistId = request.NutritionistId,
                StartDate = request.StartDate,
                // EndDate foi removido do CreatePlanRequest, logo não podemos atribuí-lo aqui
                Notes = request.Notes,
                IsActive = false
            };

            _planRepo.Add(entity);
            var saved = await _unitOfWork.SaveAsync();
            if (saved <= 0) return null;

            return await GetPlanByIdAsync(entity.Id);
        }

        public async Task<PlanDTO?> GetPlanByIdAsync(int id, int? empresaId = null)
        {
            var p = await _planRepo.GetByIdAsync(id);
            if (p == null) return null;

            if (empresaId.HasValue)
            {
                var client = await _unitOfWork.Clients.GetByIdAsync(p.ClientId);
                if (client == null || (client.EmpresaId ?? 0) != empresaId.Value)
                    return null;
            }

            return new PlanDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Type = p.Type,
                Duration = p.Duration,
                TargetCalories = p.TargetCalories,
                TargetWeight = p.TargetWeight,
                Status = p.Status,
                ClientId = p.ClientId,
                NutritionistId = p.NutritionistId,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Notes = p.Notes,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedDate,
                UpdatedAt = p.UpdatedDate,

                Goals = p.Goals?.Select(g => new PlanGoalDTO
                {
                    Id = g.Id,
                    Type = g.Type,
                    Target = g.Target,
                    Current = g.Current,
                    Unit = g.Unit,
                    Description = g.Description
                }).ToList() ?? new(),

                Meals = p.Meals?.Select(m => new PlanMealDTO
                {
                    Id = m.Id,
                    Name = m.Name,
                    Type = m.Type,
                    Time = m.Time,
                    Calories = m.Calories,
                    Macros = new MacroNutrientsDTO
                    {
                        Carbs = m.Macros.Carbs,
                        Protein = m.Macros.Protein,
                        Fat = m.Macros.Fat,
                        Fiber = m.Macros.Fiber,
                        Sugar = m.Macros.Sugar,
                        Sodium = m.Macros.Sodium
                    },
                    Foods = m.Foods?.Select(f => new PlanFoodDTO
                    {
                        Id = f.Id,
                        Name = f.Name,
                        Quantity = f.Quantity,
                        Unit = f.Unit,
                        Calories = f.Calories,
                        Macros = new MacroNutrientsDTO
                        {
                            Carbs = f.Macros.Carbs,
                            Protein = f.Macros.Protein,
                            Fat = f.Macros.Fat,
                            Fiber = f.Macros.Fiber,
                            Sugar = f.Macros.Sugar,
                            Sodium = f.Macros.Sodium
                        },
                        Category = f.Category
                    }).ToList() ?? new(),
                    Instructions = m.Instructions,
                    IsCompleted = m.IsCompleted
                }).ToList() ?? new(),

                ProgressEntries = p.ProgressEntries?.Select(pr => new PlanProgressDTO
                {
                    Id = pr.Id,
                    Date = pr.Date,
                    Weight = pr.Weight,
                    Calories = pr.Calories,
                    MealsCompleted = pr.MealsCompleted,
                    TotalMeals = pr.TotalMeals,
                    Notes = pr.Notes,
                    Photos = pr.Photos?.ToList() ?? new()
                }).ToList() ?? new()
            };
        }

        public async Task<PagedResult<PlanDTO>> GetPlansPagedAsync(int page, int pageSize, int? empresaId = null)
        {
            if (!empresaId.HasValue)
            {
                var paged = await _planRepo.GetAllPagedAsync(page, pageSize);

                return new PagedResult<PlanDTO>
                {
                    CurrentPage = paged.CurrentPage,
                    PageSize = paged.PageSize,
                    RowCount = paged.RowCount,
                    PageCount = paged.PageCount,
                    Results = paged.Results.Select(p => new PlanDTO
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Type = p.Type,
                        Duration = p.Duration,
                        TargetCalories = p.TargetCalories,
                        TargetWeight = p.TargetWeight,
                        Status = p.Status,
                        ClientId = p.ClientId,
                        NutritionistId = p.NutritionistId,
                        StartDate = p.StartDate,
                        EndDate = p.EndDate,
                        Notes = p.Notes,
                        IsActive = p.IsActive,
                        CreatedAt = p.CreatedDate,
                        UpdatedAt = p.UpdatedDate
                    }).ToList()
                };
            }

            // Plan não possui EmpresaId no schema atual. Então filtramos por EmpresaId do Client.
            var clients = (await _unitOfWork.Clients.GetAll()).ToList();
            var allowedClientIds = clients
                .Where(c => (c.EmpresaId ?? 0) == empresaId.Value)
                .Select(c => c.Id)
                .ToHashSet();

            var allPlans = (await _planRepo.GetAll()).ToList();
            var filtered = allPlans.Where(p => allowedClientIds.Contains(p.ClientId)).ToList();

            var rowCount = filtered.Count;
            var pageCount = (int)Math.Ceiling((double)rowCount / pageSize);
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            var pageItems = filtered
                .OrderByDescending(p => p.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PlanDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Type = p.Type,
                    Duration = p.Duration,
                    TargetCalories = p.TargetCalories,
                    TargetWeight = p.TargetWeight,
                    Status = p.Status,
                    ClientId = p.ClientId,
                    NutritionistId = p.NutritionistId,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Notes = p.Notes,
                    IsActive = p.IsActive,
                    CreatedAt = p.CreatedDate,
                    UpdatedAt = p.UpdatedDate
                })
                .ToList();

            return new PagedResult<PlanDTO>
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = rowCount,
                PageCount = pageCount,
                Results = pageItems
            };
        }

        public async Task<bool> UpdatePlanAsync(int id, UpdatePlanRequest request)
        {
            var p = await _planRepo.GetByIdAsync(id);
            if (p == null) return false;

            if (request.Name != null) p.Name = request.Name;
            if (request.Description != null) p.Description = request.Description;
            if (request.Type.HasValue) p.Type = request.Type.Value;
            if (request.Duration.HasValue) p.Duration = request.Duration.Value;
            if (request.TargetCalories.HasValue) p.TargetCalories = request.TargetCalories.Value;
            if (request.TargetWeight != null) p.TargetWeight = request.TargetWeight;
            if (request.Status.HasValue) p.Status = request.Status.Value;
            if (request.StartDate != null) p.StartDate = request.StartDate;
            if (request.EndDate != null) p.EndDate = request.EndDate;
            if (request.Notes != null) p.Notes = request.Notes;
            if (request.IsActive.HasValue) p.IsActive = request.IsActive.Value;

            _planRepo.Update(p);
            var saved = await _unitOfWork.SaveAsync();
            return saved > 0;
        }

        public async Task<bool> DeletePlanAsync(int id)
        {
            var p = await _planRepo.GetByIdAsync(id);
            if (p == null) return false;

            _planRepo.Delete(p);
            var saved = await _unitOfWork.SaveAsync();
            return saved > 0;
        }
    }
}
