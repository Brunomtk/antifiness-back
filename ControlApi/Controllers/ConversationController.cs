using Microsoft.AspNetCore.Mvc;
using Core.DTO.Message;
using Services;

namespace ControlApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConversationController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public ConversationController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet]
        public ActionResult<ConversationsPagedDTO> GetConversations(
            [FromQuery] ConversationFiltersDTO filters,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20)
        {
            try
            {
                var result = _messageService.GetConversations(filters, page, limit);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public ActionResult<ConversationResponse> GetConversation(int id)
        {
            try
            {
                var result = _messageService.GetConversationById(id);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult<ConversationResponse> CreateConversation([FromBody] CreateConversationRequest request)
        {
            try
            {
                var result = _messageService.CreateConversation(request);
                return CreatedAtAction(nameof(GetConversation), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public ActionResult<ConversationResponse> UpdateConversation(int id, [FromBody] UpdateConversationRequest request)
        {
            try
            {
                var result = _messageService.UpdateConversation(id, request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteConversation(int id)
        {
            try
            {
                _messageService.DeleteConversation(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/participants")]
        public ActionResult AddParticipant(int id, [FromBody] AddParticipantRequest request)
        {
            try
            {
                _messageService.AddParticipant(id, request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}/participants/{userId}")]
        public ActionResult RemoveParticipant(int id, int userId)
        {
            try
            {
                _messageService.RemoveParticipant(id, userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/participants/{userId}")]
        public ActionResult UpdateParticipant(int id, int userId, [FromBody] UpdateParticipantRequest request)
        {
            try
            {
                _messageService.UpdateParticipant(id, userId, request);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/archive")]
        public ActionResult ArchiveConversation(int id)
        {
            try
            {
                _messageService.ArchiveConversation(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/unarchive")]
        public ActionResult UnarchiveConversation(int id)
        {
            try
            {
                _messageService.UnarchiveConversation(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/mute")]
        public ActionResult MuteConversation(int id)
        {
            try
            {
                _messageService.MuteConversation(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/unmute")]
        public ActionResult UnmuteConversation(int id)
        {
            try
            {
                _messageService.UnmuteConversation(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}/messages")]
        public ActionResult<MessagesPagedDTO> GetConversationMessages(
            int id,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20)
        {
            try
            {
                var result = _messageService.GetConversationMessages(id, page, limit);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/messages/read")]
        public ActionResult MarkMessagesAsRead(int id, [FromBody] MarkMessagesAsReadRequest request)
        {
            try
            {
                _messageService.MarkMessagesAsRead(id, request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}/stats")]
        public ActionResult<ConversationStatsDTO> GetConversationStats(int id)
        {
            try
            {
                var result = _messageService.GetConversationStats(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
