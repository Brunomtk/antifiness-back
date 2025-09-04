// File: ControlApi/Controllers/PlansController.cs
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Saller.Infrastructure.ServiceExtension; // para PagedResult<T>
using System.Threading.Tasks;
using Core.DTO.Plan;

namespace ControlApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlansController : ControllerBase
    {
        private readonly IPlanService _planService;

        public PlansController(IPlanService planService)
        {
            _planService = planService;
        }

        /// <summary>Cadastra um novo plano.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(PlanDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PlanDTO>> Create([FromBody] CreatePlanRequest request)
        {
            var created = await _planService.CreatePlanAsync(request);
            if (created == null)
                return BadRequest("Não foi possível criar o plano.");

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>Obtém um plano pelo seu ID.</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(PlanDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PlanDTO>> GetById(int id)
        {
            var dto = await _planService.GetPlanByIdAsync(id);
            if (dto == null)
                return NotFound();

            return Ok(dto);
        }

        /// <summary>Retorna uma listagem paginada de planos.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<PlanDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<PlanDTO>>> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var paged = await _planService.GetPlansPagedAsync(page, pageSize);
            return Ok(paged);
        }

        /// <summary>Atualiza um plano existente.</summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePlanRequest request)
        {
            var updated = await _planService.UpdatePlanAsync(id, request);
            if (!updated)
                return BadRequest("Não foi possível atualizar o plano.");

            return NoContent();
        }

        /// <summary>Exclui um plano pelo seu ID.</summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _planService.DeletePlanAsync(id);
            if (!deleted)
                return BadRequest("Não foi possível excluir o plano.");

            return NoContent();
        }
    }
}
