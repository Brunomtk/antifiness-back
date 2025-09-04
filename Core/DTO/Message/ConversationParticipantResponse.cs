using System;
using Core.Enums;

namespace Core.DTO.Message
{
    public class ConversationParticipantResponse
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public ParticipantRole Role { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime? LastSeenAt { get; set; }
        public bool IsOnline { get; set; }
        public ParticipantPermissionsResponse Permissions { get; set; } = new ParticipantPermissionsResponse();
    }

    public class ParticipantPermissionsResponse
    {
        public bool CanSendMessages { get; set; }
        public bool CanSendAttachments { get; set; }
        public bool CanDeleteMessages { get; set; }
        public bool CanAddParticipants { get; set; }
        public bool CanRemoveParticipants { get; set; }
        public bool CanEditConversation { get; set; }
    }
}
