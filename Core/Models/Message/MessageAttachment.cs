using Core.Enums;

namespace Core.Models.Message
{
    public class MessageAttachment : BaseModel
    {
        public int MessageId { get; set; }
        public AttachmentType Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public long Size { get; set; }
        public string MimeType { get; set; } = string.Empty;
        public string? Thumbnail { get; set; }
        
        // Navigation properties
        public Message Message { get; set; } = null!;
        
        // Metadata as owned entity
        public AttachmentMetadata Metadata { get; set; } = new AttachmentMetadata();
    }
}
