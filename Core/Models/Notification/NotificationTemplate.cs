using System;
using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Core.Models.Notification
{
    public class NotificationTemplate : BaseModel
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
        
        [MaxLength(2000)]
        public string Variables { get; set; } = string.Empty; // JSON array
        
        public bool IsActive { get; set; } = true;
    }
}
