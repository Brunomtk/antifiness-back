namespace Core.Models.Notification
{
    public class NotificationTypeSettings
    {
        public bool System { get; set; } = true;
        public bool Diet { get; set; } = true;
        public bool Workout { get; set; } = true;
        public bool Plan { get; set; } = true;
        public bool Message { get; set; } = true;
        public bool Reminder { get; set; } = true;
        public bool Achievement { get; set; } = true;
        public bool Alert { get; set; } = true;
    }
}
