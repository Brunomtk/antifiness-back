namespace Core.DTO.Message
{
    public class UpdateConversationRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool? IsArchived { get; set; }
        public bool? IsMuted { get; set; }
        public ConversationSettingsRequest? Settings { get; set; }
    }
}
