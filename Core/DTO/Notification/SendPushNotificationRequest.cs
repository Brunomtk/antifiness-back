using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Notification
{
    public class SendPushNotificationRequest
    {
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public PushNotificationPayload Payload { get; set; } = new();
    }
    
    public class PushNotificationPayload
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Body { get; set; } = string.Empty;
        
        public string? Icon { get; set; }
        public int? Badge { get; set; }
        public object? Data { get; set; }
        public PushNotificationAction[]? Actions { get; set; }
    }
    
    public class PushNotificationAction
    {
        [Required]
        public string Action { get; set; } = string.Empty;
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        public string? Icon { get; set; }
    }
}
