using System.Collections.Generic;

namespace Core.DTO.Message
{
    public class MarkMessagesAsReadRequest
    {
        public List<int> MessageIds { get; set; } = new List<int>();
        public int ReaderId { get; set; }
    }
}
