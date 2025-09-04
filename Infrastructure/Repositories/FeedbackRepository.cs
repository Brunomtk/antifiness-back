using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.DTO.Feedback;
using Core.Models.Feedback;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class FeedbackRepository : GenericRepository<Feedback>
    {
        public FeedbackRepository(DbContextClass context) : base(context)
        {
        }

        // Atalho para o DbSet<Feedback>
        private DbSet<Feedback> Feedbacks => _dbContext.Set<Feedback>();

        public async Task<FeedbacksPagedDTO> GetPagedFeedbacksAsync(FeedbackFiltersDTO filters, int pageNumber, int pageSize)
        {
            var query = Feedbacks
                .Include(f => f.Client)
                .Include(f => f.Trainer)
                .AsQueryable();

            // Filtros
            if (filters.ClientId.HasValue)
                query = query.Where(f => f.ClientId == filters.ClientId.Value);

            if (filters.TrainerId.HasValue)
                query = query.Where(f => f.TrainerId == filters.TrainerId.Value);

            if (filters.Type.HasValue)
                query = query.Where(f => f.Type == filters.Type.Value);

            if (filters.Category.HasValue)
                query = query.Where(f => f.Category == filters.Category.Value);

            if (filters.Status.HasValue)
                query = query.Where(f => f.Status == filters.Status.Value);

            if (filters.MinRating.HasValue)
                query = query.Where(f => f.Rating >= filters.MinRating.Value);

            if (filters.MaxRating.HasValue)
                query = query.Where(f => f.Rating <= filters.MaxRating.Value);

            if (filters.StartDate.HasValue)
                query = query.Where(f => f.CreatedDate >= filters.StartDate.Value);

            if (filters.EndDate.HasValue)
                query = query.Where(f => f.CreatedDate <= filters.EndDate.Value);

            if (filters.IsAnonymous.HasValue)
                query = query.Where(f => f.IsAnonymous == filters.IsAnonymous.Value);

            if (!string.IsNullOrEmpty(filters.Search))
            {
                query = query.Where(f =>
                    f.Title.Contains(filters.Search) ||
                    (f.Description != null && f.Description.Contains(filters.Search)));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var feedbacks = await query
                .OrderByDescending(f => f.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(f => new FeedbackResponse
                {
                    Id = f.Id,
                    ClientId = f.ClientId,
                    ClientName = f.IsAnonymous ? "Anônimo" : (f.Client != null ? f.Client.Name : null),
                    TrainerId = f.TrainerId,
                    TrainerName = f.Trainer != null ? f.Trainer.Name : null,
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
                })
                .ToListAsync();

            return new FeedbacksPagedDTO
            {
                Feedbacks = feedbacks,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages
            };
        }

        public async Task<FeedbackStatsDTO> GetFeedbackStatsAsync()
        {
            var feedbacks = await Feedbacks.ToListAsync();

            return new FeedbackStatsDTO
            {
                TotalFeedbacks = feedbacks.Count,
                PendingFeedbacks = feedbacks.Count(f => f.Status == Core.Enums.FeedbackStatus.Pending),
                ResolvedFeedbacks = feedbacks.Count(f => f.Status == Core.Enums.FeedbackStatus.Resolved),
                AverageRating = feedbacks.Any() ? feedbacks.Average(f => f.Rating) : 0,
                FeedbacksByType = feedbacks
                    .GroupBy(f => f.Type.ToString())
                    .ToDictionary(g => g.Key, g => g.Count()),
                FeedbacksByCategory = feedbacks
                    .GroupBy(f => f.Category.ToString())
                    .ToDictionary(g => g.Key, g => g.Count()),
                FeedbacksByStatus = feedbacks
                    .GroupBy(f => f.Status.ToString())
                    .ToDictionary(g => g.Key, g => g.Count())
            };
        }
    }
}
