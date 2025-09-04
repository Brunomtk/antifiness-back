using System;
using System.Collections.Generic;
using Core.Enums;

namespace Core.Models.Message
{
    public class Message : BaseModel
    {
        public int ConversationId { get; set; }
        public int SenderId { get; set; }
        public int? ReceiverId { get; set; }
        public MessageType Type { get; set; }
        public MessageStatus Status { get; set; }
        public string Content { get; set; } = string.Empty;
        public int? ReplyToId { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        
        // Navigation properties
        public Conversation Conversation { get; set; } = null!;
        public User Sender { get; set; } = null!;
        public User? Receiver { get; set; }
        public Message? ReplyTo { get; set; }
        public ICollection<Message> Replies { get; set; } = new List<Message>();
        public ICollection<MessageAttachment> Attachments { get; set; } = new List<MessageAttachment>();
        public ICollection<MessageReaction> Reactions { get; set; } = new List<MessageReaction>();
        
        // Metadata as owned entity
        public MessageMetadata Metadata { get; set; } = new MessageMetadata();
    }
}
