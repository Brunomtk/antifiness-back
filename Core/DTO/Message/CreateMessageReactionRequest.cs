namespace Core.DTO.Message
{
    public class CreateMessageReactionRequest
    {
        public int UserId { get; set; }
        public string Emoji { get; set; } = string.Empty;
    }
}
