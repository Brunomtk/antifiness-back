using System.Linq;
using Core.DTO.Feedback;
using Core.Enums.User;
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
        public async Task<ActionResult<FeedbackResponse>> GetFeedbackById(int id, [FromQuery] int? empresaId = null)
        {
            var feedback = await _feedbackService.GetFeedbackByIdAsync(id, empresaId);
            if (feedback == null)
                return NotFound(new { message = "Feedback não encontrado" });

            return Ok(feedback);
        }

        [HttpGet]
        public async Task<ActionResult<FeedbacksPagedDTO>> GetPagedFeedbacks(
            [FromQuery] FeedbackFiltersDTO filters,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? empresaId = null)
        {
            var result = await _feedbackService.GetPagedFeedbacksAsync(filters, pageNumber, pageSize, empresaId);
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
        public async Task<ActionResult<List<FeedbackResponse>>> GetFeedbacksByClient(int clientId, [FromQuery] int? empresaId = null)
        {
            var feedbacks = await _feedbackService.GetFeedbacksByClientAsync(clientId, empresaId);
            return Ok(feedbacks);
        }


        // ---------------- FEEDBACK OBRIGATÓRIO (modal) ----------------

        private bool IsAdmin()
        {
            var userType = User?.Claims?.FirstOrDefault(c => c.Type == "userType")?.Value;
            return int.TryParse(userType, out var t) && t == (int)UserType.Admin;
        }

        /// <summary>
        /// Retorna se o cliente tem feedback obrigatório pendente (ex: a cada 30 dias) + perguntas para o modal.
        /// </summary>
        [HttpGet("mandatory/pending")]
        public async Task<ActionResult<MandatoryFeedbackPendingResponse>> GetMandatoryPending([FromQuery] int clientId)
        {
            var result = await _feedbackService.GetMandatoryPendingAsync(clientId);
            return Ok(result);
        }

        /// <summary>
        /// (Admin) Consulta configuração global do feedback obrigatório.
        /// </summary>
        [HttpGet("mandatory/config")]
        public async Task<ActionResult<MandatoryFeedbackConfigDTO>> GetMandatoryConfig()
        {
            if (!IsAdmin()) return Forbid();
            var config = await _feedbackService.GetMandatoryConfigAsync();
            return Ok(config);
        }

        /// <summary>
        /// (Admin) Habilita/desabilita feedback obrigatório para TODOS os clientes.
        /// Se Enabled=true e ForceAllNow=true, força uma nova rodada para todo mundo.
        /// </summary>
        [HttpPost("mandatory/config")]
        public async Task<ActionResult<MandatoryFeedbackConfigDTO>> SetMandatoryConfig([FromBody] SetMandatoryFeedbackConfigRequest request)
        {
            if (!IsAdmin()) return Forbid();
            var config = await _feedbackService.SetMandatoryConfigAsync(request);
            return Ok(config);
        }

        /// <summary>
        /// Envia o feedback obrigatório e fecha a pendência.
        /// </summary>
        [HttpPost("mandatory/submit")]
        public async Task<ActionResult<FeedbackResponse>> SubmitMandatory([FromBody] SubmitMandatoryFeedbackRequest request)
        {
            try
            {
                var feedback = await _feedbackService.SubmitMandatoryFeedbackAsync(request);
                return Ok(feedback);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("trainer/{trainerId}")]
        public async Task<ActionResult<List<FeedbackResponse>>> GetFeedbacksByTrainer(int trainerId)
        {
            var feedbacks = await _feedbackService.GetFeedbacksByTrainerAsync(trainerId);
            return Ok(feedbacks);
        }
    }
}
