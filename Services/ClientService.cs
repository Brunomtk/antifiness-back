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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public interface IClientService
    {
        Task<IEnumerable<DietSummaryDTO>> GetDietHistoryAsync(int clientId);
        Task<IEnumerable<WorkoutSummaryDTO>> GetWorkoutHistoryAsync(int clientId);
        Task<DietSummaryDTO?> GetCurrentDietAsync(int clientId);
        Task<WorkoutSummaryDTO?> GetCurrentWorkoutAsync(int clientId);
        Task<ClientBasicDTO?> GetClientBasicByIdAsync(int id);
        Task<ClientResponse?> CreateClientAsync(CreateClientRequest request);
        Task<ClientResponse?> GetClientByIdAsync(int id);
        Task<ClientsPagedDTO> GetClientsPagedAsync(ClientFiltersDTO filters);
        Task<bool> UpdateClientAsync(int id, UpdateClientRequest request);
        Task<bool> DeleteClientAsync(int id);
        Task<ClientMeasurementDTO?> AddWeightProgressAsync(int clientId, AddWeightProgressRequest request);
        Task<ClientMeasurementDTO?> AddMeasurementsProgressAsync(int clientId, AddMeasurementsProgressRequest request);
        Task<object?> AddPhotoProgressAsync(int clientId, AddPhotoProgressRequest request);
        Task<AchievementDTO?> AddAchievementAsync(int clientId, AddAchievementRequest request);
        Task<ClientStatsDTO> GetClientStatsAsync();
        Task<IReadOnlyList<AchievementDTO>> GetAchievementsAsync(int clientId, int page = 1, int pageSize = 20);
    }

    public sealed class ClientService : IClientService
    {
        private readonly IUnitOfWork _uow;
        private readonly IClientRepository _clients;
        private readonly DietRepository _diets;
        private readonly WorkoutRepository _workouts;

        // In-memory achievements store (to be replaced by EF entity later)
        private static int _achievementSeq = 1000;
        private static readonly ConcurrentDictionary<int, List<AchievementDTO>> _achievements = new();

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
                    Status = g.Status ?? ClientStatus.Active
                }).ToList(),
                Measurements = req.Measurements?.Select(m => new ClientMeasurement
                {
                    Date = m.Date == default ? DateTime.UtcNow : m.Date,
                    Weight = m.Weight ?? 0.0,
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
                CreatedDate = DateTime.UtcNow
            };

            return photoProgress;
        }

        public async Task<AchievementDTO?> AddAchievementAsync(int clientId, AddAchievementRequest request)
        {
            var client = await _clients.GetByIdAsync(clientId);
            if (client == null) return null;

            var dto = new AchievementDTO
            {
                Id = Interlocked.Increment(ref _achievementSeq),
                ClientId = clientId,
                Title = request.Title,
                Description = request.Description,
                Type = request.Type,
                Category = request.Category,
                UnlockedDate = DateTime.UtcNow
            };

            var list = _achievements.GetOrAdd(clientId, _ => new List<AchievementDTO>());
            lock (list)
            {
                list.Add(dto);
            }

            return dto;
        }


        
        public Task<IReadOnlyList<AchievementDTO>> GetAchievementsAsync(int clientId, int page = 1, int pageSize = 20)
        {
            if (page <= 0 || pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(page), "page e pageSize devem ser positivos");

            if (!_achievements.TryGetValue(clientId, out var list))
                list = new List<AchievementDTO>();

            var ordered = list.OrderByDescending(a => a.UnlockedDate);
            var paged = ordered.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Task.FromResult<IReadOnlyList<AchievementDTO>>(paged);
        }

public async Task<ClientStatsDTO> GetClientStatsAsync()
        {
            var allClients = (await _clients.GetAll()).ToList();
            var total = allClients.Count;
            var active = allClients.Count(c => c.Status == ClientStatus.Active);
            var inactive = allClients.Count(c => c.Status == ClientStatus.Inactive);
            var paused = allClients.Count(c => c.Status == ClientStatus.Paused);

            var currentMonth = DateTime.UtcNow.AddMonths(-1);
            var newThisMonth = allClients.Count(c => c.CreatedDate >= currentMonth);

            var previousMonth = DateTime.UtcNow.AddMonths(-2);
            var newPreviousMonth = allClients.Count(c => c.CreatedDate >= previousMonth && c.CreatedDate < currentMonth);
            var monthlyGrowth = newPreviousMonth == 0 ? 0.0 : ((double)(newThisMonth - newPreviousMonth) / newPreviousMonth) * 100.0;

            var goalsAchieved = allClients
                .SelectMany(c => c.Goals ?? Enumerable.Empty<ClientGoal>())
                .Count(g => g.Status == ClientStatus.Completed);

            var weightLossValues = allClients
                .Where(c => c.Measurements != null && c.Measurements.Any())
                .Select(c =>
                {
                    var measurements = c.Measurements.OrderBy(m => m.Date).ToList();
                    if (measurements.Count < 2) return 0.0;

                    var firstWeight = measurements.First().Weight;
                    var lastWeight = measurements.Last().Weight;
                    return firstWeight - lastWeight;
                })
                .Where(w => w > 0)
                .ToList();

            var avgWeightLoss = weightLossValues.Any() ? weightLossValues.Average() : 0.0;

            var retention = total == 0 ? 0.0 :
                allClients.Count(c => (DateTime.UtcNow - c.CreatedDate).TotalDays >= 30 && c.Status == ClientStatus.Active) * 100.0 / total;

            var clientsWithNutritionist = allClients.Count(c => c.EmpresaId > 0);
            var clientsWithPlans = allClients.Count(c => !string.IsNullOrEmpty(c.PlanId));

            return new ClientStatsDTO
            {
                TotalClients = total,
                ActiveClients = active,
                InactiveClients = inactive,
                PausedClients = paused,
                NewClientsThisMonth = newThisMonth,
                ClientsWithGoalsAchieved = goalsAchieved,
                AverageWeightLoss = Math.Round(avgWeightLoss, 2),
                RetentionRate = Math.Round(retention, 2),
                MonthlyGrowthPercentage = Math.Round(monthlyGrowth, 2),
                ClientsWithNutritionist = clientsWithNutritionist,
                ClientsWithActivePlan = clientsWithPlans
            };
        }

        private ClientPreferences MapToClientPreferences(ClientPreferencesDTO dto)
        {
            return new ClientPreferences
            {
                DietaryRestrictions = dto.DietaryRestrictions ?? new List<string>(),
                FavoriteFood = dto.FavoriteFood ?? new List<string>(),
                DislikedFood = dto.DislikedFood ?? new List<string>(),
                MealTimes = new MealTimes
                {
                    Breakfast = dto.MealTimes?.Breakfast ?? string.Empty,
                    Lunch = dto.MealTimes?.Lunch ?? string.Empty,
                    Dinner = dto.MealTimes?.Dinner ?? string.Empty,
                    Snacks = dto.MealTimes?.Snacks ?? new List<string>()
                },
                WorkoutPreferences = new WorkoutPreferences
                {
                    Types = dto.WorkoutPreferences?.Types ?? new List<string>(),
                    Duration = dto.WorkoutPreferences?.Duration ?? 60,
                    Frequency = dto.WorkoutPreferences?.Frequency ?? 3,
                    TimeOfDay = dto.WorkoutPreferences?.TimeOfDay ?? string.Empty
                }
            };
        }

        private ClientPreferencesDTO MapToClientPreferencesDTO(ClientPreferences preferences)
        {
            return new ClientPreferencesDTO
            {
                DietaryRestrictions = preferences.DietaryRestrictions ?? new List<string>(),
                FavoriteFood = preferences.FavoriteFood ?? new List<string>(),
                DislikedFood = preferences.DislikedFood ?? new List<string>(),
                MealTimes = new MealTimesDTO
                {
                    Breakfast = preferences.MealTimes?.Breakfast ?? string.Empty,
                    Lunch = preferences.MealTimes?.Lunch ?? string.Empty,
                    Dinner = preferences.MealTimes?.Dinner ?? string.Empty,
                    Snacks = preferences.MealTimes?.Snacks ?? new List<string>()
                },
                WorkoutPreferences = new WorkoutPreferencesDTO
                {
                    Types = preferences.WorkoutPreferences?.Types ?? new List<string>(),
                    Duration = preferences.WorkoutPreferences?.Duration ?? 60,
                    Frequency = preferences.WorkoutPreferences?.Frequency ?? 3,
                    TimeOfDay = preferences.WorkoutPreferences?.TimeOfDay ?? string.Empty
                }
            };
        }

        private ClientResponse MapToResponse(Client c) => new ClientResponse
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.Email,
            Phone = c.Phone,
            Avatar = c.Avatar,
            DateOfBirth = c.DateOfBirth ?? DateTime.MinValue,
            Gender = c.Gender ?? string.Empty,
            Height = c.Height ?? 0,
            CurrentWeight = c.CurrentWeight ?? 0.0,
            TargetWeight = c.TargetWeight ?? 0.0,
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
                Status = g.Status
            }).ToList() ?? new List<ClientGoalDTO>(),
            Measurements = c.Measurements?.Select(m => new ClientMeasurementDTO
            {
                Id = m.Id,
                Date = m.Date,
                Weight = m.Weight,
                BodyFat = m.BodyFat,
                MuscleMass = m.MuscleMass,
                Waist = m.Waist,
                Chest = m.Chest,
                Arms = m.Arms,
                Thighs = m.Thighs,
                Notes = m.Notes
            }).ToList() ?? new List<ClientMeasurementDTO>(),
            Preferences = c.Preferences != null ? MapToClientPreferencesDTO(c.Preferences) : new ClientPreferencesDTO(),
            MedicalConditions = c.MedicalConditions,
            Allergies = c.Allergies,
            CreatedDate = c.CreatedDate,
            UpdatedDate = c.UpdatedDate};
        public async Task<DietSummaryDTO?> GetCurrentDietAsync(int clientId)
        {
            var diet = await _diets.GetCurrentByClientIdAsync(clientId);
            return diet != null ? MapToDietSummaryDTO(diet) : null;
        }

        public async Task<WorkoutSummaryDTO?> GetCurrentWorkoutAsync(int clientId)
        {
            var w = await _workouts.GetCurrentByClientIdAsync(clientId);
            return w != null ? MapToWorkoutSummaryDTO(w) : null;
        }

        public async Task<ClientBasicDTO?> GetClientBasicByIdAsync(int id)
        {
            var client = await _clients.GetByIdAsync(id);
            if (client == null) return null;
            return MapToClientBasicDTO(client);
        }


        private static ClientBasicDTO MapToClientBasicDTO(Client c) => new ClientBasicDTO
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.Email,
            Phone = c.Phone,
            Avatar = c.Avatar,
            DateOfBirth = c.DateOfBirth ?? DateTime.MinValue,
            Gender = c.Gender ?? string.Empty,
            Height = c.Height ?? 0,
            CurrentWeight = c.CurrentWeight ?? 0.0,
            TargetWeight = c.TargetWeight ?? 0.0,
            ActivityLevel = c.ActivityLevel,
            Status = c.Status,
            KanbanStage = c.KanbanStage,
            PlanId = c.PlanId,
            EmpresaId = c.EmpresaId ?? 0,
            CreatedDate = c.CreatedDate,
            UpdatedDate = c.UpdatedDate};


        private static WorkoutSummaryDTO MapToWorkoutSummaryDTO(Core.Models.Workout.Workout w) => new WorkoutSummaryDTO
        {
            Id = w.Id,
            Name = w.Name,
            Description = w.Description,
            EmpresaId = w.EmpresaId,
            ClientId = w.ClientId,
            Notes = w.Notes,
            CreatedDate = w.CreatedDate,
            UpdatedDate = w.UpdatedDate
        };


        
        public async Task<IEnumerable<DietSummaryDTO>> GetDietHistoryAsync(int clientId)
        {
            var diets = await _diets.GetByClientIdAsync(clientId);
            return diets.Select(MapToDietSummaryDTO).ToList();
        }

        public async Task<IEnumerable<WorkoutSummaryDTO>> GetWorkoutHistoryAsync(int clientId)
        {
            var wrks = await _workouts.GetByClientIdAsync(clientId);
            return wrks.Select(MapToWorkoutSummaryDTO).ToList();
        }

private static DietSummaryDTO MapToDietSummaryDTO(Core.Models.Diet.Diet d) => new DietSummaryDTO
        {
            Id = d.Id,
            Name = d.Name,
            Description = d.Description,
            ClientId = d.ClientId,
            EmpresaId = d.EmpresaId,
            DailyCalories = d.DailyCalories,
            DailyProtein  = d.DailyProtein,
            DailyCarbs    = d.DailyCarbs,
            DailyFat      = d.DailyFat,
            CreatedDate = d.CreatedDate,
            UpdatedDate = d.UpdatedDate
        };


}
}