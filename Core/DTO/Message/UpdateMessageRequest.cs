using System.Collections.Generic;

namespace Core.DTO.Message
{
    public class UpdateMessageRequest
    {
        public string? Content { get; set; }
        public List<CreateMessageAttachmentRequest>? Attachments { get; set; }
        public MessageMetadataRequest? Metadata { get; set; }
    }
}
