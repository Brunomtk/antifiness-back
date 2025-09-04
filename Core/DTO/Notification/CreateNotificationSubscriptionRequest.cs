using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Notification
{
    public class CreateNotificationSubscriptionRequest
    {
        [Required]
        public int UserId { get; set; }
        
        [Required]
        [MaxLength(500)]
        public string Endpoint { get; set; } = string.Empty;
        
        [Required]
        public PushKeys Keys { get; set; } = new();
        
        [MaxLength(500)]
        public string? UserAgent { get; set; }
    }
    
    public class PushKeys
    {
        [Required]
        [MaxLength(200)]
        public string P256dh { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(200)]
        public string Auth { get; set; } = string.Empty;
    }
}
