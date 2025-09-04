using System;
using Core.Enums;
using Core.Models.Notification;

namespace Core.DTO.Notification
{
    public class NotificationSettingsResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public bool EmailNotifications { get; set; }
        public bool PushNotifications { get; set; }
        public bool SmsNotifications { get; set; }
        public NotificationCategorySettings Categories { get; set; } = new();
        public NotificationTypeSettings Types { get; set; } = new();
        public QuietHours QuietHours { get; set; } = new();
        public NotificationFrequency Frequency { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
