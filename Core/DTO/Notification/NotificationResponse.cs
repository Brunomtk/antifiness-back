using System;
using Core.Enums;
using Core.Models.Notification;

namespace Core.DTO.Notification
{
    public class NotificationResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public NotificationType Type { get; set; }
        public NotificationCategory Category { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationData? Data { get; set; }
        public bool Read { get; set; }
        public NotificationPriority Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string? ActionUrl { get; set; }
        public string? ActionLabel { get; set; }
    }
}
