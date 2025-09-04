using System;
using System.Collections.Generic;
using Core.Enums;
using Core.Models.Client;

namespace Core.Models.Message
{
    public class Conversation : BaseModel
    {
        public int? EmpresasId { get; set; }
        public ConversationType Type { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool IsArchived { get; set; }
        public bool IsMuted { get; set; }
        public int UnreadCount { get; set; }
        
        // Navigation properties
        public Empresas? Empresas { get; set; }
        public ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public Message? LastMessage { get; set; }
        public int? LastMessageId { get; set; }
        
        // Settings as owned entity
        public ConversationSettings Settings { get; set; } = new ConversationSettings();
    }
}
