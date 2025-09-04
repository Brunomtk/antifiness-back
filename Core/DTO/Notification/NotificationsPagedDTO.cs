using System.Collections.Generic;

namespace Core.DTO.Notification
{
    public class NotificationsPagedDTO
    {
        public List<NotificationResponse> Notifications { get; set; } = new();
        public bool HasMore { get; set; }
        public int Total { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
    }
}
