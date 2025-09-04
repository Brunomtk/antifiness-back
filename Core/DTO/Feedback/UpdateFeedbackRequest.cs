using Core.Enums;

namespace Core.DTO.Feedback
{
    public class UpdateFeedbackRequest
    {
        public FeedbackType? Type { get; set; }
        public FeedbackCategory? Category { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? Rating { get; set; }
        public FeedbackStatus? Status { get; set; }
        public string? AdminResponse { get; set; }
        public string? AttachmentUrl { get; set; }
        public bool? IsAnonymous { get; set; }
    }
}
