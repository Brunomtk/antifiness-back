using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Models.Notification
{
    public class NotificationSubscription : BaseModel
    {
        [Required]
        public int UserId { get; set; }
        
        [Required]
        [MaxLength(500)]
        public string Endpoint { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(200)]
        public string P256dhKey { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(200)]
        public string AuthKey { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? UserAgent { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime? LastUsed { get; set; }
        
        // Navigation properties
        public User User { get; set; } = null!;
    }
}
