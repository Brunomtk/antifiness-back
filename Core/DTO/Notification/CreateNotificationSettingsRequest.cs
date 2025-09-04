using Core.Enums;
using Core.Models.Notification;

namespace Core.DTO.Notification
{
    public class CreateNotificationSettingsRequest
    {
        public int UserId { get; set; }
        public bool EmailNotifications { get; set; } = true;
        public bool PushNotifications { get; set; } = true;
        public bool SmsNotifications { get; set; } = false;
        public NotificationCategorySettings Categories { get; set; } = new();
        public NotificationTypeSettings Types { get; set; } = new();
        public QuietHours QuietHours { get; set; } = new();
        public NotificationFrequency Frequency { get; set; } = NotificationFrequency.Immediate;
    }
}
