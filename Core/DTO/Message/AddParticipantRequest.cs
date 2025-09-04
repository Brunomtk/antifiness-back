using Core.Enums;

namespace Core.DTO.Message
{
    public class AddParticipantRequest
    {
        public int UserId { get; set; }
        public ParticipantRole Role { get; set; } = ParticipantRole.Member;
        public ParticipantPermissionsRequest? Permissions { get; set; }
    }

    public class ParticipantPermissionsRequest
    {
        public bool? CanSendMessages { get; set; }
        public bool? CanSendAttachments { get; set; }
        public bool? CanDeleteMessages { get; set; }
        public bool? CanAddParticipants { get; set; }
        public bool? CanRemoveParticipants { get; set; }
        public bool? CanEditConversation { get; set; }
    }
}
