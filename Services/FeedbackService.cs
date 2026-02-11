using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using Core.DTO.Feedback;
using Core.Models.Feedback;
using Core.Models.AppSettings;
using Infrastructure.Repositories;

namespace Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;

        // Config global do feedback obrigatório
        private const string MandatoryEnabledKey = "mandatory_feedback_enabled";
        private const string MandatoryForceFromUtcKey = "mandatory_feedback_force_from_utc";

        public FeedbackService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<FeedbackResponse> CreateFeedbackAsync(CreateFeedbackRequest request)
        {
            var feedback = new Feedback
            {
                ClientId = request.ClientId,
                TrainerId = request.TrainerId,
                Type = request.Type,
                Category = request.Category,
                Title = request.Title,
                Description = request.Description,
                Rating = Math.Max(1, Math.Min(5, request.Rating)), // força 1..5
                Status = Core.Enums.FeedbackStatus.Pending,
                AttachmentUrl = request.AttachmentUrl,
                IsAnonymous = request.IsAnonymous
            };

            await _unitOfWork.Feedbacks.Add(feedback); // <- era AddAsync
            await _unitOfWork.SaveAsync();

            return await GetFeedbackByIdAsync(feedback.Id)
                   ?? throw new InvalidOperationException("Failed to create feedback");
        }

        public async Task<FeedbackResponse> UpdateFeedbackAsync(int id, UpdateFeedbackRequest request)
        {
            // alguns ambientes têm repo sem GetByIdAsync -> busca via GetAll()
            var feedback = (await _unitOfWork.Feedbacks.GetAll())
                .FirstOrDefault(f => f.Id == id);

            if (feedback == null)
                throw new KeyNotFoundException($"Feedback with ID {id} not found");

            if (request.Type.HasValue)
                feedback.Type = request.Type.Value;

            if (request.Category.HasValue)
                feedback.Category = request.Category.Value;

            if (!string.IsNullOrEmpty(request.Title))
                feedback.Title = request.Title;

            if (!string.IsNullOrEmpty(request.Description))
                feedback.Description = request.Description;

            if (request.Rating.HasValue)
                feedback.Rating = Math.Max(1, Math.Min(5, request.Rating.Value));

            if (request.Status.HasValue)
            {
                feedback.Status = request.Status.Value;
                if (request.Status.Value == Core.Enums.FeedbackStatus.Resolved && !feedback.ResponseDate.HasValue)
                    feedback.ResponseDate = DateTime.UtcNow;
            }

            if (!string.IsNullOrEmpty(request.AdminResponse))
            {
                feedback.AdminResponse = request.AdminResponse;
                feedback.ResponseDate = DateTime.UtcNow;
            }

            if (!string.IsNullOrEmpty(request.AttachmentUrl))
                feedback.AttachmentUrl = request.AttachmentUrl;

            if (request.IsAnonymous.HasValue)
                feedback.IsAnonymous = request.IsAnonymous.Value;

            _unitOfWork.Feedbacks.Update(feedback);
            await _unitOfWork.SaveAsync();

            return await GetFeedbackByIdAsync(id)
                   ?? throw new InvalidOperationException("Failed to update feedback");
        }

        public async Task<FeedbackResponse?> GetFeedbackByIdAsync(int id, int? empresaId = null)
        {
            // Evita Include sobre GetAll(); mapeia com null-safe
            var feedback = (await _unitOfWork.Feedbacks.GetAll())
                .FirstOrDefault(f => f.Id == id);

            if (feedback == null) return null;

            if (empresaId.HasValue)
            {
                var client = await _unitOfWork.Clients.GetByIdAsync(feedback.ClientId);
                if (client == null || (client.EmpresaId ?? 0) != empresaId.Value)
                    return null;
            }

            return new FeedbackResponse
            {
                Id = feedback.Id,
                ClientId = feedback.ClientId,
                ClientName = feedback.IsAnonymous ? "Anônimo" : (feedback.Client?.Name ?? string.Empty),
                TrainerId = feedback.TrainerId,
                TrainerName = feedback.Trainer?.Name,
                Type = feedback.Type,
                TypeName = feedback.Type.ToString(),
                Category = feedback.Category,
                CategoryName = feedback.Category.ToString(),
                Title = feedback.Title,
                Description = feedback.Description,
                Rating = feedback.Rating,
                Status = feedback.Status,
                StatusName = feedback.Status.ToString(),
                AdminResponse = feedback.AdminResponse,
                ResponseDate = feedback.ResponseDate,
                AttachmentUrl = feedback.AttachmentUrl,
                IsAnonymous = feedback.IsAnonymous,
                CreatedDate = feedback.CreatedDate,
                UpdatedDate = feedback.UpdatedDate
            };
        }

        public async Task<FeedbacksPagedDTO> GetPagedFeedbacksAsync(FeedbackFiltersDTO filters, int pageNumber, int pageSize, int? empresaId = null)
        {
            if (!empresaId.HasValue)
            {
                // usa método especializado do repositório (com Include interno)
                return await _unitOfWork.Feedbacks.GetPagedFeedbacksAsync(filters, pageNumber, pageSize);
            }

            // Feedback não possui EmpresaId no schema atual. Filtramos por EmpresaId do Client.
            var clients = (await _unitOfWork.Clients.GetAll()).ToList();
            var allowedClientIds = clients
                .Where(c => (c.EmpresaId ?? 0) == empresaId.Value)
                .Select(c => c.Id)
                .ToHashSet();

            var all = (await _unitOfWork.Feedbacks.GetAll()).ToList();
            var q = all.Where(f => allowedClientIds.Contains(f.ClientId)).AsQueryable();

            // aplica filtros já existentes (mesma semântica do repo)
            if (filters.ClientId.HasValue) q = q.Where(f => f.ClientId == filters.ClientId.Value);
            if (filters.TrainerId.HasValue) q = q.Where(f => f.TrainerId == filters.TrainerId.Value);
            if (filters.Type.HasValue) q = q.Where(f => f.Type == filters.Type.Value);
            if (filters.Category.HasValue) q = q.Where(f => f.Category == filters.Category.Value);
            if (filters.Status.HasValue) q = q.Where(f => f.Status == filters.Status.Value);
            if (filters.MinRating.HasValue) q = q.Where(f => f.Rating >= filters.MinRating.Value);
            if (filters.MaxRating.HasValue) q = q.Where(f => f.Rating <= filters.MaxRating.Value);
            if (filters.StartDate.HasValue) q = q.Where(f => f.CreatedDate >= filters.StartDate.Value);
            if (filters.EndDate.HasValue) q = q.Where(f => f.CreatedDate <= filters.EndDate.Value);
            if (filters.IsAnonymous.HasValue) q = q.Where(f => f.IsAnonymous == filters.IsAnonymous.Value);
            if (!string.IsNullOrWhiteSpace(filters.Search))
            {
                var s = filters.Search.Trim().ToLower();
                q = q.Where(f => (f.Title ?? string.Empty).ToLower().Contains(s) || (f.Description ?? string.Empty).ToLower().Contains(s));
            }

            var totalCount = q.Count();
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var items = q
                .OrderByDescending(f => f.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return new FeedbacksPagedDTO
            {
                Feedbacks = items.Select(f => new FeedbackResponse
                {
                    Id = f.Id,
                    ClientId = f.ClientId,
                    ClientName = f.IsAnonymous ? "Anônimo" : (f.Client?.Name ?? string.Empty),
                    TrainerId = f.TrainerId,
                    TrainerName = f.Trainer?.Name,
                    Type = f.Type,
                    Category = f.Category,
                    Title = f.Title,
                    Description = f.Description,
                    Rating = f.Rating,
                    Status = f.Status,
                    AdminResponse = f.AdminResponse,
                    ResponseDate = f.ResponseDate,
                    AttachmentUrl = f.AttachmentUrl,
                    IsAnonymous = f.IsAnonymous,
                    CreatedDate = f.CreatedDate,
                    UpdatedDate = f.UpdatedDate
                }).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages
            };
        }

        public async Task<bool> DeleteFeedbackAsync(int id)
        {
            var feedback = (await _unitOfWork.Feedbacks.GetAll())
                .FirstOrDefault(f => f.Id == id);

            if (feedback == null)
                return false;

            _unitOfWork.Feedbacks.Delete(feedback);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public Task<FeedbackStatsDTO> GetFeedbackStatsAsync()
        {
            return _unitOfWork.Feedbacks.GetFeedbackStatsAsync();
        }

        public async Task<List<FeedbackResponse>> GetFeedbacksByClientAsync(int clientId, int? empresaId = null)
        {
            // Segurança multi-tenant: se empresaId foi informado, garantimos que o cliente pertence à empresa.
            if (empresaId.HasValue)
            {
                var client = await _unitOfWork.Clients.GetByIdAsync(clientId);
                if (client == null || (client.EmpresaId ?? 0) != empresaId.Value)
                    return new List<FeedbackResponse>();
            }

            var feedbacks = (await _unitOfWork.Feedbacks.GetAll())
                .Where(f => f.ClientId == clientId)
                .OrderByDescending(f => f.CreatedDate)
                .ToList();

            return feedbacks.Select(f => new FeedbackResponse
            {
                Id = f.Id,
                ClientId = f.ClientId,
                ClientName = f.IsAnonymous ? "Anônimo" : (f.Client?.Name ?? string.Empty),
                TrainerId = f.TrainerId,
                TrainerName = f.Trainer?.Name,
                Type = f.Type,
                TypeName = f.Type.ToString(),
                Category = f.Category,
                CategoryName = f.Category.ToString(),
                Title = f.Title,
                Description = f.Description,
                Rating = f.Rating,
                Status = f.Status,
                StatusName = f.Status.ToString(),
                AdminResponse = f.AdminResponse,
                ResponseDate = f.ResponseDate,
                AttachmentUrl = f.AttachmentUrl,
                IsAnonymous = f.IsAnonymous,
                CreatedDate = f.CreatedDate,
                UpdatedDate = f.UpdatedDate
            }).ToList();
        }

        public async Task<List<FeedbackResponse>> GetFeedbacksByTrainerAsync(int trainerId)
        {
            var feedbacks = (await _unitOfWork.Feedbacks.GetAll())
                .Where(f => f.TrainerId == trainerId)
                .OrderByDescending(f => f.CreatedDate)
                .ToList();

            return feedbacks.Select(f => new FeedbackResponse
            {
                Id = f.Id,
                ClientId = f.ClientId,
                ClientName = f.IsAnonymous ? "Anônimo" : (f.Client?.Name ?? string.Empty),
                TrainerId = f.TrainerId,
                TrainerName = f.Trainer?.Name,
                Type = f.Type,
                TypeName = f.Type.ToString(),
                Category = f.Category,
                CategoryName = f.Category.ToString(),
                Title = f.Title,
                Description = f.Description,
                Rating = f.Rating,
                Status = f.Status,
                StatusName = f.Status.ToString(),
                AdminResponse = f.AdminResponse,
                ResponseDate = f.ResponseDate,
                AttachmentUrl = f.AttachmentUrl,
                IsAnonymous = f.IsAnonymous,
                CreatedDate = f.CreatedDate,
                UpdatedDate = f.UpdatedDate
            }).ToList();

        }

        // Obrigatório: feedback modal (a cada X dias)
        private const int MandatoryFeedbackEveryDays = 30;

        private async Task<bool> GetMandatoryEnabledAsync()
        {
            var setting = (await _unitOfWork.AppSettings.GetAll())
                .FirstOrDefault(s => s.Key == MandatoryEnabledKey);

            return setting != null && bool.TryParse(setting.Value, out var v) && v;
        }

        private async Task<DateTime?> GetMandatoryForceFromAsync()
        {
            var setting = (await _unitOfWork.AppSettings.GetAll())
                .FirstOrDefault(s => s.Key == MandatoryForceFromUtcKey);

            if (setting == null) return null;
            if (DateTime.TryParse(setting.Value, out var dt)) return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            return null;
        }

        private async Task UpsertSettingAsync(string key, string value)
        {
            var existing = (await _unitOfWork.AppSettings.GetAll()).FirstOrDefault(s => s.Key == key);
            if (existing == null)
            {
                await _unitOfWork.AppSettings.Add(new Core.Models.AppSettings.AppSetting
                {
                    Key = key,
                    Value = value,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            else
            {
                existing.Value = value;
                existing.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.AppSettings.Update(existing);
            }
        }

        public async Task<MandatoryFeedbackConfigDTO> GetMandatoryConfigAsync()
        {
            var enabled = await GetMandatoryEnabledAsync();
            var forceFrom = await GetMandatoryForceFromAsync();

            return new MandatoryFeedbackConfigDTO
            {
                Enabled = enabled,
                ForceFromUtc = forceFrom,
                EveryDays = MandatoryFeedbackEveryDays
            };
        }

        public async Task<MandatoryFeedbackConfigDTO> SetMandatoryConfigAsync(SetMandatoryFeedbackConfigRequest request)
        {
            // Ao habilitar: forçamos uma nova “rodada” para TODOS, a partir de agora.
            // Ao desabilitar: nenhum cliente fica pendente.
            await UpsertSettingAsync(MandatoryEnabledKey, request.Enabled.ToString());
            if (request.Enabled)
            {
                if (request.ForceAllNow)
                    await UpsertSettingAsync(MandatoryForceFromUtcKey, DateTime.UtcNow.ToString("O"));
            }

            await _unitOfWork.SaveAsync();
            return await GetMandatoryConfigAsync();
        }

        public async Task<MandatoryFeedbackPendingResponse> GetMandatoryPendingAsync(int clientId)
        {
            var now = DateTime.UtcNow;

            // Se o admin ainda não habilitou globalmente, nunca fica pendente.
            var enabled = await GetMandatoryEnabledAsync();
            if (!enabled)
            {
                return new MandatoryFeedbackPendingResponse
                {
                    HasPending = false,
                    DaysUntilNextRequired = null,
                    Title = "",
                    Description = "",
                    Questions = new List<MandatoryFeedbackQuestionDTO>()
                };
            }

            // Quando o admin habilita, ele define um "force from".
            // Isso faz TODO mundo precisar responder pelo menos 1x após essa data.
            var forceFrom = await GetMandatoryForceFromAsync() ?? DateTime.MinValue;

            // Último feedback obrigatório do cliente
            var last = (await _unitOfWork.Feedbacks.GetAll())
                .Where(f => f.ClientId == clientId && f.Type == Core.Enums.FeedbackType.MandatorySurvey && f.CreatedDate >= forceFrom)
                .OrderByDescending(f => f.CreatedDate)
                .FirstOrDefault();

            var due = last == null ? now : last.CreatedDate.AddDays(MandatoryFeedbackEveryDays);
            var hasPending = last == null || due <= now;

            var resp = new MandatoryFeedbackPendingResponse
            {
                HasPending = hasPending,
                DaysUntilNextRequired = hasPending ? null : (int)Math.Ceiling((due - now).TotalDays),
                Title = "Conta pra gente 🙂",
                Description = "Esse feedback é obrigatório e ajuda a melhorar sua experiência.",
                Questions = new List<MandatoryFeedbackQuestionDTO>
                {
                    new MandatoryFeedbackQuestionDTO { Key = "overallRating", Label = "Nota geral do app (1 a 5)", Type = "rating", Required = true, Min = 1, Max = 5 },
                    new MandatoryFeedbackQuestionDTO { Key = "appUsabilityRating", Label = "Facilidade de usar o app (1 a 5)", Type = "rating", Required = true, Min = 1, Max = 5 },
                    new MandatoryFeedbackQuestionDTO { Key = "dietRating", Label = "Qualidade das dietas (1 a 5)", Type = "rating", Required = true, Min = 1, Max = 5 },
                    new MandatoryFeedbackQuestionDTO { Key = "workoutRating", Label = "Qualidade dos treinos (1 a 5)", Type = "rating", Required = true, Min = 1, Max = 5 },
                    new MandatoryFeedbackQuestionDTO { Key = "comment", Label = "Algum comentário ou sugestão?", Type = "text", Required = false }
                }
            };

            return resp;
        }

        public async Task<FeedbackResponse> SubmitMandatoryFeedbackAsync(SubmitMandatoryFeedbackRequest request)
        {
            // Impede envio duplicado se ainda não estiver pendente (evita flood no admin)
            var pending = await GetMandatoryPendingAsync(request.ClientId);
            if (!pending.HasPending)
                throw new InvalidOperationException("Mandatory feedback is not pending for this client yet.");

            var payload = new
            {
                request.OverallRating,
                request.AppUsabilityRating,
                request.DietRating,
                request.WorkoutRating,
                request.Comment
            };

            var feedback = new Feedback
            {
                ClientId = request.ClientId,
                TrainerId = request.TrainerId,
                Type = Core.Enums.FeedbackType.MandatorySurvey,
                Category = Core.Enums.FeedbackCategory.App,
                Title = "Mandatory App Feedback",
                Description = JsonSerializer.Serialize(payload),
                Rating = Math.Max(1, Math.Min(5, request.OverallRating)),
                Status = Core.Enums.FeedbackStatus.Resolved, // já vem respondido
                ResponseDate = DateTime.UtcNow,
                IsAnonymous = request.IsAnonymous
            };

            await _unitOfWork.Feedbacks.Add(feedback);
            await _unitOfWork.SaveAsync();

            return await GetFeedbackByIdAsync(feedback.Id)
                   ?? throw new InvalidOperationException("Failed to submit mandatory feedback");
        }
    }
}
