using System;

namespace Core.Models.Message
{
    public class MessageReaction : BaseModel
    {
        public int MessageId { get; set; }
        public int UserId { get; set; }
        public string Emoji { get; set; } = string.Empty;
        
        // Navigation properties
        public Message Message { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
