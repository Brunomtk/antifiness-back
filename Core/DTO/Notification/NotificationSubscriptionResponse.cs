using System;

namespace Core.DTO.Notification
{
    public class NotificationSubscriptionResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Endpoint { get; set; } = string.Empty;
        public PushKeysResponse Keys { get; set; } = new();
        public string? UserAgent { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUsed { get; set; }
    }
    
    public class PushKeysResponse
    {
        public string P256dh { get; set; } = string.Empty;
        public string Auth { get; set; } = string.Empty;
    }
}
