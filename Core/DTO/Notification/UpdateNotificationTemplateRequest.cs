using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Core.DTO.Notification
{
    public class UpdateNotificationTemplateRequest
    {
        [Required]
        public NotificationType Type { get; set; }
        
        [Required]
        public NotificationCategory Category { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(1000)]
        public string Message { get; set; } = string.Empty;
        
        public string[] Variables { get; set; } = Array.Empty<string>();
        
        public bool IsActive { get; set; } = true;
    }
}
