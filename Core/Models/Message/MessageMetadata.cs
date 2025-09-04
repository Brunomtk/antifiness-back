namespace Core.Models.Message
{
    public class MessageMetadata
    {
        public bool Edited { get; set; } = false;
        public string? EditReason { get; set; }
        public string? SystemAction { get; set; }
        public string? CustomData { get; set; }
    }
}
