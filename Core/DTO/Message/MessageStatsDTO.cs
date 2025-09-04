namespace Core.DTO.Message
{
    public class MessageStatsDTO
    {
        public int TotalMessages { get; set; }
        public int TotalConversations { get; set; }
        public int UnreadMessages { get; set; }
        public int ActiveConversations { get; set; }
        public int MessagesThisWeek { get; set; }
        public double AverageResponseTime { get; set; }
        public int MostActiveHour { get; set; }
        public int AttachmentsSent { get; set; }
    }
}
