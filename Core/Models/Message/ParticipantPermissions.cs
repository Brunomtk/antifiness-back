namespace Core.Models.Message
{
    public class ParticipantPermissions
    {
        public bool CanSendMessages { get; set; } = true;
        public bool CanSendAttachments { get; set; } = true;
        public bool CanDeleteMessages { get; set; } = false;
        public bool CanAddParticipants { get; set; } = false;
        public bool CanRemoveParticipants { get; set; } = false;
        public bool CanEditConversation { get; set; } = false;
    }
}
