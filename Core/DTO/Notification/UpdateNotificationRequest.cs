using System;

namespace Core.DTO.Notification
{
    public class UpdateNotificationRequest
    {
        public bool? Read { get; set; }
        public DateTime? ReadAt { get; set; }
    }
}
