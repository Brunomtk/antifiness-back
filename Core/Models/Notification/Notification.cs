using System;
using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Core.Models.Notification
{
    public class Notification : BaseModel
    {
        [Required]
        public int UserId { get; set; }
        
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
        
        public NotificationData? Data { get; set; }
        
        public bool Read { get; set; } = false;
        
        public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
        
        public DateTime? ReadAt { get; set; }
        
        public DateTime? ExpiresAt { get; set; }
        
        [MaxLength(500)]
        public string? ActionUrl { get; set; }
        
        [MaxLength(100)]
        public string? ActionLabel { get; set; }
        
        // Navigation properties
        public User User { get; set; } = null!;
    }
}
