using Core.Enums;

namespace Core.DTO.Feedback
{
    public class CreateFeedbackRequest
    {
        public int ClientId { get; set; }
        public int? TrainerId { get; set; }
        public FeedbackType Type { get; set; }
        public FeedbackCategory Category { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? AttachmentUrl { get; set; }
        public bool IsAnonymous { get; set; }
    }
}
