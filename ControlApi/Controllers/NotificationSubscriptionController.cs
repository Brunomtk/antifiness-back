using System;
using System.Threading.Tasks;
using Core.DTO.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace ControlApi.Controllers
{
    [ApiController]
    [Route("api/notification-subscriptions")]
    [Authorize]
    public class NotificationSubscriptionController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationSubscriptionController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost]
        public async Task<ActionResult<NotificationSubscriptionResponse>> CreateSubscription([FromBody] CreateNotificationSubscriptionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var subscription = await _notificationService.CreateNotificationSubscriptionAsync(request);
            return CreatedAtAction(nameof(CreateSubscription), new { id = subscription.Id }, subscription);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubscription(int id)
        {
            var deleted = await _notificationService.DeleteNotificationSubscriptionAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpPost("push")]
        public async Task<ActionResult<object>> SendPushNotification([FromBody] SendPushNotificationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var queued = await _notificationService.SendPushNotificationAsync(request);
            return Accepted(new { queued });
        }
    }
}
