using System.Threading.Tasks;
using ControlApi.Helpers;
using Core.DTO.Billing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace ControlApi.Controllers
{
    [ApiController]
    [Route("api/companies/subscription")]
    public class CompanySubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public CompanySubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN,COMPANY")]
        public async Task<ActionResult<CompanySubscriptionDTO?>> GetMyCompanySubscription([FromQuery] int? empresaId = null)
        {
            var resolvedEmpresaId = ResolveEmpresaId(empresaId);
            if (resolvedEmpresaId == null)
            {
                return BadRequest("empresaId é obrigatório.");
            }

            var sub = await _subscriptionService.GetCompanySubscriptionAsync(resolvedEmpresaId.Value);
            return Ok(sub);
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN,COMPANY")]
        public async Task<ActionResult<CompanySubscriptionDTO>> UpsertMyCompanySubscription(
            [FromBody] UpsertCompanySubscriptionRequest request,
            [FromQuery] int? empresaId = null)
        {
            var resolvedEmpresaId = ResolveEmpresaId(empresaId);
            if (resolvedEmpresaId == null)
            {
                return BadRequest("empresaId é obrigatório.");
            }

            var sub = await _subscriptionService.UpsertCompanySubscriptionAsync(resolvedEmpresaId.Value, request);
            return Ok(sub);
        }

        [HttpPatch("cancel-at-period-end")]
        [Authorize(Roles = "ADMIN,COMPANY")]
        public async Task<IActionResult> CancelAtPeriodEnd([FromQuery] bool cancelAtPeriodEnd, [FromQuery] int? empresaId = null)
        {
            var resolvedEmpresaId = ResolveEmpresaId(empresaId);
            if (resolvedEmpresaId == null)
            {
                return BadRequest("empresaId é obrigatório.");
            }

            var ok = await _subscriptionService.CancelAtPeriodEndAsync(resolvedEmpresaId.Value, cancelAtPeriodEnd);
            if (!ok) return NotFound();
            return NoContent();
        }

        private int? ResolveEmpresaId(int? requestedEmpresaId)
        {
            var role = User.GetRole();
            if (role == "COMPANY")
            {
                return User.GetEmpresaId();
            }

            // ADMIN pode escolher via query, ou cair no claim se existir
            return requestedEmpresaId ?? User.GetEmpresaId();
        }
    }
}
