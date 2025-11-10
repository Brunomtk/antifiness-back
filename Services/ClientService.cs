using Core.DTO.Client;
using Core.DTO.Diet;
using Core.DTO.Workout;
using Core.Enums;
using Core.Models;
using Core.Models.Client;
using Infrastructure.Repositories;
using Saller.Infrastructure.ServiceExtension;
using System;
using System.Threading;
// removed: using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public interface IClientService
    {
        Task<ClientsPagedDTO> GetClientsPagedAsync(ClientFiltersDTO filters);
        Task<ClientResponse?> GetClientByIdAsync(int id);
        Task<ClientResponse?> CreateClientAsync(CreateClientRequest req);
        Task<bool> UpdateClientAsync(int id, UpdateClientRequest req);
        Task<bool> DeleteClientAsync(int id);

        Task<ClientMeasurementDTO?> AddWeightProgressAsync(int clientId, AddWeightProgressRequest request);
        Task<ClientMeasurementDTO?> AddMeasurementsProgressAsync(int clientId, AddMeasurementsProgressRequest request);
        Task<object?> AddPhotoProgressAsync(int clientId, AddPhotoProgressRequest request);

        Task<DietSummaryDTO?> GetCurrentDietAsync(int clientId);
        Task<WorkoutSummaryDTO?> GetCurrentWorkoutAsync(int clientId);

        Task<ClientBasicDTO?> GetClientBasicByIdAsync(int id);
        Task<IEnumerable<DietSummaryDTO>> GetDietHistoryAsync(int clientId);
        Task<IEnumerable<WorkoutSummaryDTO>> GetWorkoutHistoryAsync(int clientId);

        Task<AchievementDTO?> AddAchievementAsync(int clientId, AddAchievementRequest request);
        Task<IReadOnlyList<AchievementDTO>> GetAchievementsAsync(int clientId, int page = 1, int pageSize = 20);
        Task<AchievementDTO?> UpdateAchievementAsync(int clientId, int achievementId, UpdateAchievementRequest request);
        Task<bool> DeleteAchievementAsync(int clientId, int achievementId);

        Task<ClientStatsDTO> GetClientStatsAsync();
    }

public sealed class ClientService : IClientService
    {
        private readonly IUnitOfWork _uow;
        private readonly IClientRepository _clients;
        private readonly DietRepository _diets;
        private readonly WorkoutRepository _workouts;

        public ClientService(IUnitOfWork uow, DietRepository diets, WorkoutRepository workouts)
        {
            _uow = uow;
            _clients = uow.Clients;
            _diets = diets;
            _workouts = workouts;
        }

        public async Task<ClientResponse?> CreateClientAsync(CreateClientRequest req)
        {
            // Verificar se email já existe
            var allClients = (await _clients.GetAll()).ToList();
            var existingClient = (!string.IsNullOrWhiteSpace(req.Email)) ? (await _clients.GetAll()).FirstOrDefault(c => (c.Email ?? string.Empty).ToLower() == req.Email!.ToLower()) : null;
            if (existingClient != null)
                return null;

            var entity = new Client
            {
                Name = string.IsNullOrWhiteSpace(req.Name) ? "Cliente" : req.Name!.Trim(),
                Email = string.IsNullOrWhiteSpace(req.Email) ? null : req.Email!.Trim().ToLowerInvariant(),
                Phone = string.IsNullOrWhiteSpace(req.Phone) ? null : req.Phone!.Trim(),
                Avatar = req.Avatar,
                DateOfBirth = req.DateOfBirth ?? new DateTime(2000,1,1,0,0,0,DateTimeKind.Utc),
                Gender = string.IsNullOrWhiteSpace(req.Gender) ? string.Empty : req.Gender,
                Height = req.Height ?? 0,
                CurrentWeight = req.CurrentWeight ?? 0.0,
                TargetWeight = req.TargetWeight ?? 0.0,
                ActivityLevel = req.ActivityLevel ?? ActivityLevel.Sedentary,
                KanbanStage = req.KanbanStage ?? CrmStage.Lead,
                EmpresaId = req.EmpresaId ?? 1,
                Goals = req.Goals?.Select(g => new ClientGoal
                {
                    Type = g.Type,
                    Description = string.IsNullOrWhiteSpace(g.Description) ? "" : g.Description.Trim(),
                    TargetValue = g.TargetValue,
                    TargetDate = g.TargetDate,
                    Priority = g.Priority,
                }).ToList(),
                Measurements = req.Measurements?.Select(m => new ClientMeasurement
                {
                    Date = m.Date == default ? DateTime.UtcNow : m.Date,
                    Weight = m.Weight ?? 0d,
                    BodyFat = m.BodyFat,
                    MuscleMass = m.MuscleMass,
                    Waist = m.Waist,
                    Chest = m.Chest,
                    Arms = m.Arms,
                    Thighs = m.Thighs,
                    Notes = m.Notes
                }).ToList() ?? new List<ClientMeasurement>(),
                Preferences = req.Preferences != null ? MapToClientPreferences(req.Preferences) : new ClientPreferences(),
                MedicalConditions = req.MedicalConditions,
                Allergies = req.Allergies
            };

            _clients.Add(entity);
            var saved = await _uow.SaveAsync();
            if (saved <= 0) return null;

            return await GetClientByIdAsync(entity.Id);
        }

        public async Task<ClientResponse?> GetClientByIdAsync(int id)
        {
            var client = await _clients.GetByIdAsync(id);
            if (client == null) return null;
            return MapToResponse(client);
        }

        public async Task<ClientsPagedDTO> GetClientsPagedAsync(ClientFiltersDTO filters)
        {
            var allClients = (await _clients.GetAll()).ToList();
            var filtered = allClients.AsQueryable();

            if (!string.IsNullOrEmpty(filters.Status))
            {
                if (Enum.TryParse<ClientStatus>(filters.Status, true, out var status))
                    filtered = filtered.Where(c => c.Status == status);
            }

            if (!string.IsNullOrEmpty(filters.ActivityLevel))
            {
                if (Enum.TryParse<ActivityLevel>(filters.ActivityLevel, true, out var activityLevel))
                    filtered = filtered.Where(c => c.ActivityLevel == activityLevel);
            }

            if (!string.IsNullOrEmpty(filters.PlanId))
                filtered = filtered.Where(c => c.PlanId == filters.PlanId);

            if (filters.EmpresaId.HasValue)
                filtered = filtered.Where(c => c.EmpresaId == filters.EmpresaId.Value);

            if (!string.IsNullOrEmpty(filters.Search))
            {
                var search = filters.Search.ToLower();
                filtered = filtered.Where(c =>
                    c.Name.ToLower().Contains(search) ||
                    c.Email.ToLower().Contains(search));
            }

            if (!string.IsNullOrEmpty(filters.KanbanStage))
            {
                if (Enum.TryParse<CrmStage>(filters.KanbanStage, true, out var kanbanStage))
                    filtered = filtered.Where(c => c.KanbanStage == kanbanStage);
            }

            // Ordenação
            filtered = filters.OrderBy?.ToLower() switch
            {
                "email" => filters.OrderDirection?.ToLower() == "desc"
                    ? filtered.OrderByDescending(c => c.Email)
                    : filtered.OrderBy(c => c.Email),
                "createdate" => filters.OrderDirection?.ToLower() == "desc"
                    ? filtered.OrderByDescending(c => c.CreatedDate)
                    : filtered.OrderBy(c => c.CreatedDate),
                _ => filters.OrderDirection?.ToLower() == "desc"
                    ? filtered.OrderByDescending(c => c.Name)
                    : filtered.OrderBy(c => c.Name)
            };

            var totalRecords = filtered.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / filters.PageSize);

            var results = filtered
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .Select(MapToResponse)
                .ToList();

            return new ClientsPagedDTO
            {
                Results = results,
                CurrentPage = filters.Page,
                PageSize = filters.PageSize,
                TotalPages = totalPages,
                TotalRecords = totalRecords,
                HasPreviousPage = filters.Page > 1,
                HasNextPage = filters.Page < totalPages
            };
        }

        public async Task<bool> UpdateClientAsync(int id, UpdateClientRequest req)
        {
            var client = await _clients.GetByIdAsync(id);
            if (client == null) return false;

            // Verificar email único se estiver sendo alterado
            if (!string.IsNullOrEmpty(req.Email) && req.Email != client.Email)
            {
                var allClients = (await _clients.GetAll()).ToList();
                var existingClient = allClients.FirstOrDefault(c => c.Email.ToLower() == req.Email.ToLower() && c.Id != id);
                if (existingClient != null)
                    return false;
            }

            // Atualizar campos
            if (req.Name != null) client.Name = req.Name;
            if (req.Email != null) client.Email = req.Email;
            if (req.Phone != null) client.Phone = req.Phone;
            if (req.Avatar != null) client.Avatar = req.Avatar;
            if (req.DateOfBirth != null) client.DateOfBirth = req.DateOfBirth.Value;
            if (req.Gender != null) client.Gender = req.Gender;
            if (req.Height != null) client.Height = req.Height.Value;
            if (req.CurrentWeight != null) client.CurrentWeight = req.CurrentWeight.Value;
            if (req.TargetWeight != null) client.TargetWeight = req.TargetWeight.Value;
            if (req.ActivityLevel != null) client.ActivityLevel = req.ActivityLevel.Value;
            if (req.KanbanStage != null) client.KanbanStage = req.KanbanStage.Value;
            if (req.Status != null) client.Status = req.Status.Value;
            if (req.PlanId != null) client.PlanId = req.PlanId;
            if (req.EmpresaId != null) client.EmpresaId = req.EmpresaId.Value;
            if (req.MedicalConditions != null) client.MedicalConditions = req.MedicalConditions.ToList();
            if (req.Allergies != null) client.Allergies = req.Allergies.ToList();

            _clients.Update(client);
            var saved = await _uow.SaveAsync();
            return saved > 0;
        }

        public async Task<bool> DeleteClientAsync(int id)
        {
            var client = await _clients.GetByIdAsync(id);
            if (client == null) return false;

            _clients.Delete(client);
            var saved = await _uow.SaveAsync();
            return saved > 0;
        }

        public async Task<ClientMeasurementDTO?> AddWeightProgressAsync(int clientId, AddWeightProgressRequest request)
        {
            var client = await _clients.GetByIdAsync(clientId);
            if (client == null) return null;

            var measurement = new ClientMeasurement
            {
                ClientId = clientId,
                Date = request.Date,
                Weight = request.Weight,
                Notes = request.Notes
            };

            client.Measurements ??= new List<ClientMeasurement>();
            client.Measurements.Add(measurement);

            _clients.Update(client);
            var saved = await _uow.SaveAsync();
            if (saved <= 0) return null;

            return new ClientMeasurementDTO
            {
                Id = measurement.Id,
                Date = measurement.Date,
                Weight = measurement.Weight,
                Notes = measurement.Notes
            };
        }

        public async Task<ClientMeasurementDTO?> AddMeasurementsProgressAsync(int clientId, AddMeasurementsProgressRequest request)
        {
            var client = await _clients.GetByIdAsync(clientId);
            if (client == null) return null;

            var measurement = new ClientMeasurement
            {
                ClientId = clientId,
                Date = request.Date,
                BodyFat = request.BodyFat,
                MuscleMass = request.MuscleMass,
                Waist = request.Waist,
                Chest = request.Chest,
                Arms = request.Arms,
                Thighs = request.Thighs,
                Notes = request.Notes
            };

            client.Measurements ??= new List<ClientMeasurement>();
            client.Measurements.Add(measurement);

            _clients.Update(client);
            var saved = await _uow.SaveAsync();
            if (saved <= 0) return null;

            return new ClientMeasurementDTO
            {
                Id = measurement.Id,
                Date = measurement.Date,
                BodyFat = measurement.BodyFat,
                MuscleMass = measurement.MuscleMass,
                Waist = measurement.Waist,
                Chest = measurement.Chest,
                Arms = measurement.Arms,
                Thighs = measurement.Thighs,
                Notes = measurement.Notes
            };
        }

        public async Task<object?> AddPhotoProgressAsync(int clientId, AddPhotoProgressRequest request)
        {
            var client = await _clients.GetByIdAsync(clientId);
            if (client == null) return null;

            // Simulação de adição de foto
            var photoProgress = new
            {
                Id = new Random().Next(1000, 9999),
                ClientId = clientId,
                Date = request.Date,
                Image = request.Image,
                Notes = request.Notes,
            };

            return photoProgress;
        }

        
        public async Task<AchievementDTO?> AddAchievementAsync(int clientId, AddAchievementRequest request)
        {
            var client = await _clients.GetByIdAsync(clientId);
            if (client == null) return null;

            var entity = new ClientAchievement
            {
                ClientId = clientId,
                Title = request.Title?.Trim() ?? string.Empty,
                Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description!.Trim(),
                Type = request.Type,
                Category = request.Category,
                UnlockedDate = DateTime.UtcNow,
            };

            await _uow.ClientAchievements.Add(entity);
            var saved = await _uow.SaveAsync();
            if (saved <= 0) return null;

            return new AchievementDTO
            {
                Id = entity.Id,
                ClientId = entity.ClientId,
                Title = entity.Title,
                Description = entity.Description,
                Type = entity.Type,
                Category = entity.Category,
                UnlockedDate = entity.UnlockedDate
            };
        }

        public async Task<IReadOnlyList<AchievementDTO>> GetAchievementsAsync(int clientId, int page = 1, int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            var all = await _uow.ClientAchievements.GetAll();
            var list = all.Cast<ClientAchievement>()
                          .Where(a => a.ClientId == clientId)
                          .OrderByDescending(a => a.UnlockedDate)
                          .Skip((page - 1) * pageSize)
                          .Take(pageSize)
                          .Select(a => new AchievementDTO
                          {
                              Id = a.Id,
                              ClientId = a.ClientId,
                              Title = a.Title,
                              Description = a.Description,
                              Type = a.Type,
                              Category = a.Category,
                              UnlockedDate = a.UnlockedDate
                          })
                          .ToList();
            return list;
        }

        public async Task<AchievementDTO?> UpdateAchievementAsync(int clientId, int achievementId, UpdateAchievementRequest request)
        {
            var all = await _uow.ClientAchievements.GetAll();
            var entity = all.Cast<ClientAchievement>().FirstOrDefault(a => a.Id == achievementId && a.ClientId == clientId);
            if (entity == null) return null;

            if (!string.IsNullOrWhiteSpace(request.Title)) entity.Title = request.Title!.Trim();
            if (!string.IsNullOrWhiteSpace(request.Description)) entity.Description = request.Description!.Trim();
            if (!string.IsNullOrWhiteSpace(request.Type)) entity.Type = request.Type;
            if (!string.IsNullOrWhiteSpace(request.Category)) entity.Category = request.Category;

            _uow.ClientAchievements.Update(entity);
            var saved = await _uow.SaveAsync();
            if (saved <= 0) return null;

            return new AchievementDTO
            {
                Id = entity.Id,
                ClientId = entity.ClientId,
                Title = entity.Title,
                Description = entity.Description,
                Type = entity.Type,
                Category = entity.Category,
                UnlockedDate = entity.UnlockedDate
            };
        }

        public async Task<bool> DeleteAchievementAsync(int clientId, int achievementId)
        {
            var all = await _uow.ClientAchievements.GetAll();
            var entity = all.Cast<ClientAchievement>().FirstOrDefault(a => a.Id == achievementId && a.ClientId == clientId);
            if (entity == null) return false;

            _uow.ClientAchievements.Delete(entity);
            return await _uow.SaveAsync() > 0;
        }


public async Task<DietSummaryDTO?> GetCurrentDietAsync(int clientId)
{
    var diet = await _diets.GetCurrentByClientIdAsync(clientId);
    if (diet == null) return null;
    return new DietSummaryDTO
    {
        Id = diet.Id,
        Name = diet.Name ?? string.Empty,
        Description = diet.Description,
        ClientId = diet.ClientId,
        EmpresaId = diet.EmpresaId,
        DailyCalories = diet.DailyCalories,
        DailyProtein = diet.DailyProtein,
        DailyCarbs = diet.DailyCarbs,
        DailyFat = diet.DailyFat,
    };
}


public async Task<WorkoutSummaryDTO?> GetCurrentWorkoutAsync(int clientId)
{
    var w = await _workouts.GetCurrentByClientIdAsync(clientId);
    if (w == null) return null;
    return new WorkoutSummaryDTO
    {
        Id = w.Id,
        Name = w.Name ?? string.Empty,
        Description = w.Description,
        EmpresaId = w.EmpresaId,
        ClientId = w.ClientId,
        Notes = w.Notes,
};
}


public async Task<ClientBasicDTO?> GetClientBasicByIdAsync(int id)
{
    var c = await _clients.GetByIdAsync(id);
    if (c == null) return null;
    return new ClientBasicDTO
    {
        Id = c.Id,
        Name = c.Name ?? string.Empty,
        Email = c.Email ?? string.Empty,
        Phone = c.Phone,
        Avatar = c.Avatar,
        DateOfBirth = c.DateOfBirth ?? DateTime.MinValue
    };
}


public async Task<IEnumerable<DietSummaryDTO>> GetDietHistoryAsync(int clientId)
{
    var list = await _diets.GetByClientIdAsync(clientId);
    return list.Select(d => new DietSummaryDTO
    {
        Id = d.Id,
        Name = d.Name ?? string.Empty,
        Description = d.Description,
        ClientId = d.ClientId,
        EmpresaId = d.EmpresaId,
        DailyCalories = d.DailyCalories,
        DailyProtein = d.DailyProtein,
        DailyCarbs = d.DailyCarbs,
        DailyFat = d.DailyFat,
    });
}


public async Task<IEnumerable<WorkoutSummaryDTO>> GetWorkoutHistoryAsync(int clientId)
{
    var list = await _workouts.GetByClientIdAsync(clientId);
    return list.Select(w => new WorkoutSummaryDTO
    {
        Id = w.Id,
        Name = w.Name ?? string.Empty,
        Description = w.Description,
        EmpresaId = w.EmpresaId,
        ClientId = w.ClientId,
        Notes = w.Notes,
});
}


public async Task<ClientStatsDTO> GetClientStatsAsync()
{
    var all = await _uow.Clients.GetAll();
    var clients = all.Cast<Client>().ToList();
    var total = clients.Count;
    var active = clients.Count(c => c.Status == ClientStatus.Active);
    var inactive = clients.Count(c => c.Status == ClientStatus.Inactive);
    var paused = clients.Count(c => c.Status == ClientStatus.Paused);
    var newThisMonth = clients.Count(c => c.CreatedDate.Month == DateTime.UtcNow.Month && c.CreatedDate.Year == DateTime.UtcNow.Year);
    var goalsAchieved = clients.Count(c => (c.Goals?.Any() ?? false));
    return new ClientStatsDTO
    {
        TotalClients = total,
        ActiveClients = active,
        InactiveClients = inactive,
        PausedClients = paused,
        NewClientsThisMonth = newThisMonth,
        ClientsWithGoalsAchieved = goalsAchieved
    };
}



private ClientPreferences MapToClientPreferences(ClientPreferencesDTO dto)
{
    if (dto == null) return new ClientPreferences();
    return new ClientPreferences
    {
        DietaryRestrictions = dto.DietaryRestrictions?.ToList() ?? new List<string>(),
        FavoriteFood = dto.FavoriteFood?.ToList() ?? new List<string>(),
        DislikedFood = dto.DislikedFood?.ToList() ?? new List<string>(),
        MealTimes = new MealTimes
        {
            Breakfast = dto.MealTimes?.Breakfast ?? string.Empty,
            Lunch = dto.MealTimes?.Lunch ?? string.Empty,
            Dinner = dto.MealTimes?.Dinner ?? string.Empty,
            Snacks = dto.MealTimes?.Snacks?.ToList() ?? new List<string>()
        },
        WorkoutPreferences = new WorkoutPreferences
        {
            Types = dto.WorkoutPreferences?.Types?.ToList() ?? new List<string>(),
            Duration = dto.WorkoutPreferences?.Duration ?? 0,
            Frequency = dto.WorkoutPreferences?.Frequency ?? 0,
            TimeOfDay = dto.WorkoutPreferences?.TimeOfDay ?? string.Empty
        }
    };
}


private ClientResponse MapToResponse(Client c)
{
    return new ClientResponse
    {
        Id = c.Id,
        Name = c.Name ?? string.Empty,
        Email = c.Email ?? string.Empty,
        Phone = c.Phone,
        Avatar = c.Avatar,
        DateOfBirth = c.DateOfBirth ?? DateTime.MinValue,
        Gender = c.Gender?.ToString() ?? string.Empty,
        Height = c.Height ?? 0,
        CurrentWeight = c.CurrentWeight ?? 0d,
        TargetWeight = c.TargetWeight ?? 0d,
        ActivityLevel = c.ActivityLevel,
        Status = c.Status,
        KanbanStage = c.KanbanStage,
        PlanId = c.PlanId,
        EmpresaId = c.EmpresaId ?? 0,
        Goals = c.Goals?.Select(g => new ClientGoalDTO
        {
            Id = g.Id,
            Type = g.Type,
            Description = g.Description,
            TargetValue = g.TargetValue,
            TargetDate = g.TargetDate,
            Priority = g.Priority,
            Status = g.Status,
        }).ToList(),
        Measurements = c.Measurements?.Select(m => new ClientMeasurementDTO
        {
            Date = m.Date,
            Weight = m.Weight,
            BodyFat = m.BodyFat,
            MuscleMass = m.MuscleMass,
            Waist = m.Waist,
            Chest = m.Chest,
            Arms = m.Arms,
            Thighs = m.Thighs,
            Notes = m.Notes
        }).ToList(),
        Preferences = new ClientPreferencesDTO
        {
            DietaryRestrictions = c.Preferences?.DietaryRestrictions?.ToList() ?? new List<string>(),
            FavoriteFood = c.Preferences?.FavoriteFood?.ToList() ?? new List<string>(),
            DislikedFood = c.Preferences?.DislikedFood?.ToList() ?? new List<string>(),
            MealTimes = new MealTimesDTO
            {
                Breakfast = c.Preferences?.MealTimes?.Breakfast ?? string.Empty,
                Lunch = c.Preferences?.MealTimes?.Lunch ?? string.Empty,
                Dinner = c.Preferences?.MealTimes?.Dinner ?? string.Empty,
                Snacks = c.Preferences?.MealTimes?.Snacks?.ToList() ?? new List<string>()
            },
            WorkoutPreferences = new WorkoutPreferencesDTO
            {
                Types = c.Preferences?.WorkoutPreferences?.Types?.ToList() ?? new List<string>(),
                Duration = c.Preferences?.WorkoutPreferences?.Duration ?? 0,
                Frequency = c.Preferences?.WorkoutPreferences?.Frequency ?? 0,
                TimeOfDay = c.Preferences?.WorkoutPreferences?.TimeOfDay ?? string.Empty
            }
        },
        MedicalConditions = c.MedicalConditions?.ToList(),
        Allergies = c.Allergies?.ToList(),
    };
}

}
}