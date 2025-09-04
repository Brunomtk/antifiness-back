using System;
using System.Threading.Tasks;
using Core.DTO.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace ControlApi.Controllers
{
    [ApiController]
    [Route("api/notification-settings")]
    [Authorize]
    public class NotificationSettingsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationSettingsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<NotificationSettingsResponse>> GetSettings(int userId)
        {
            var settings = await _notificationService.GetNotificationSettingsAsync(userId);
            return Ok(settings);
        }

        [HttpPatch("{userId}")]
        public async Task<ActionResult<NotificationSettingsResponse>> UpdateSettings(int userId, [FromBody] UpdateNotificationSettingsRequest request)
        {
            var settings = await _notificationService.UpdateNotificationSettingsAsync(userId, request);
            return Ok(settings);
        }
    }
}
