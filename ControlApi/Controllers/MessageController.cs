using Microsoft.AspNetCore.Mvc;
using Core.DTO.Message;
using Services;

namespace ControlApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet]
        public ActionResult<MessagesPagedDTO> GetMessages(
            [FromQuery] MessageFiltersDTO filters,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20)
        {
            try
            {
                var result = _messageService.GetMessages(filters, page, limit);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public ActionResult<MessageResponse> GetMessage(int id)
        {
            try
            {
                var result = _messageService.GetMessageById(id);
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
        public ActionResult<MessageResponse> CreateMessage([FromBody] CreateMessageRequest request)
        {
            try
            {
                var result = _messageService.CreateMessage(request);
                return CreatedAtAction(nameof(GetMessage), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public ActionResult<MessageResponse> UpdateMessage(int id, [FromBody] UpdateMessageRequest request)
        {
            try
            {
                var result = _messageService.UpdateMessage(id, request);
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
        public ActionResult DeleteMessage(int id)
        {
            try
            {
                _messageService.DeleteMessage(id);
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

        [HttpPost("{id}/delivered")]
        public ActionResult MarkAsDelivered(int id)
        {
            try
            {
                _messageService.MarkMessageAsDelivered(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/reactions")]
        public ActionResult AddReaction(int id, [FromBody] CreateMessageReactionRequest request)
        {
            try
            {
                _messageService.AddReaction(id, request);
                return CreatedAtAction(nameof(GetMessage), new { id }, null);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{messageId}/reactions/{reactionId}")]
        public ActionResult RemoveReaction(int messageId, int reactionId)
        {
            try
            {
                _messageService.RemoveReaction(messageId, reactionId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("search")]
        public ActionResult<MessagesPagedDTO> SearchMessages(
            [FromQuery] MessageFiltersDTO filters,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20)
        {
            try
            {
                var result = _messageService.SearchMessages(filters, page, limit);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("stats")]
        public ActionResult<MessageStatsDTO> GetMessageStats(
            [FromQuery] int? empresasId = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            try
            {
                var result = _messageService.GetMessageStats(empresasId, from, to);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
