using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ControlApi.Helpers;
using Core.DTO.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace ControlApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "ADMIN,COMPANY,CLIENTE")]
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
            var role = (User.GetRole() ?? string.Empty).Trim();
            var tokenUserId = User.GetUserId();
            if (tokenUserId == null) return Unauthorized();

            if (!string.Equals(role, "ADMIN", StringComparison.OrdinalIgnoreCase))
            {
                filters.UserId = tokenUserId.Value;
            }

            var result = await _notificationService.GetPagedNotificationsAsync(filters);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NotificationResponse>> GetNotification(int id)
        {
            try
            {
                var notification = await _notificationService.GetNotificationByIdAsync(id);

                var role = (User.GetRole() ?? string.Empty).Trim();
                var tokenUserId = User.GetUserId();
                if (tokenUserId == null) return Unauthorized();

                if (!string.Equals(role, "ADMIN", StringComparison.OrdinalIgnoreCase) && notification.UserId != tokenUserId.Value)
                    return Forbid();

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

            var role = (User.GetRole() ?? string.Empty).Trim();
            var tokenUserId = User.GetUserId();
            if (tokenUserId == null) return Unauthorized();

            if (!string.Equals(role, "ADMIN", StringComparison.OrdinalIgnoreCase))
            {
                request.UserId = tokenUserId.Value;
            }

            var notification = await _notificationService.CreateNotificationAsync(request);
            return CreatedAtAction(nameof(GetNotification), new { id = notification.Id }, notification);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<NotificationResponse>> UpdateNotification(int id, [FromBody] UpdateNotificationRequest request)
        {
            try
            {
                var role = (User.GetRole() ?? string.Empty).Trim();
                var tokenUserId = User.GetUserId();
                if (tokenUserId == null) return Unauthorized();

                if (!string.Equals(role, "ADMIN", StringComparison.OrdinalIgnoreCase))
                {
                    var existing = await _notificationService.GetNotificationByIdAsync(id);
                    if (existing.UserId != tokenUserId.Value) return Forbid();
                }

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
            var role = (User.GetRole() ?? string.Empty).Trim();
            var tokenUserId = User.GetUserId();
            if (tokenUserId == null) return Unauthorized();

            if (!string.Equals(role, "ADMIN", StringComparison.OrdinalIgnoreCase))
            {
                var existing = await _notificationService.GetNotificationByIdAsync(id);
                if (existing.UserId != tokenUserId.Value) return Forbid();
            }

            var deleted = await _notificationService.DeleteNotificationAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpPost("mark-all-read")]
        public async Task<ActionResult<object>> MarkAllAsRead([FromBody] MarkAllReadRequest request)
        {
            var role = (User.GetRole() ?? string.Empty).Trim();
            var tokenUserId = User.GetUserId();
            if (tokenUserId == null) return Unauthorized();

            var userId = string.Equals(role, "ADMIN", StringComparison.OrdinalIgnoreCase)
                ? request.UserId
                : tokenUserId.Value;

            var updated = await _notificationService.MarkAllAsReadAsync(userId);
            return Ok(new { updated });
        }

        [HttpGet("stats")]
        public async Task<ActionResult<NotificationStatsDTO>> GetStats([FromQuery] int? userId, [FromQuery] DateTime? start, [FromQuery] DateTime? end)
        {
            var role = (User.GetRole() ?? string.Empty).Trim();
            var tokenUserId = User.GetUserId();
            if (tokenUserId == null) return Unauthorized();

            if (!string.Equals(role, "ADMIN", StringComparison.OrdinalIgnoreCase))
                userId = tokenUserId.Value;

            var stats = await _notificationService.GetNotificationStatsAsync(userId, start, end);
            return Ok(stats);
        }
    }

    public class MarkAllReadRequest
    {
        public int UserId { get; set; }
    }
}
