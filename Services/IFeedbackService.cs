using Core.DTO.Feedback;

namespace Services
{
    public interface IFeedbackService
    {
        Task<FeedbackResponse> CreateFeedbackAsync(CreateFeedbackRequest request);
        Task<FeedbackResponse> UpdateFeedbackAsync(int id, UpdateFeedbackRequest request);
        Task<FeedbackResponse?> GetFeedbackByIdAsync(int id, int? empresaId = null);
        Task<FeedbacksPagedDTO> GetPagedFeedbacksAsync(FeedbackFiltersDTO filters, int pageNumber, int pageSize, int? empresaId = null);
        Task<bool> DeleteFeedbackAsync(int id);
        Task<FeedbackStatsDTO> GetFeedbackStatsAsync();
        Task<List<FeedbackResponse>> GetFeedbacksByClientAsync(int clientId, int? empresaId = null);
        Task<List<FeedbackResponse>> GetFeedbacksByTrainerAsync(int trainerId);

        Task<MandatoryFeedbackPendingResponse> GetMandatoryPendingAsync(int clientId);
        Task<FeedbackResponse> SubmitMandatoryFeedbackAsync(SubmitMandatoryFeedbackRequest request);

        Task<MandatoryFeedbackConfigDTO> GetMandatoryConfigAsync();
        Task<MandatoryFeedbackConfigDTO> SetMandatoryConfigAsync(SetMandatoryFeedbackConfigRequest request);

    }
}
