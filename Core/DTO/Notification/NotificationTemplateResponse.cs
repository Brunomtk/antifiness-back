using System;
using Core.Enums;

namespace Core.DTO.Notification
{
    public class NotificationTemplateResponse
    {
        public int Id { get; set; }
        public NotificationType Type { get; set; }
        public NotificationCategory Category { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string[] Variables { get; set; } = Array.Empty<string>();
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
