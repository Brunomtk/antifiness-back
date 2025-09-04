using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.DTO.Notification;
using Core.Models.Notification;
using Infrastructure.ServiceExtension;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>
    {
        public NotificationRepository(DbContextClass context) : base(context)
        {
        }

        public async Task<NotificationsPagedDTO> GetPagedNotificationsAsync(NotificationFiltersDTO filters)
        {
            var query = _dbContext.Notifications.AsQueryable();

            // Apply filters
            if (filters.Type != null && filters.Type.Length > 0)
            {
                query = query.Where(n => filters.Type.Contains(n.Type));
            }

            if (filters.Category != null && filters.Category.Length > 0)
            {
                query = query.Where(n => filters.Category.Contains(n.Category));
            }

            if (filters.Priority != null && filters.Priority.Length > 0)
            {
                query = query.Where(n => filters.Priority.Contains(n.Priority));
            }

            if (filters.Read.HasValue)
            {
                query = query.Where(n => n.Read == filters.Read.Value);
            }

            if (filters.StartDate.HasValue)
            {
                query = query.Where(n => n.CreatedDate >= filters.StartDate.Value);
            }

            if (filters.EndDate.HasValue)
            {
                query = query.Where(n => n.CreatedDate <= filters.EndDate.Value);
            }

            if (!string.IsNullOrEmpty(filters.Search))
            {
                var searchLower = filters.Search.ToLower();
                query = query.Where(n =>
                    n.Title.ToLower().Contains(searchLower) ||
                    n.Message.ToLower().Contains(searchLower));
            }

            var total = await query.CountAsync();

            var notifications = await query
                .OrderByDescending(n => n.CreatedDate)
                .Skip((filters.Page - 1) * filters.Limit)
                .Take(filters.Limit)
                .Select(n => new NotificationResponse
                {
                    Id = n.Id,
                    UserId = n.UserId,
                    Type = n.Type,
                    Category = n.Category,
                    Title = n.Title,
                    Message = n.Message,
                    Data = n.Data,
                    Read = n.Read,
                    Priority = n.Priority,
                    CreatedAt = n.CreatedDate,
                    ReadAt = n.ReadAt,
                    ExpiresAt = n.ExpiresAt,
                    ActionUrl = n.ActionUrl,
                    ActionLabel = n.ActionLabel
                })
                .ToListAsync();

            return new NotificationsPagedDTO
            {
                Notifications = notifications,
                HasMore = (filters.Page * filters.Limit) < total,
                Total = total,
                Page = filters.Page,
                Limit = filters.Limit
            };
        }

        public async Task<int> MarkAllAsReadAsync(int userId)
        {
            var notifications = await _dbContext.Notifications
                .Where(n => n.UserId == userId && !n.Read)
                .ToListAsync();

            var now = DateTime.UtcNow;
            foreach (var notification in notifications)
            {
                notification.Read = true;
                notification.ReadAt = now;
            }

            return await _dbContext.SaveChangesAsync();
        }

        public async Task<NotificationStatsDTO> GetNotificationStatsAsync(int? userId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbContext.Notifications.AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(n => n.UserId == userId.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(n => n.CreatedDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(n => n.CreatedDate <= endDate.Value);
            }

            var notifications = await query.ToListAsync();
            var total = notifications.Count;
            var unread = notifications.Count(n => !n.Read);

            var byType = notifications
                .GroupBy(n => n.Type.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            var byCategory = notifications
                .GroupBy(n => n.Category.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            var byPriority = notifications
                .GroupBy(n => n.Priority.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            var readRate = total > 0 ? (double)(total - unread) / total * 100 : 0;

            var readNotifications = notifications.Where(n => n.Read && n.ReadAt.HasValue).ToList();
            var averageReadTime = readNotifications.Count > 0
                ? readNotifications.Average(n => (n.ReadAt!.Value - n.CreatedDate).TotalMinutes)
                : 0;

            var mostActiveHour = notifications
                .GroupBy(n => n.CreatedDate.Hour)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault()?.Key ?? 0;

            var weeklyTrend = new double[7];
            var now = DateTime.UtcNow;
            for (int i = 0; i < 7; i++)
            {
                var date = now.AddDays(-i).Date;
                weeklyTrend[6 - i] = notifications.Count(n => n.CreatedDate.Date == date);
            }

            return new NotificationStatsDTO
            {
                Total = total,
                Unread = unread,
                ByType = byType,
                ByCategory = byCategory,
                ByPriority = byPriority,
                ReadRate = readRate,
                AverageReadTime = averageReadTime,
                MostActiveHour = mostActiveHour,
                WeeklyTrend = weeklyTrend
            };
        }

        public async Task<List<Notification>> GetUnreadNotificationsAsync(int userId)
        {
            return await _dbContext.Notifications
                .Where(n => n.UserId == userId && !n.Read)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetExpiredNotificationsAsync()
        {
            var now = DateTime.UtcNow;
            return await _dbContext.Notifications
                .Where(n => n.ExpiresAt.HasValue && n.ExpiresAt.Value <= now)
                .ToListAsync();
        }
    }
}
