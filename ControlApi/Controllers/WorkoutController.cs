using System;
using System.Threading.Tasks;
using ControlApi.Helpers;
using Core.DTO.Workout;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace ControlApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "ADMIN,COMPANY,CLIENTE")]
    public class WorkoutController : ControllerBase
    {
        private readonly IWorkoutService _workoutService;

        public WorkoutController(IWorkoutService workoutService)
        {
            _workoutService = workoutService;
        }

        private int? GetScopedEmpresaId()
        {
            return string.Equals(User.GetRole(), "COMPANY", StringComparison.OrdinalIgnoreCase)
                ? User.GetEmpresaId()
                : null;
        }

        private int? GetScopedClientId()
        {
            return string.Equals(User.GetRole(), "CLIENTE", StringComparison.OrdinalIgnoreCase)
                ? User.GetClientId()
                : null;
        }

        private async Task<bool> CanAccessWorkoutAsync(int workoutId)
        {
            var scopedEmpresaId = GetScopedEmpresaId();
            var workout = await _workoutService.GetWorkoutByIdAsync(workoutId, scopedEmpresaId);
            if (workout == null) return false;

            var scopedClientId = GetScopedClientId();
            if (scopedClientId.HasValue && workout.ClientId != scopedClientId.Value)
                return false;

            return true;
        }

        /// <summary>
        /// Lista todos os treinos com filtros opcionais
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "ADMIN,COMPANY")]
        public async Task<ActionResult<WorkoutsPagedDTO>> GetWorkouts([FromQuery] WorkoutFiltersDTO filters)
        {
            try
            {
                var scopedEmpresaId = GetScopedEmpresaId();
                if (scopedEmpresaId.HasValue)
                    filters.EmpresaId = scopedEmpresaId.Value;

                var scopedClientId = GetScopedClientId();
                if (scopedClientId.HasValue)
                    filters.ClientId = scopedClientId.Value;

                var result = await _workoutService.GetWorkoutsAsync(filters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkoutResponse>> GetWorkout(int id, [FromQuery] int? empresaId = null)
        {
            try
            {
                var workout = await _workoutService.GetWorkoutByIdAsync(id, empresaId);
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
        [Authorize(Roles = "ADMIN,COMPANY")]
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
            
            if (!await CanAccessWorkoutAsync(workoutId)) return Forbid();

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
            
            if (!await CanAccessWorkoutAsync(workoutId)) return Forbid();

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
        [Authorize(Roles = "ADMIN,COMPANY")]
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

        #region Exercise substitutions

        /// <summary>
        /// Lista as substituições de um exercício dentro de um treino.
        /// </summary>
        [HttpGet("{workoutId}/exercises/{workoutExerciseId}/substitutions")]
        public async Task<ActionResult<List<WorkoutExerciseSubstitutionResponse>>> GetWorkoutExerciseSubstitutions(
            int workoutId, int workoutExerciseId)
        {
            try
            {
                var items = await _workoutService.GetWorkoutExerciseSubstitutionsAsync(workoutId, workoutExerciseId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Cria uma nova substituição para um exercício do treino.
        /// </summary>
        [HttpPost("{workoutId}/exercises/{workoutExerciseId}/substitutions")]
        public async Task<ActionResult<WorkoutExerciseSubstitutionResponse>> CreateWorkoutExerciseSubstitution(
            int workoutId,
            int workoutExerciseId,
            [FromBody] CreateWorkoutExerciseSubstitutionRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var created = await _workoutService.CreateWorkoutExerciseSubstitutionAsync(workoutId, workoutExerciseId, request);
                return CreatedAtAction(nameof(GetWorkoutExerciseSubstitutions), new { workoutId, workoutExerciseId }, created);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza uma substituição de exercício do treino.
        /// </summary>
        [HttpPut("{workoutId}/exercises/{workoutExerciseId}/substitutions/{substitutionId}")]
        public async Task<ActionResult<WorkoutExerciseSubstitutionResponse>> UpdateWorkoutExerciseSubstitution(
            int workoutId,
            int workoutExerciseId,
            int substitutionId,
            [FromBody] UpdateWorkoutExerciseSubstitutionRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updated = await _workoutService.UpdateWorkoutExerciseSubstitutionAsync(workoutId, workoutExerciseId, substitutionId, request);
                if (updated == null)
                    return NotFound(new { message = "Substituição não encontrada para este exercício/treino" });

                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Remove uma substituição de exercício do treino.
        /// </summary>
        [HttpDelete("{workoutId}/exercises/{workoutExerciseId}/substitutions/{substitutionId}")]
        public async Task<ActionResult> DeleteWorkoutExerciseSubstitution(
            int workoutId,
            int workoutExerciseId,
            int substitutionId)
        {
            try
            {
                var removed = await _workoutService.DeleteWorkoutExerciseSubstitutionAsync(workoutId, workoutExerciseId, substitutionId);
                if (!removed)
                    return NotFound(new { message = "Substituição não encontrada para este exercício/treino" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        #endregion

    }
}