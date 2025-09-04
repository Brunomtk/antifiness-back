using System;
using System.Collections.Generic;
using Core.Enums;

namespace Core.DTO.Message
{
    public class ConversationResponse
    {
        public int Id { get; set; }
        public int? EmpresasId { get; set; }
        public ConversationType Type { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool IsArchived { get; set; }
        public bool IsMuted { get; set; }
        public int UnreadCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<ConversationParticipantResponse> Participants { get; set; } = new List<ConversationParticipantResponse>();
        public MessageResponse? LastMessage { get; set; }
        public ConversationSettingsResponse Settings { get; set; } = new ConversationSettingsResponse();
    }

    public class ConversationSettingsResponse
    {
        public bool Notifications { get; set; }
        public bool SoundEnabled { get; set; }
        public bool AutoArchive { get; set; }
        public int? RetentionDays { get; set; }
    }
}
