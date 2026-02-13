using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DTO.Billing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace ControlApi.Controllers
{
    [ApiController]
    [Route("api/subscription-plans")]
    public class SubscriptionPlansController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionPlansController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IList<SubscriptionPlanDTO>>> GetPlans([FromQuery] bool includeInactive = false)
        {
            var plans = await _subscriptionService.GetPlansAsync(includeInactive);
            return Ok(plans);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<SubscriptionPlanDTO>> CreatePlan([FromBody] CreateSubscriptionPlanRequest request)
        {
            var created = await _subscriptionService.CreatePlanAsync(request);
            return Created($"api/subscription-plans/{created.Id}", created);
        }
    }
}
