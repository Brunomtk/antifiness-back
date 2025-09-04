using Core.DTO.Feedback;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace ControlApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpPost]
        public async Task<ActionResult<FeedbackResponse>> CreateFeedback([FromBody] CreateFeedbackRequest request)
        {
            try
            {
                var feedback = await _feedbackService.CreateFeedbackAsync(request);
                return CreatedAtAction(nameof(GetFeedbackById), new { id = feedback.Id }, feedback);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FeedbackResponse>> GetFeedbackById(int id)
        {
            var feedback = await _feedbackService.GetFeedbackByIdAsync(id);
            if (feedback == null)
                return NotFound(new { message = "Feedback não encontrado" });

            return Ok(feedback);
        }

        [HttpGet]
        public async Task<ActionResult<FeedbacksPagedDTO>> GetPagedFeedbacks(
            [FromQuery] FeedbackFiltersDTO filters,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _feedbackService.GetPagedFeedbacksAsync(filters, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<FeedbackResponse>> UpdateFeedback(int id, [FromBody] UpdateFeedbackRequest request)
        {
            try
            {
                var feedback = await _feedbackService.UpdateFeedbackAsync(id, request);
                return Ok(feedback);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFeedback(int id)
        {
            var result = await _feedbackService.DeleteFeedbackAsync(id);
            if (!result)
                return NotFound(new { message = "Feedback não encontrado" });

            return NoContent();
        }

        [HttpGet("stats")]
        public async Task<ActionResult<FeedbackStatsDTO>> GetFeedbackStats()
        {
            var stats = await _feedbackService.GetFeedbackStatsAsync();
            return Ok(stats);
        }

        [HttpGet("client/{clientId}")]
        public async Task<ActionResult<List<FeedbackResponse>>> GetFeedbacksByClient(int clientId)
        {
            var feedbacks = await _feedbackService.GetFeedbacksByClientAsync(clientId);
            return Ok(feedbacks);
        }

        [HttpGet("trainer/{trainerId}")]
        public async Task<ActionResult<List<FeedbackResponse>>> GetFeedbacksByTrainer(int trainerId)
        {
            var feedbacks = await _feedbackService.GetFeedbacksByTrainerAsync(trainerId);
            return Ok(feedbacks);
        }
    }
}
