using Core.Enums;
using Core.Models;

namespace Core.Models.Feedback
{
    public class Feedback : BaseModel
    {
        public int ClientId { get; set; }
        public int? TrainerId { get; set; }
        public FeedbackType Type { get; set; }
        public FeedbackCategory Category { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Rating { get; set; } // 1-5 stars
        public FeedbackStatus Status { get; set; }
        public string? AdminResponse { get; set; }
        public DateTime? ResponseDate { get; set; }
        public string? AttachmentUrl { get; set; }
        public bool IsAnonymous { get; set; }
        
        // Navigation properties
        public virtual Client.Client Client { get; set; } = null!;
        public virtual User? Trainer { get; set; }
    }
}
