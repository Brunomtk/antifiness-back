namespace Core.Models.Message
{
    public class ConversationSettings
    {
        public bool Notifications { get; set; } = true;
        public bool SoundEnabled { get; set; } = true;
        public bool AutoArchive { get; set; } = false;
        public int? RetentionDays { get; set; }
    }
}
