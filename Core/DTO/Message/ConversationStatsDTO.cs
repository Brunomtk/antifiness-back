using System;

namespace Core.DTO.Message
{
    public class ConversationStatsDTO
    {
        public int MessageCount { get; set; }
        public int ParticipantCount { get; set; }
        public double AverageResponseTime { get; set; }
        public DateTime? LastActivity { get; set; }
        public int AttachmentCount { get; set; }
        public int ReactionCount { get; set; }
    }
}
