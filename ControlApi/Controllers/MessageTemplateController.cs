using Microsoft.AspNetCore.Mvc;
using Core.DTO.Message;
using Core.Enums;
using Services;

namespace ControlApi.Controllers
{
    [ApiController]
    [Route("api/message-templates")]
    public class MessageTemplateController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageTemplateController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet]
        public ActionResult<List<MessageTemplateResponse>> GetTemplates(
            [FromQuery] TemplateCategory? category = null,
            [FromQuery] int? empresasId = null)
        {
            try
            {
                var result = _messageService.GetTemplates(category, empresasId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public ActionResult<MessageTemplateResponse> GetTemplate(int id)
        {
            try
            {
                var result = _messageService.GetTemplateById(id);
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
        public ActionResult<MessageTemplateResponse> CreateTemplate([FromBody] CreateMessageTemplateRequest request)
        {
            try
            {
                var result = _messageService.CreateTemplate(request);
                return CreatedAtAction(nameof(GetTemplate), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public ActionResult<MessageTemplateResponse> UpdateTemplate(int id, [FromBody] UpdateMessageTemplateRequest request)
        {
            try
            {
                var result = _messageService.UpdateTemplate(id, request);
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
        public ActionResult DeleteTemplate(int id)
        {
            try
            {
                _messageService.DeleteTemplate(id);
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

        [HttpPost("{id}/render")]
        public ActionResult<string> RenderTemplate(int id, [FromBody] RenderTemplateRequest request)
        {
            try
            {
                var result = _messageService.RenderTemplate(id, request);
                return Ok(new { content = result });
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
    }
}
