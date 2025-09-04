using System.Collections.Generic;

namespace Core.DTO.Message
{
    public class ConversationsPagedDTO
    {
        public List<ConversationResponse> Conversations { get; set; } = new List<ConversationResponse>();
        public bool HasMore { get; set; }
        public int Total { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
    }
}
