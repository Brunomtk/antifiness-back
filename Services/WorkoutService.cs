using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Core.DTO.Workout;
using Core.Models.Workout;
using Infrastructure.Repositories;
using Core.Enums;

namespace Services
{
    public class WorkoutService : IWorkoutService
    {
        private readonly ExerciseRepository _exerciseRepository;
        private readonly WorkoutRepository _workoutRepository;
        private readonly IGenericRepository<WorkoutExercise> _workoutExerciseRepository;
        private readonly IGenericRepository<WorkoutProgress> _workoutProgressRepository;
        private readonly IUnitOfWork _unitOfWork;

        public WorkoutService(
            ExerciseRepository exerciseRepository,
            WorkoutRepository workoutRepository,
            IUnitOfWork unitOfWork)
        {
            _exerciseRepository = exerciseRepository;
            _workoutRepository = workoutRepository;
            _unitOfWork = unitOfWork;
            _workoutExerciseRepository = unitOfWork.WorkoutExercises;
            _workoutProgressRepository = unitOfWork.WorkoutProgress;
        }

        #region Exercise Methods

        public async Task<ExercisesPagedDTO> GetExercisesAsync(ExerciseFiltersDTO filters)
        {
            var data = (await _exerciseRepository.GetAll()).Where(e => e.IsActive).ToList();
            IEnumerable<Exercise> query = data;

            // Apply filters
            if (filters.EmpresaId.HasValue)
                query = query.Where(e => e.EmpresaId == filters.EmpresaId.Value);

            if (!string.IsNullOrEmpty(filters.Search))
                query = query.Where(e => e.Name.Contains(filters.Search, StringComparison.OrdinalIgnoreCase) || 
                                       (e.Description != null && e.Description.Contains(filters.Search, StringComparison.OrdinalIgnoreCase)));

            if (filters.Difficulty?.Any() == true)
                query = query.Where(e => filters.Difficulty.Contains(e.Difficulty));

            if (filters.Category?.Any() == true)
                query = query.Where(e => filters.Category.Contains(e.Category));

            if (filters.MuscleGroups?.Any() == true)
            {
                query = query.Where(e => 
                {
                    var muscleGroups = ParseStringList(e.MuscleGroups);
                    return filters.MuscleGroups.Any(mg => muscleGroups.Contains(mg));
                });
            }

            if (filters.Equipment?.Any() == true)
            {
                query = query.Where(e => 
                {
                    var equipment = ParseStringList(e.Equipment);
                    return filters.Equipment.Any(eq => equipment.Contains(eq));
                });
            }

            // Apply sorting
            var sortBy = filters.SortBy?.ToLower() ?? "createdat";
            var sortOrder = filters.SortOrder?.ToLower() ?? "asc";

            query = sortBy switch
            {
                "name" => sortOrder == "desc" ? query.OrderByDescending(e => e.Name) : query.OrderBy(e => e.Name),
                "difficulty" => sortOrder == "desc" ? query.OrderByDescending(e => e.Difficulty) : query.OrderBy(e => e.Difficulty),
                "category" => sortOrder == "desc" ? query.OrderByDescending(e => e.Category) : query.OrderBy(e => e.Category),
                "updatedat" => sortOrder == "desc" ? query.OrderByDescending(e => e.UpdatedDate) : query.OrderBy(e => e.UpdatedDate),
                _ => sortOrder == "desc" ? query.OrderByDescending(e => e.CreatedDate) : query.OrderBy(e => e.CreatedDate)
            };

            var totalCount = query.Count();
            var exercises = query
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToList();

            return new ExercisesPagedDTO
            {
                Exercises = exercises.Select(MapToExerciseResponse).ToList(),
                TotalCount = totalCount,
                Page = filters.Page,
                PageSize = filters.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / filters.PageSize),
                HasNextPage = filters.Page * filters.PageSize < totalCount,
                HasPreviousPage = filters.Page > 1
            };
        }

        public async Task<ExerciseResponse?> GetExerciseByIdAsync(int id)
        {
            var exercise = (await _exerciseRepository.GetAll()).FirstOrDefault(e => e.Id == id && e.IsActive);
            return exercise != null ? MapToExerciseResponse(exercise) : null;
        }

        public async Task<ExerciseResponse> CreateExerciseAsync(CreateExerciseRequest request)
        {
            var exercise = new Exercise
            {
                Name = request.Name,
                Description = request.Description,
                Instructions = request.Instructions,
                MuscleGroups = JsonSerializer.Serialize(request.MuscleGroups),
                Equipment = JsonSerializer.Serialize(request.Equipment),
                Difficulty = request.Difficulty,
                Category = request.Category,
                Tips = request.Tips,
                Variations = request.Variations,
                MediaUrls = request.MediaUrls != null ? JsonSerializer.Serialize(request.MediaUrls) : null,
                EmpresaId = request.EmpresaId,
                IsActive = true
            };

            _exerciseRepository.Add(exercise);
            await _unitOfWork.SaveAsync();

            return MapToExerciseResponse(exercise);
        }

        public async Task<ExerciseResponse?> UpdateExerciseAsync(int id, UpdateExerciseRequest request)
        {
            var exercise = (await _exerciseRepository.GetAll()).FirstOrDefault(e => e.Id == id && e.IsActive);
            if (exercise == null) return null;

            if (!string.IsNullOrEmpty(request.Name))
                exercise.Name = request.Name;

            if (request.Description != null)
                exercise.Description = request.Description;

            if (request.Instructions != null)
                exercise.Instructions = request.Instructions;

            if (request.MuscleGroups != null)
                exercise.MuscleGroups = JsonSerializer.Serialize(request.MuscleGroups);

            if (request.Equipment != null)
                exercise.Equipment = JsonSerializer.Serialize(request.Equipment);

            if (request.Difficulty.HasValue)
                exercise.Difficulty = request.Difficulty.Value;

            if (request.Category.HasValue)
                exercise.Category = request.Category.Value;

            if (request.Tips != null)
                exercise.Tips = request.Tips;

            if (request.Variations != null)
                exercise.Variations = request.Variations;

            if (request.MediaUrls != null)
                exercise.MediaUrls = JsonSerializer.Serialize(request.MediaUrls);

            if (request.IsActive.HasValue)
                exercise.IsActive = request.IsActive.Value;

            exercise.UpdatedDate = DateTime.UtcNow;

            _exerciseRepository.Update(exercise);
            await _unitOfWork.SaveAsync();

            return MapToExerciseResponse(exercise);
        }

        public async Task<bool> DeleteExerciseAsync(int id)
        {
            var exercise = (await _exerciseRepository.GetAll()).FirstOrDefault(e => e.Id == id);
            if (exercise == null) return false;

            exercise.IsActive = false;
            exercise.UpdatedDate = DateTime.UtcNow;

            _exerciseRepository.Update(exercise);
            await _unitOfWork.SaveAsync();

            return true;
        }

        #endregion

        #region Workout Methods

        public async Task<WorkoutsPagedDTO> GetWorkoutsAsync(WorkoutFiltersDTO filters)
        {
            var data = (await _workoutRepository.GetAll()).ToList();
            IEnumerable<Workout> query = data;

            // Apply filters
            if (filters.EmpresaId.HasValue)
                query = query.Where(w => w.EmpresaId == filters.EmpresaId.Value);

            if (filters.ClientId.HasValue)
                query = query.Where(w => w.ClientId == filters.ClientId.Value);

            if (!string.IsNullOrEmpty(filters.Search))
                query = query.Where(w => w.Name.Contains(filters.Search, StringComparison.OrdinalIgnoreCase) || 
                                       (w.Description != null && w.Description.Contains(filters.Search, StringComparison.OrdinalIgnoreCase)));

            if (filters.Type?.Any() == true)
                query = query.Where(w => filters.Type.Contains(w.Type));

            if (filters.Difficulty?.Any() == true)
                query = query.Where(w => filters.Difficulty.Contains(w.Difficulty));

            if (filters.Status?.Any() == true)
                query = query.Where(w => filters.Status.Contains(w.Status));

            if (filters.IsTemplate.HasValue)
                query = query.Where(w => w.IsTemplate == filters.IsTemplate.Value);

            if (filters.DateStart.HasValue)
                query = query.Where(w => w.CreatedDate >= filters.DateStart.Value);

            if (filters.DateEnd.HasValue)
                query = query.Where(w => w.CreatedDate <= filters.DateEnd.Value);

            if (filters.Tags?.Any() == true)
            {
                query = query.Where(w => 
                {
                    var tags = ParseStringList(w.Tags);
                    return filters.Tags.Any(tag => tags.Contains(tag));
                });
            }

            // Apply sorting
            var sortBy = filters.SortBy?.ToLower() ?? "createdat";
            var sortOrder = filters.SortOrder?.ToLower() ?? "asc";

            query = sortBy switch
            {
                "name" => sortOrder == "desc" ? query.OrderByDescending(w => w.Name) : query.OrderBy(w => w.Name),
                "type" => sortOrder == "desc" ? query.OrderByDescending(w => w.Type) : query.OrderBy(w => w.Type),
                "difficulty" => sortOrder == "desc" ? query.OrderByDescending(w => w.Difficulty) : query.OrderBy(w => w.Difficulty),
                "status" => sortOrder == "desc" ? query.OrderByDescending(w => w.Status) : query.OrderBy(w => w.Status),
                "updatedat" => sortOrder == "desc" ? query.OrderByDescending(w => w.UpdatedDate) : query.OrderBy(w => w.UpdatedDate),
                _ => sortOrder == "desc" ? query.OrderByDescending(w => w.CreatedDate) : query.OrderBy(w => w.CreatedDate)
            };

            var totalCount = query.Count();
            var workouts = query
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToList();

            return new WorkoutsPagedDTO
            {
                Workouts = workouts.Select(MapToWorkoutResponse).ToList(),
                TotalCount = totalCount,
                Page = filters.Page,
                PageSize = filters.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / filters.PageSize),
                HasNextPage = filters.Page * filters.PageSize < totalCount,
                HasPreviousPage = filters.Page > 1
            };
        }

        public async Task<WorkoutResponse?> GetWorkoutByIdAsync(int id)
        {
            var workout = await _workoutRepository.GetWorkoutWithExercisesForUpdateAsync(id);
            return workout != null ? MapToWorkoutResponse(workout) : null;
        }

        public async Task<WorkoutResponse> CreateWorkoutAsync(CreateWorkoutRequest request)
        {
            var workout = new Workout
            {
                Name = request.Name,
                Description = request.Description,
                Type = request.Type,
                Difficulty = request.Difficulty,
                EstimatedDuration = request.EstimatedDuration,
                EstimatedCalories = request.EstimatedCalories,
                Tags = request.Tags != null ? JsonSerializer.Serialize(request.Tags) : null,
                IsTemplate = request.IsTemplate,
                Notes = request.Notes,
                EmpresaId = request.EmpresaId,
                ClientId = request.ClientId,
                Status = WorkoutStatus.Draft
            };

            _workoutRepository.Add(workout);
            await _unitOfWork.SaveAsync();

            // Add exercises
            if (request.Exercises != null)
            {
                foreach (var exerciseRequest in request.Exercises)
                {
                    var workoutExercise = new WorkoutExercise
                    {
                        WorkoutId = workout.Id,
                        ExerciseId = exerciseRequest.ExerciseId,
                        Order = exerciseRequest.Order,
                        Sets = exerciseRequest.Sets,
                        Reps = exerciseRequest.Reps,
                        Weight = exerciseRequest.Weight,
                        RestTime = exerciseRequest.RestTime,
                        Notes = exerciseRequest.Notes
                    };

                    _workoutExerciseRepository.Add(workoutExercise);
                }
                await _unitOfWork.SaveAsync();
            }

            return await GetWorkoutByIdAsync(workout.Id) ?? throw new InvalidOperationException("Failed to retrieve created workout");
        }

        public async Task<WorkoutResponse?> UpdateWorkoutAsync(int id, UpdateWorkoutRequest request)
        {
            var workout = await _workoutRepository.GetWorkoutWithExercisesAsync(id);
            if (workout == null) return null;

            if (!string.IsNullOrEmpty(request.Name))
                workout.Name = request.Name;

            if (request.Description != null)
                workout.Description = request.Description;

            if (request.Type.HasValue)
                workout.Type = request.Type.Value;

            if (request.Difficulty.HasValue)
                workout.Difficulty = request.Difficulty.Value;

            if (request.EstimatedDuration.HasValue)
                workout.EstimatedDuration = request.EstimatedDuration.Value;

            if (request.EstimatedCalories.HasValue)
                workout.EstimatedCalories = request.EstimatedCalories.Value;

            if (request.Tags != null)
                workout.Tags = JsonSerializer.Serialize(request.Tags);

            if (request.Notes != null)
                workout.Notes = request.Notes;

            workout.UpdatedDate = DateTime.UtcNow;

            // Update exercises if provided
            if (request.Exercises != null)
            {
                // Remove existing exercises
                var existingExercises = workout.WorkoutExercises?.ToList() ?? new List<WorkoutExercise>();
                foreach (var existing in existingExercises)
                {
                    existing.Exercise = null;
                    _workoutExerciseRepository.Delete(existing);
                }

                // Add new exercises
                foreach (var exerciseRequest in request.Exercises)
                {
                    var workoutExercise = new WorkoutExercise
                    {
                        WorkoutId = workout.Id,
                        ExerciseId = exerciseRequest.ExerciseId,
                        Order = exerciseRequest.Order,
                        Sets = exerciseRequest.Sets,
                        Reps = exerciseRequest.Reps,
                        Weight = exerciseRequest.Weight,
                        RestTime = exerciseRequest.RestTime,
                        Notes = exerciseRequest.Notes
                    };

                    _workoutExerciseRepository.Add(workoutExercise);
                }
            }

            _workoutRepository.Update(workout);
            await _unitOfWork.SaveAsync();

            return await GetWorkoutByIdAsync(id);
        }

        public async Task<bool> DeleteWorkoutAsync(int id)
        {
            var workout = (await _workoutRepository.GetAll()).FirstOrDefault(w => w.Id == id);
            if (workout == null) return false;

            _workoutRepository.Delete(workout);
            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<bool> ChangeWorkoutStatusAsync(int id, ChangeWorkoutStatusRequest request)
        {
            var workout = (await _workoutRepository.GetAll()).FirstOrDefault(w => w.Id == id);
            if (workout == null) return false;

            workout.Status = request.Status;
            workout.UpdatedDate = DateTime.UtcNow;

            _workoutRepository.Update(workout);
            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<WorkoutsPagedDTO> GetTemplatesAsync(WorkoutFiltersDTO filters)
        {
            filters.IsTemplate = true;
            return await GetWorkoutsAsync(filters);
        }

        public async Task<WorkoutResponse?> InstantiateTemplateAsync(int templateId, CreateWorkoutRequest? overrides = null)
        {
            var template = await _workoutRepository.GetWorkoutWithExercisesAsync(templateId);
            if (template == null || !template.IsTemplate) return null;

            var templateTags = ParseStringList(template.Tags);

            var newWorkout = new CreateWorkoutRequest
            {
                Name = overrides?.Name ?? $"{template.Name} - Copy",
                Description = overrides?.Description ?? template.Description,
                Type = overrides?.Type ?? template.Type,
                Difficulty = overrides?.Difficulty ?? template.Difficulty,
                EstimatedDuration = overrides?.EstimatedDuration ?? template.EstimatedDuration,
                EstimatedCalories = overrides?.EstimatedCalories ?? template.EstimatedCalories,
                Tags = overrides?.Tags ?? templateTags,
                IsTemplate = false,
                Notes = overrides?.Notes ?? template.Notes,
                EmpresaId = overrides?.EmpresaId ?? template.EmpresaId,
                ClientId = overrides?.ClientId ?? template.ClientId,
                Exercises = overrides?.Exercises ?? (template.WorkoutExercises?.Select(we => new CreateWorkoutExerciseRequest
                {
                    ExerciseId = we.ExerciseId,
                    Order = we.Order,
                    Sets = we.Sets,
                    Reps = we.Reps,
                    Weight = we.Weight,
                    RestTime = we.RestTime,
                    Notes = we.Notes
                }).ToList() ?? new List<CreateWorkoutExerciseRequest>())
            };

            return await CreateWorkoutAsync(newWorkout);
        }

        #endregion

        
        #region Exercise substitutions

        public async Task<List<WorkoutExerciseSubstitutionResponse>> GetWorkoutExerciseSubstitutionsAsync(
            int workoutId,
            int workoutExerciseId)
        {
            var repo = _workoutRepository as WorkoutRepository
                ?? throw new InvalidOperationException("WorkoutRepository inválido.");

            var workoutExercise = await repo.GetWorkoutExerciseWithSubstitutionsForUpdateAsync(workoutId, workoutExerciseId);
            if (workoutExercise == null || workoutExercise.Substitutions == null)
                return new List<WorkoutExerciseSubstitutionResponse>();

            return workoutExercise.Substitutions
                .OrderBy(s => s.Exercise.Name)
                .Select(MapToWorkoutExerciseSubstitutionResponse)
                .ToList();
        }

        public async Task<WorkoutExerciseSubstitutionResponse> CreateWorkoutExerciseSubstitutionAsync(
            int workoutId,
            int workoutExerciseId,
            CreateWorkoutExerciseSubstitutionRequest request)
        {
            var repo = _workoutRepository as WorkoutRepository
                ?? throw new InvalidOperationException("WorkoutRepository inválido.");

            var workoutExercise = await repo.GetWorkoutExerciseWithSubstitutionsForUpdateAsync(workoutId, workoutExerciseId);
            if (workoutExercise == null)
                throw new InvalidOperationException("Exercício do treino não encontrado.");

            var exercise = await _exerciseRepository.GetById(request.ExerciseId);
            if (exercise == null)
                throw new InvalidOperationException("Exercício de substituição não encontrado.");

            workoutExercise.Substitutions ??= new List<WorkoutExerciseSubstitution>();

            var substitution = new WorkoutExerciseSubstitution
            {
                WorkoutExerciseId = workoutExercise.Id,
                ExerciseId = exercise.Id,
                Notes = request.Notes,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            workoutExercise.Substitutions.Add(substitution);

            _workoutExerciseRepository.Update(workoutExercise);
            await _unitOfWork.SaveAsync();

            return MapToWorkoutExerciseSubstitutionResponse(substitution);
        }

        public async Task<WorkoutExerciseSubstitutionResponse?> UpdateWorkoutExerciseSubstitutionAsync(
            int workoutId,
            int workoutExerciseId,
            int substitutionId,
            UpdateWorkoutExerciseSubstitutionRequest request)
        {
            var repo = _workoutRepository as WorkoutRepository
                ?? throw new InvalidOperationException("WorkoutRepository inválido.");

            var workoutExercise = await repo.GetWorkoutExerciseWithSubstitutionsForUpdateAsync(workoutId, workoutExerciseId);
            if (workoutExercise == null || workoutExercise.Substitutions == null)
                return null;

            var substitution = workoutExercise.Substitutions.FirstOrDefault(s => s.Id == substitutionId);
            if (substitution == null)
                return null;

            if (request.ExerciseId.HasValue)
            {
                var exercise = await _exerciseRepository.GetById(request.ExerciseId.Value);
                if (exercise == null)
                    throw new InvalidOperationException("Exercício de substituição não encontrado.");
                substitution.ExerciseId = exercise.Id;
            }

            if (request.Notes != null)
                substitution.Notes = request.Notes;

            substitution.UpdatedDate = DateTime.UtcNow;

            _workoutExerciseRepository.Update(workoutExercise);
            await _unitOfWork.SaveAsync();

            return MapToWorkoutExerciseSubstitutionResponse(substitution);
        }

        public async Task<bool> DeleteWorkoutExerciseSubstitutionAsync(
            int workoutId,
            int workoutExerciseId,
            int substitutionId)
        {
            var repo = _workoutRepository as WorkoutRepository
                ?? throw new InvalidOperationException("WorkoutRepository inválido.");

            var workoutExercise = await repo.GetWorkoutExerciseWithSubstitutionsForUpdateAsync(workoutId, workoutExerciseId);
            if (workoutExercise == null || workoutExercise.Substitutions == null)
                return false;

            var substitution = workoutExercise.Substitutions.FirstOrDefault(s => s.Id == substitutionId);
            if (substitution == null)
                return false;

            workoutExercise.Substitutions.Remove(substitution);

            _workoutExerciseRepository.Update(workoutExercise);
            await _unitOfWork.SaveAsync();

            return true;
        }

        #endregion



#region Progress Methods

        public async Task<List<WorkoutProgressResponse>> GetWorkoutProgressAsync(int workoutId)
        {
            var progresses = (await _workoutProgressRepository.GetAll())
                .Where(p => p.WorkoutId == workoutId)
                .OrderByDescending(p => p.Date)
                .ToList();

            return progresses.Select(MapToWorkoutProgressResponse).ToList();
        }

        public async Task<WorkoutProgressResponse> CreateWorkoutProgressAsync(int workoutId, CreateWorkoutProgressRequest request)
        {
            var workout = (await _workoutRepository.GetAll()).FirstOrDefault(w => w.Id == workoutId);
            if (workout == null) throw new ArgumentException("Workout not found");

            var progress = new WorkoutProgress
            {
                WorkoutId = workoutId,
                ClientId = request.ClientId,
                Date = request.Date,
                ActualDuration = request.ActualDuration,
                ActualCalories = request.ActualCalories,
                Rating = request.Rating,
                Mood = request.Mood,
                EnergyLevel = request.EnergyLevel,
                IsCompleted = request.IsCompleted,
                ExerciseProgress = request.ExerciseProgress != null ? JsonSerializer.Serialize(request.ExerciseProgress) : null,
                Notes = request.Notes,
                HasPersonalRecord = false // TODO: Implement PR detection logic
            };

            _workoutProgressRepository.Add(progress);
            await _unitOfWork.SaveAsync();

            return MapToWorkoutProgressResponse(progress);
        }

        #endregion

        #region Stats Methods

        public async Task<WorkoutStatsDTO> GetWorkoutStatsAsync(int? empresaId = null, int? clientId = null)
        {
            var exercises = (await _exerciseRepository.GetAll()).ToList();
            var workouts = (await _workoutRepository.GetAll()).ToList();
            var progresses = (await _workoutProgressRepository.GetAll()).ToList();

            if (empresaId.HasValue)
            {
                exercises = exercises.Where(e => e.EmpresaId == empresaId.Value).ToList();
                workouts = workouts.Where(w => w.EmpresaId == empresaId.Value).ToList();
            }

            if (clientId.HasValue)
            {
                workouts = workouts.Where(w => w.ClientId == clientId.Value).ToList();
                progresses = progresses.Where(p => p.ClientId == clientId.Value).ToList();
            }

            var totalExercises = exercises.Count;
            var activeExercises = exercises.Count(e => e.IsActive);
            var totalWorkouts = workouts.Count;
            var completedWorkouts = progresses.Count(p => p.IsCompleted);
            var templateWorkouts = workouts.Count(w => w.IsTemplate);

            var completionRate = totalWorkouts > 0 ? (double)completedWorkouts / totalWorkouts * 100 : 0;

            var totalDuration = progresses.Where(p => p.ActualDuration.HasValue).Sum(p => p.ActualDuration.Value);
            var totalCalories = progresses.Where(p => p.ActualCalories.HasValue).Sum(p => p.ActualCalories.Value);
            var averageRating = progresses.Where(p => p.Rating.HasValue).Any() 
                ? progresses.Where(p => p.Rating.HasValue).Average(p => p.Rating.Value) 
                : 0;
            var personalRecords = progresses.Count(p => p.HasPersonalRecord);

            // Workout types distribution
            var workoutsByType = workouts
                .GroupBy(w => w.Type)
                .Select(g => new WorkoutTypeStats
                {
                    Type = g.Key.ToString(),
                    Count = g.Count(),
                    Percentage = totalWorkouts > 0 ? (double)g.Count() / totalWorkouts * 100 : 0
                })
                .ToList();

            return new WorkoutStatsDTO
            {
                TotalExercises = totalExercises,
                ActiveExercises = activeExercises,
                TotalWorkouts = totalWorkouts,
                CompletedWorkouts = completedWorkouts,
                TemplateWorkouts = templateWorkouts,
                CompletionRate = completionRate,
                TotalDuration = totalDuration,
                TotalCalories = totalCalories,
                AverageRating = averageRating,
                PersonalRecords = personalRecords,
                WorkoutsByType = workoutsByType,
                MuscleGroupDistribution = new List<MuscleGroupStats>()
            };
        }

        #endregion

        #region Helper Methods

        private List<string> ParseStringList(string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return new List<string>();
            try
            {
                return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        #endregion

        #region Mapping Methods

        private ExerciseResponse MapToExerciseResponse(Exercise exercise)
        {
            return new ExerciseResponse
            {
                Id = exercise.Id,
                Name = exercise.Name,
                Description = exercise.Description,
                Instructions = exercise.Instructions,
                MuscleGroups = ParseStringList(exercise.MuscleGroups),
                Equipment = ParseStringList(exercise.Equipment),
                Difficulty = exercise.Difficulty,
                Category = exercise.Category,
                Tips = exercise.Tips,
                Variations = exercise.Variations,
                MediaUrls = ParseStringList(exercise.MediaUrls),
                IsActive = exercise.IsActive,
                EmpresaId = exercise.EmpresaId,
                CreatedAt = exercise.CreatedDate,
                UpdatedAt = exercise.UpdatedDate
            };
        }

        private WorkoutResponse MapToWorkoutResponse(Workout workout)
        {
            return new WorkoutResponse
            {
                Id = workout.Id,
                Name = workout.Name,
                Description = workout.Description,
                Type = workout.Type,
                Difficulty = workout.Difficulty,
                Status = workout.Status,
                EstimatedDuration = workout.EstimatedDuration,
                EstimatedCalories = workout.EstimatedCalories,
                Tags = ParseStringList(workout.Tags),
                IsTemplate = workout.IsTemplate,
                Notes = workout.Notes,
                EmpresaId = workout.EmpresaId,
                ClientId = workout.ClientId,
                Exercises = workout.WorkoutExercises?.Select(we => new WorkoutExerciseResponse
                {
                    Id = we.Id,
                    ExerciseId = we.ExerciseId,
                    ExerciseName = we.Exercise?.Name ?? "",
                    Order = we.Order,
                    Sets = we.Sets,
                    Reps = we.Reps,
                    Weight = we.Weight,
                    RestTime = we.RestTime,
                    Notes = we.Notes,
                    IsCompleted = we.IsCompleted,
                    CompletedSets = we.CompletedSets,
                    Substitutions = (we.Substitutions ?? new List<WorkoutExerciseSubstitution>())
                        .Select(MapToWorkoutExerciseSubstitutionResponse)
                        .ToList()
                }).OrderBy(e => e.Order).ToList() ?? new List<WorkoutExerciseResponse>(),

                CreatedAt = workout.CreatedDate,
                UpdatedAt = workout.UpdatedDate
            };
        }

        
        private WorkoutExerciseSubstitutionResponse MapToWorkoutExerciseSubstitutionResponse(WorkoutExerciseSubstitution substitution)
        {
            return new WorkoutExerciseSubstitutionResponse
            {
                Id = substitution.Id,
                WorkoutExerciseId = substitution.WorkoutExerciseId,
                ExerciseId = substitution.ExerciseId,
                ExerciseName = substitution.Exercise?.Name ?? string.Empty,
                Notes = substitution.Notes,
                CreatedAt = substitution.CreatedDate,
                UpdatedAt = substitution.UpdatedDate
            };
        }

private WorkoutProgressResponse MapToWorkoutProgressResponse(WorkoutProgress progress)
        {
            return new WorkoutProgressResponse
            {
                Id = progress.Id,
                WorkoutId = progress.WorkoutId,
                WorkoutName = progress.Workout?.Name ?? "",
                ClientId = progress.ClientId,
                Date = progress.Date,
                ActualDuration = progress.ActualDuration,
                ActualCalories = progress.ActualCalories,
                Rating = progress.Rating,
                Mood = progress.Mood,
                EnergyLevel = progress.EnergyLevel,
                IsCompleted = progress.IsCompleted,
                ExerciseProgress = !string.IsNullOrEmpty(progress.ExerciseProgress) 
                    ? JsonSerializer.Deserialize<List<Core.DTO.Workout.ExerciseProgressResponse>>(progress.ExerciseProgress)
                    : null,
                Notes = progress.Notes,
                HasPersonalRecord = progress.HasPersonalRecord,
                CreatedAt = progress.CreatedDate
            };
        }

        #endregion
    }
}