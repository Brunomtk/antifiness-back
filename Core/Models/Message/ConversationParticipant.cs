using System;
using Core.Enums;

namespace Core.Models.Message
{
    public class ConversationParticipant : BaseModel
    {
        public int ConversationId { get; set; }
        public int UserId { get; set; }
        public ParticipantRole Role { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime? LastSeenAt { get; set; }
        public bool IsOnline { get; set; }
        
        // Navigation properties
        public Conversation Conversation { get; set; } = null!;
        public User User { get; set; } = null!;
        
        // Permissions as owned entity
        public ParticipantPermissions Permissions { get; set; } = new ParticipantPermissions();
    }
}
