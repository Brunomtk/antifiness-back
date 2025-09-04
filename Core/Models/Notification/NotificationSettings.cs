using System;
using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Core.Models.Notification
{
    public class NotificationSettings : BaseModel
    {
        [Required]
        public int UserId { get; set; }
        
        public bool EmailNotifications { get; set; } = true;
        
        public bool PushNotifications { get; set; } = true;
        
        public bool SmsNotifications { get; set; } = false;
        
        public NotificationCategorySettings Categories { get; set; } = new();
        
        public NotificationTypeSettings Types { get; set; } = new();
        
        public QuietHours QuietHours { get; set; } = new();
        
        public NotificationFrequency Frequency { get; set; } = NotificationFrequency.Immediate;
        
        // Navigation properties
        public User User { get; set; } = null!;
    }
}
