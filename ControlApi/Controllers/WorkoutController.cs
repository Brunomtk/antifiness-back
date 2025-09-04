using Microsoft.AspNetCore.Mvc;
using Services;
using Core.DTO.Workout;

namespace ControlApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkoutController : ControllerBase
    {
        private readonly IWorkoutService _workoutService;

        public WorkoutController(IWorkoutService workoutService)
        {
            _workoutService = workoutService;
        }

        /// <summary>
        /// Lista todos os treinos com filtros opcionais
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<WorkoutsPagedDTO>> GetWorkouts([FromQuery] WorkoutFiltersDTO filters)
        {
            try
            {
                var result = await _workoutService.GetWorkoutsAsync(filters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Busca um treino por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkoutResponse>> GetWorkout(int id)
        {
            try
            {
                var workout = await _workoutService.GetWorkoutByIdAsync(id);
                if (workout == null)
                    return NotFound(new { message = "Treino não encontrado" });

                return Ok(workout);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Cria um novo treino
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<WorkoutResponse>> CreateWorkout([FromBody] CreateWorkoutRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var workout = await _workoutService.CreateWorkoutAsync(request);
                return CreatedAtAction(nameof(GetWorkout), new { id = workout.Id }, workout);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza um treino existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<WorkoutResponse>> UpdateWorkout(int id, [FromBody] UpdateWorkoutRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var workout = await _workoutService.UpdateWorkoutAsync(id, request);
                if (workout == null)
                    return NotFound(new { message = "Treino não encontrado" });

                return Ok(workout);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Remove um treino
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteWorkout(int id)
        {
            try
            {
                var success = await _workoutService.DeleteWorkoutAsync(id);
                if (!success)
                    return NotFound(new { message = "Treino não encontrado" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Altera o status de um treino
        /// </summary>
        [HttpPost("{id}/status")]
        public async Task<ActionResult> ChangeWorkoutStatus(int id, [FromBody] ChangeWorkoutStatusRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var success = await _workoutService.ChangeWorkoutStatusAsync(id, request);
                if (!success)
                    return NotFound(new { message = "Treino não encontrado" });

                return Ok(new { message = "Status alterado com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Lista templates de treinos
        /// </summary>
        [HttpGet("templates")]
        public async Task<ActionResult<WorkoutsPagedDTO>> GetTemplates([FromQuery] WorkoutFiltersDTO filters)
        {
            try
            {
                var result = await _workoutService.GetTemplatesAsync(filters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Cria um treino a partir de um template
        /// </summary>
        [HttpPost("templates/{templateId}/instantiate")]
        public async Task<ActionResult<WorkoutResponse>> InstantiateTemplate(int templateId, [FromBody] CreateWorkoutRequest? overrides = null)
        {
            try
            {
                var workout = await _workoutService.InstantiateTemplateAsync(templateId, overrides);
                if (workout == null)
                    return NotFound(new { message = "Template não encontrado" });

                return CreatedAtAction(nameof(GetWorkout), new { id = workout.Id }, workout);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Lista o progresso de um treino
        /// </summary>
        [HttpGet("{workoutId}/progress")]
        public async Task<ActionResult<List<WorkoutProgressResponse>>> GetWorkoutProgress(int workoutId)
        {
            try
            {
                var progress = await _workoutService.GetWorkoutProgressAsync(workoutId);
                return Ok(progress);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Registra progresso de um treino
        /// </summary>
        [HttpPost("{workoutId}/progress")]
        public async Task<ActionResult<WorkoutProgressResponse>> CreateWorkoutProgress(int workoutId, [FromBody] CreateWorkoutProgressRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var progress = await _workoutService.CreateWorkoutProgressAsync(workoutId, request);
                return CreatedAtAction(nameof(GetWorkoutProgress), new { workoutId = workoutId }, progress);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém estatísticas de treinos
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult<WorkoutStatsDTO>> GetWorkoutStats([FromQuery] int? empresaId = null, [FromQuery] int? clientId = null)
        {
            try
            {
                var stats = await _workoutService.GetWorkoutStatsAsync(empresaId, clientId);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }
    }
}
