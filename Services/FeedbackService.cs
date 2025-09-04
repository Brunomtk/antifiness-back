using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.DTO.Feedback;
using Core.Models.Feedback;
using Infrastructure.Repositories;

namespace Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;

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

        public async Task<FeedbackResponse?> GetFeedbackByIdAsync(int id)
        {
            // Evita Include sobre GetAll(); mapeia com null-safe
            var feedback = (await _unitOfWork.Feedbacks.GetAll())
                .FirstOrDefault(f => f.Id == id);

            if (feedback == null)
                return null;

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

        public Task<FeedbacksPagedDTO> GetPagedFeedbacksAsync(FeedbackFiltersDTO filters, int pageNumber, int pageSize)
        {
            // usa método especializado do repositório (com Include interno)
            return _unitOfWork.Feedbacks.GetPagedFeedbacksAsync(filters, pageNumber, pageSize);
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

        public async Task<List<FeedbackResponse>> GetFeedbacksByClientAsync(int clientId)
        {
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
    }
}
