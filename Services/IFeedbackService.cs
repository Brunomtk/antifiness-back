using Core.DTO.Feedback;

namespace Services
{
    public interface IFeedbackService
    {
        Task<FeedbackResponse> CreateFeedbackAsync(CreateFeedbackRequest request);
        Task<FeedbackResponse> UpdateFeedbackAsync(int id, UpdateFeedbackRequest request);
        Task<FeedbackResponse?> GetFeedbackByIdAsync(int id);
        Task<FeedbacksPagedDTO> GetPagedFeedbacksAsync(FeedbackFiltersDTO filters, int pageNumber, int pageSize);
        Task<bool> DeleteFeedbackAsync(int id);
        Task<FeedbackStatsDTO> GetFeedbackStatsAsync();
        Task<List<FeedbackResponse>> GetFeedbacksByClientAsync(int clientId);
        Task<List<FeedbackResponse>> GetFeedbacksByTrainerAsync(int trainerId);
    }
}
