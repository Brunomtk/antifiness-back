using System.Collections.Generic;

namespace Core.DTO.Message
{
    public class MessagesPagedDTO
    {
        public List<MessageResponse> Messages { get; set; } = new List<MessageResponse>();
        public bool HasMore { get; set; }
        public int Total { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
    }
}
