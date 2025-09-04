using Core.Enums;
using Core.Models.Notification;

namespace Core.DTO.Notification
{
    public class UpdateNotificationSettingsRequest
    {
        public bool? EmailNotifications { get; set; }
        public bool? PushNotifications { get; set; }
        public bool? SmsNotifications { get; set; }
        public NotificationCategorySettings? Categories { get; set; }
        public NotificationTypeSettings? Types { get; set; }
        public QuietHours? QuietHours { get; set; }
        public NotificationFrequency? Frequency { get; set; }
    }
}
