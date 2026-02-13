using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ControlApi.Helpers;
using Core.DTO.Feedback;
using Core.Enums.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace ControlApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "ADMIN,COMPANY,CLIENTE")]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        private int? GetScopedEmpresaId()
        {
            return string.Equals(User.GetRole(), "COMPANY", StringComparison.OrdinalIgnoreCase)
                ? User.GetEmpresaId()
                : null;
        }

        private int? GetScopedClientId()
        {
            return string.Equals(User.GetRole(), "CLIENTE", StringComparison.OrdinalIgnoreCase)
                ? User.GetClientId()
                : null;
        }

        private bool IsAdmin()
        {
            // Mantém compatibilidade com tokens antigos que tenham userType
            var userType = User?.Claims?.FirstOrDefault(c => c.Type == "userType")?.Value;
            return int.TryParse(userType, out var t) && t == (int)UserType.Admin;
        }

        [HttpPost]
        [Authorize(Roles = "COMPANY,CLIENTE")]
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
            // COMPANY só pode ver feedbacks da própria empresa
            var scopedEmpresaId = GetScopedEmpresaId();
            if (scopedEmpresaId.HasValue)
                empresaId = scopedEmpresaId.Value;

            var feedback = await _feedbackService.GetFeedbackByIdAsync(id, empresaId);
            if (feedback == null)
                return NotFound(new { message = "Feedback não encontrado" });

            // CLIENTE só pode ver o próprio feedback (pelo ClientId)
            var scopedClientId = GetScopedClientId();
            if (scopedClientId.HasValue && feedback.ClientId != scopedClientId.Value)
                return Forbid();

            return Ok(feedback);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN,COMPANY,CLIENTE")]
        public async Task<ActionResult<FeedbacksPagedDTO>> GetPagedFeedbacks(
            [FromQuery] FeedbackFiltersDTO filters,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? empresaId = null)
        {
            var scopedEmpresaId = GetScopedEmpresaId();
            if (scopedEmpresaId.HasValue)
                empresaId = scopedEmpresaId.Value;

            var scopedClientId = GetScopedClientId();
            if (scopedClientId.HasValue)
                filters.ClientId = scopedClientId.Value;

            var result = await _feedbackService.GetPagedFeedbacksAsync(filters, pageNumber, pageSize, empresaId);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN,COMPANY")]
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
        [Authorize(Roles = "ADMIN,COMPANY")]
        public async Task<ActionResult> DeleteFeedback(int id)
        {
            var result = await _feedbackService.DeleteFeedbackAsync(id);
            if (!result)
                return NotFound(new { message = "Feedback não encontrado" });

            return NoContent();
        }

        [HttpGet("stats")]
        [Authorize(Roles = "ADMIN,COMPANY")]
        public async Task<ActionResult<FeedbackStatsDTO>> GetFeedbackStats()
        {
            var stats = await _feedbackService.GetFeedbackStatsAsync();
            return Ok(stats);
        }

        [HttpGet("client/{clientId}")]
        public async Task<ActionResult<List<FeedbackResponse>>> GetFeedbacksByClient(int clientId, [FromQuery] int? empresaId = null)
        {
            var scopedClientId = GetScopedClientId();
            if (scopedClientId.HasValue && scopedClientId.Value != clientId)
                return Forbid();

            var scopedEmpresaId = GetScopedEmpresaId();
            if (scopedEmpresaId.HasValue)
                empresaId = scopedEmpresaId.Value;

            var feedbacks = await _feedbackService.GetFeedbacksByClientAsync(clientId, empresaId);
            return Ok(feedbacks);
        }

        // ---------------- FEEDBACK OBRIGATÓRIO (modal) ----------------

        [HttpGet("mandatory/pending")]
        [Authorize(Roles = "CLIENTE")]
        public async Task<ActionResult<MandatoryFeedbackPendingResponse>> GetMandatoryPending([FromQuery] int clientId)
        {
            var scopedClientId = GetScopedClientId();
            if (scopedClientId.HasValue && scopedClientId.Value != clientId)
                return Forbid();

            var result = await _feedbackService.GetMandatoryPendingAsync(clientId);
            return Ok(result);
        }

        [HttpGet("mandatory/config")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<MandatoryFeedbackConfigDTO>> GetMandatoryConfig()
        {
            // segurança extra: só ADMIN de verdade
            if (!IsAdmin()) return Forbid();
            var config = await _feedbackService.GetMandatoryConfigAsync();
            return Ok(config);
        }

        [HttpPost("mandatory/config")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<MandatoryFeedbackConfigDTO>> SetMandatoryConfig([FromBody] SetMandatoryFeedbackConfigRequest request)
        {
            if (!IsAdmin()) return Forbid();
            var config = await _feedbackService.SetMandatoryConfigAsync(request);
            return Ok(config);
        }

        [HttpPost("mandatory/submit")]
        [Authorize(Roles = "CLIENTE")]
        public async Task<ActionResult<FeedbackResponse>> SubmitMandatory([FromBody] SubmitMandatoryFeedbackRequest request)
        {
            try
            {
                var scopedClientId = GetScopedClientId();
                if (scopedClientId.HasValue && request.ClientId != scopedClientId.Value)
                    return Forbid();

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
        [Authorize(Roles = "ADMIN,COMPANY")]
        public async Task<ActionResult<List<FeedbackResponse>>> GetFeedbacksByTrainer(int trainerId)
        {
            var feedbacks = await _feedbackService.GetFeedbacksByTrainerAsync(trainerId);
            return Ok(feedbacks);
        }
    }
}
