namespace Core.Models.Notification
{
    public class NotificationCategorySettings
    {
        public bool Info { get; set; } = true;
        public bool Success { get; set; } = true;
        public bool Warning { get; set; } = true;
        public bool Error { get; set; } = true;
        public bool Reminder { get; set; } = true;
    }
}
