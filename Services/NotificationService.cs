using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Core.DTO.Notification;
using Core.Models.Notification;
using Infrastructure.Repositories;

namespace Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<NotificationsPagedDTO> GetPagedNotificationsAsync(NotificationFiltersDTO filters)
        {
            var notificationRepo = _unitOfWork.Notifications as NotificationRepository;
            return await notificationRepo!.GetPagedNotificationsAsync(filters);
        }

        public async Task<NotificationResponse> GetNotificationByIdAsync(int id)
        {
            var all = await _unitOfWork.Notifications.GetAll();
            var notification = all.FirstOrDefault(n => n.Id == id);
            if (notification == null)
                throw new KeyNotFoundException($"Notification with ID {id} not found.");

            return new NotificationResponse
            {
                Id = notification.Id,
                UserId = notification.UserId,
                Type = notification.Type,
                Category = notification.Category,
                Title = notification.Title,
                Message = notification.Message,
                Data = notification.Data,
                Read = notification.Read,
                Priority = notification.Priority,
                CreatedAt = notification.CreatedDate,
                ReadAt = notification.ReadAt,
                ExpiresAt = notification.ExpiresAt,
                ActionUrl = notification.ActionUrl,
                ActionLabel = notification.ActionLabel
            };
        }

        public async Task<NotificationResponse> CreateNotificationAsync(CreateNotificationRequest request)
        {
            var notification = new Notification
            {
                UserId = request.UserId,
                Type = request.Type,
                Category = request.Category,
                Title = request.Title,
                Message = request.Message,
                Data = request.Data,
                Priority = request.Priority,
                ExpiresAt = request.ExpiresAt,
                ActionUrl = request.ActionUrl,
                ActionLabel = request.ActionLabel
            };

            await _unitOfWork.Notifications.Add(notification);
            await _unitOfWork.SaveAsync();

            return new NotificationResponse
            {
                Id = notification.Id,
                UserId = notification.UserId,
                Type = notification.Type,
                Category = notification.Category,
                Title = notification.Title,
                Message = notification.Message,
                Data = notification.Data,
                Read = notification.Read,
                Priority = notification.Priority,
                CreatedAt = notification.CreatedDate,
                ReadAt = notification.ReadAt,
                ExpiresAt = notification.ExpiresAt,
                ActionUrl = notification.ActionUrl,
                ActionLabel = notification.ActionLabel
            };
        }

        public async Task<NotificationResponse> UpdateNotificationAsync(int id, UpdateNotificationRequest request)
        {
            var all = await _unitOfWork.Notifications.GetAll();
            var notification = all.FirstOrDefault(n => n.Id == id);
            if (notification == null)
                throw new KeyNotFoundException($"Notification with ID {id} not found.");

            if (request.Read.HasValue)
            {
                notification.Read = request.Read.Value;
                if (request.Read.Value && !notification.ReadAt.HasValue)
                {
                    notification.ReadAt = request.ReadAt ?? DateTime.UtcNow;
                }
            }

            _unitOfWork.Notifications.Update(notification);
            await _unitOfWork.SaveAsync();

            return new NotificationResponse
            {
                Id = notification.Id,
                UserId = notification.UserId,
                Type = notification.Type,
                Category = notification.Category,
                Title = notification.Title,
                Message = notification.Message,
                Data = notification.Data,
                Read = notification.Read,
                Priority = notification.Priority,
                CreatedAt = notification.CreatedDate,
                ReadAt = notification.ReadAt,
                ExpiresAt = notification.ExpiresAt,
                ActionUrl = notification.ActionUrl,
                ActionLabel = notification.ActionLabel
            };
        }

        public async Task<bool> DeleteNotificationAsync(int id)
        {
            var all = await _unitOfWork.Notifications.GetAll();
            var notification = all.FirstOrDefault(n => n.Id == id);
            if (notification == null)
                return false;

            _unitOfWork.Notifications.Delete(notification);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<int> MarkAllAsReadAsync(int userId)
        {
            var notificationRepo = _unitOfWork.Notifications as NotificationRepository;
            return await notificationRepo!.MarkAllAsReadAsync(userId);
        }

        public async Task<NotificationStatsDTO> GetNotificationStatsAsync(int? userId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var notificationRepo = _unitOfWork.Notifications as NotificationRepository;
            return await notificationRepo!.GetNotificationStatsAsync(userId, startDate, endDate);
        }

        public async Task<NotificationSettingsResponse> GetNotificationSettingsAsync(int userId)
        {
            var all = await _unitOfWork.NotificationSettings.GetAll();
            var settings = all.FirstOrDefault(s => s.UserId == userId);

            if (settings == null)
            {
                settings = new NotificationSettings { UserId = userId };
                await _unitOfWork.NotificationSettings.Add(settings);
                await _unitOfWork.SaveAsync();
            }

            return new NotificationSettingsResponse
            {
                Id = settings.Id,
                UserId = settings.UserId,
                EmailNotifications = settings.EmailNotifications,
                PushNotifications = settings.PushNotifications,
                SmsNotifications = settings.SmsNotifications,
                Categories = settings.Categories,
                Types = settings.Types,
                QuietHours = settings.QuietHours,
                Frequency = settings.Frequency,
                UpdatedAt = settings.UpdatedDate
            };
        }

        public async Task<NotificationSettingsResponse> UpdateNotificationSettingsAsync(int userId, UpdateNotificationSettingsRequest request)
        {
            var all = await _unitOfWork.NotificationSettings.GetAll();
            var settings = all.FirstOrDefault(s => s.UserId == userId);

            if (settings == null)
            {
                settings = new NotificationSettings { UserId = userId };
                await _unitOfWork.NotificationSettings.Add(settings);
            }

            if (request.EmailNotifications.HasValue)
                settings.EmailNotifications = request.EmailNotifications.Value;

            if (request.PushNotifications.HasValue)
                settings.PushNotifications = request.PushNotifications.Value;

            if (request.SmsNotifications.HasValue)
                settings.SmsNotifications = request.SmsNotifications.Value;

            if (request.Categories != null)
                settings.Categories = request.Categories;

            if (request.Types != null)
                settings.Types = request.Types;

            if (request.QuietHours != null)
                settings.QuietHours = request.QuietHours;

            if (request.Frequency.HasValue)
                settings.Frequency = request.Frequency.Value;

            // se Id == 0 ainda não foi salvo (Add acima cuidará disso)
            _unitOfWork.NotificationSettings.Update(settings);
            await _unitOfWork.SaveAsync();

            return new NotificationSettingsResponse
            {
                Id = settings.Id,
                UserId = settings.UserId,
                EmailNotifications = settings.EmailNotifications,
                PushNotifications = settings.PushNotifications,
                SmsNotifications = settings.SmsNotifications,
                Categories = settings.Categories,
                Types = settings.Types,
                QuietHours = settings.QuietHours,
                Frequency = settings.Frequency,
                UpdatedAt = settings.UpdatedDate
            };
        }

        public async Task<List<NotificationTemplateResponse>> GetNotificationTemplatesAsync()
        {
            var templates = await _unitOfWork.NotificationTemplates.GetAll();

            return templates.Select(t => new NotificationTemplateResponse
            {
                Id = t.Id,
                Type = t.Type,
                Category = t.Category,
                Title = t.Title,
                Message = t.Message,
                Variables = string.IsNullOrEmpty(t.Variables)
                    ? Array.Empty<string>()
                    : (JsonSerializer.Deserialize<string[]>(t.Variables) ?? Array.Empty<string>()),
                IsActive = t.IsActive,
                CreatedAt = t.CreatedDate,
                UpdatedAt = t.UpdatedDate
            }).ToList();
        }

        public async Task<NotificationTemplateResponse> CreateNotificationTemplateAsync(CreateNotificationTemplateRequest request)
        {
            var template = new NotificationTemplate
            {
                Type = request.Type,
                Category = request.Category,
                Title = request.Title,
                Message = request.Message,
                Variables = JsonSerializer.Serialize(request.Variables),
                IsActive = request.IsActive
            };

            await _unitOfWork.NotificationTemplates.Add(template);
            await _unitOfWork.SaveAsync();

            return new NotificationTemplateResponse
            {
                Id = template.Id,
                Type = template.Type,
                Category = template.Category,
                Title = template.Title,
                Message = template.Message,
                Variables = request.Variables,
                IsActive = template.IsActive,
                CreatedAt = template.CreatedDate,
                UpdatedAt = template.UpdatedDate
            };
        }

        public async Task<NotificationTemplateResponse> UpdateNotificationTemplateAsync(int id, UpdateNotificationTemplateRequest request)
        {
            var all = await _unitOfWork.NotificationTemplates.GetAll();
            var template = all.FirstOrDefault(t => t.Id == id);
            if (template == null)
                throw new KeyNotFoundException($"Notification template with ID {id} not found.");

            template.Type = request.Type;
            template.Category = request.Category;
            template.Title = request.Title;
            template.Message = request.Message;
            template.Variables = JsonSerializer.Serialize(request.Variables);
            template.IsActive = request.IsActive;

            _unitOfWork.NotificationTemplates.Update(template);
            await _unitOfWork.SaveAsync();

            return new NotificationTemplateResponse
            {
                Id = template.Id,
                Type = template.Type,
                Category = template.Category,
                Title = template.Title,
                Message = template.Message,
                Variables = request.Variables,
                IsActive = template.IsActive,
                CreatedAt = template.CreatedDate,
                UpdatedAt = template.UpdatedDate
            };
        }

        public async Task<bool> DeleteNotificationTemplateAsync(int id)
        {
            var all = await _unitOfWork.NotificationTemplates.GetAll();
            var template = all.FirstOrDefault(t => t.Id == id);
            if (template == null)
                return false;

            _unitOfWork.NotificationTemplates.Delete(template);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<NotificationSubscriptionResponse> CreateNotificationSubscriptionAsync(CreateNotificationSubscriptionRequest request)
        {
            var subscription = new NotificationSubscription
            {
                UserId = request.UserId,
                Endpoint = request.Endpoint,
                P256dhKey = request.Keys.P256dh,
                AuthKey = request.Keys.Auth,
                UserAgent = request.UserAgent
            };

            await _unitOfWork.NotificationSubscriptions.Add(subscription);
            await _unitOfWork.SaveAsync();

            return new NotificationSubscriptionResponse
            {
                Id = subscription.Id,
                UserId = subscription.UserId,
                Endpoint = subscription.Endpoint,
                Keys = new PushKeysResponse
                {
                    P256dh = subscription.P256dhKey,
                    Auth = subscription.AuthKey
                },
                UserAgent = subscription.UserAgent,
                IsActive = subscription.IsActive,
                CreatedAt = subscription.CreatedDate,
                LastUsed = subscription.LastUsed
            };
        }

        public async Task<bool> DeleteNotificationSubscriptionAsync(int id)
        {
            var all = await _unitOfWork.NotificationSubscriptions.GetAll();
            var subscription = all.FirstOrDefault(s => s.Id == id);
            if (subscription == null)
                return false;

            _unitOfWork.NotificationSubscriptions.Delete(subscription);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> SendPushNotificationAsync(SendPushNotificationRequest request)
        {
            await Task.CompletedTask; // placeholder de integração com push provider
            return true;
        }
    }
}
