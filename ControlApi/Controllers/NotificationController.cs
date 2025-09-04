using System;
using System.Threading.Tasks;
using Core.DTO.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace ControlApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<ActionResult<NotificationsPagedDTO>> GetNotifications([FromQuery] NotificationFiltersDTO filters)
        {
            var result = await _notificationService.GetPagedNotificationsAsync(filters);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NotificationResponse>> GetNotification(int id)
        {
            try
            {
                var notification = await _notificationService.GetNotificationByIdAsync(id);
                return Ok(notification);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<NotificationResponse>> CreateNotification([FromBody] CreateNotificationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var notification = await _notificationService.CreateNotificationAsync(request);
            return CreatedAtAction(nameof(GetNotification), new { id = notification.Id }, notification);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<NotificationResponse>> UpdateNotification(int id, [FromBody] UpdateNotificationRequest request)
        {
            try
            {
                var notification = await _notificationService.UpdateNotificationAsync(id, request);
                return Ok(notification);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var deleted = await _notificationService.DeleteNotificationAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpPost("mark-all-read")]
        public async Task<ActionResult<object>> MarkAllAsRead([FromBody] MarkAllReadRequest request)
        {
            var updated = await _notificationService.MarkAllAsReadAsync(request.UserId);
            return Ok(new { updated });
        }

        [HttpGet("stats")]
        public async Task<ActionResult<NotificationStatsDTO>> GetStats([FromQuery] int? userId, [FromQuery] DateTime? start, [FromQuery] DateTime? end)
        {
            var stats = await _notificationService.GetNotificationStatsAsync(userId, start, end);
            return Ok(stats);
        }
    }

    public class MarkAllReadRequest
    {
        public int UserId { get; set; }
    }
}
