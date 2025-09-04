using Core.Enums;

namespace Core.DTO.Feedback
{
    public class FeedbackFiltersDTO
    {
        public int? ClientId { get; set; }
        public int? TrainerId { get; set; }
        public FeedbackType? Type { get; set; }
        public FeedbackCategory? Category { get; set; }
        public FeedbackStatus? Status { get; set; }
        public int? MinRating { get; set; }
        public int? MaxRating { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsAnonymous { get; set; }
        public string? Search { get; set; }
    }
}
