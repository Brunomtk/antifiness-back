using System.Collections.Generic;
using Core.Enums;

namespace Core.DTO.Message
{
    public class CreateMessageRequest
    {
        public int ConversationId { get; set; }
        public int SenderId { get; set; }
        public int? ReceiverId { get; set; }
        public MessageType Type { get; set; } = MessageType.Text;
        public string Content { get; set; } = string.Empty;
        public int? ReplyToId { get; set; }
        public List<CreateMessageAttachmentRequest>? Attachments { get; set; }
        public MessageMetadataRequest? Metadata { get; set; }
    }

    public class CreateMessageAttachmentRequest
    {
        public int? Id { get; set; }
        public AttachmentType Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public long Size { get; set; }
        public string MimeType { get; set; } = string.Empty;
        public string? Thumbnail { get; set; }
        public AttachmentMetadataRequest? Metadata { get; set; }
    }

    public class MessageMetadataRequest
    {
        public string? SystemAction { get; set; }
        public string? CustomData { get; set; }
    }

    public class AttachmentMetadataRequest
    {
        public int? Width { get; set; }
        public int? Height { get; set; }
        public int? Duration { get; set; }
        public string? Description { get; set; }
        public string? CustomData { get; set; }
    }
}
