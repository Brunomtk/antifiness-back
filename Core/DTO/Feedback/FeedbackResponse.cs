using Core.Enums;

namespace Core.DTO.Feedback
{
    public class FeedbackResponse
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public int? TrainerId { get; set; }
        public string? TrainerName { get; set; }
        public FeedbackType Type { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public FeedbackCategory Category { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Rating { get; set; }
        public FeedbackStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string? AdminResponse { get; set; }
        public DateTime? ResponseDate { get; set; }
        public string? AttachmentUrl { get; set; }
        public bool IsAnonymous { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
