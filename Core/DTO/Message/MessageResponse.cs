using System;
using System.Collections.Generic;
using Core.Enums;

namespace Core.DTO.Message
{
    public class MessageResponse
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public int? ReceiverId { get; set; }
        public string? ReceiverName { get; set; }
        public MessageType Type { get; set; }
        public MessageStatus Status { get; set; }
        public string Content { get; set; } = string.Empty;
        public int? ReplyToId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public List<MessageAttachmentResponse> Attachments { get; set; } = new List<MessageAttachmentResponse>();
        public List<MessageReactionResponse> Reactions { get; set; } = new List<MessageReactionResponse>();
        public MessageMetadataResponse Metadata { get; set; } = new MessageMetadataResponse();
        public MessageResponse? ReplyTo { get; set; }
    }

    public class MessageAttachmentResponse
    {
        public int Id { get; set; }
        public AttachmentType Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public long Size { get; set; }
        public string MimeType { get; set; } = string.Empty;
        public string? Thumbnail { get; set; }
        public AttachmentMetadataResponse Metadata { get; set; } = new AttachmentMetadataResponse();
    }

    public class MessageReactionResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Emoji { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class MessageMetadataResponse
    {
        public bool Edited { get; set; }
        public string? EditReason { get; set; }
        public string? SystemAction { get; set; }
        public string? CustomData { get; set; }
    }

    public class AttachmentMetadataResponse
    {
        public int? Width { get; set; }
        public int? Height { get; set; }
        public int? Duration { get; set; }
        public string? Description { get; set; }
        public string? CustomData { get; set; }
    }
}
