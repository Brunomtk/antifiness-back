using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DTO.Notification;
using Core.Models.Notification;

namespace Services
{
    public interface INotificationService
    {
        Task<NotificationsPagedDTO> GetPagedNotificationsAsync(NotificationFiltersDTO filters);
        Task<NotificationResponse> GetNotificationByIdAsync(int id);
        Task<NotificationResponse> CreateNotificationAsync(CreateNotificationRequest request);
        Task<NotificationResponse> UpdateNotificationAsync(int id, UpdateNotificationRequest request);
        Task<bool> DeleteNotificationAsync(int id);
        Task<int> MarkAllAsReadAsync(int userId);
        Task<NotificationStatsDTO> GetNotificationStatsAsync(int? userId = null, System.DateTime? startDate = null, System.DateTime? endDate = null);
        
        // Settings
        Task<NotificationSettingsResponse> GetNotificationSettingsAsync(int userId);
        Task<NotificationSettingsResponse> UpdateNotificationSettingsAsync(int userId, UpdateNotificationSettingsRequest request);
        
        // Templates
        Task<List<NotificationTemplateResponse>> GetNotificationTemplatesAsync();
        Task<NotificationTemplateResponse> CreateNotificationTemplateAsync(CreateNotificationTemplateRequest request);
        Task<NotificationTemplateResponse> UpdateNotificationTemplateAsync(int id, UpdateNotificationTemplateRequest request);
        Task<bool> DeleteNotificationTemplateAsync(int id);
        
        // Subscriptions
        Task<NotificationSubscriptionResponse> CreateNotificationSubscriptionAsync(CreateNotificationSubscriptionRequest request);
        Task<bool> DeleteNotificationSubscriptionAsync(int id);
        
        // Push notifications
        Task<bool> SendPushNotificationAsync(SendPushNotificationRequest request);
    }
}
