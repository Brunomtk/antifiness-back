using System;
using Core.Enums;

namespace Core.DTO.Notification
{
    public class NotificationFiltersDTO
    {
        public NotificationType[]? Type { get; set; }
        public NotificationCategory[]? Category { get; set; }
        public NotificationPriority[]? Priority { get; set; }
        public bool? Read { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 20;
    }
}
