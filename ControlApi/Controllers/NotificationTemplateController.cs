using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DTO.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace ControlApi.Controllers
{
    [ApiController]
    [Route("api/notification-templates")]
    [Authorize]
    public class NotificationTemplateController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationTemplateController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<ActionResult<List<NotificationTemplateResponse>>> GetTemplates()
        {
            var templates = await _notificationService.GetNotificationTemplatesAsync();
            return Ok(templates);
        }

        [HttpPost]
        public async Task<ActionResult<NotificationTemplateResponse>> CreateTemplate([FromBody] CreateNotificationTemplateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var template = await _notificationService.CreateNotificationTemplateAsync(request);
            return CreatedAtAction(nameof(GetTemplates), new { id = template.Id }, template);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<NotificationTemplateResponse>> UpdateTemplate(int id, [FromBody] UpdateNotificationTemplateRequest request)
        {
            try
            {
                var template = await _notificationService.UpdateNotificationTemplateAsync(id, request);
                return Ok(template);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            var deleted = await _notificationService.DeleteNotificationTemplateAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
