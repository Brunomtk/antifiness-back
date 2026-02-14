using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Services;
using Core.DTO.Workout;
using ControlApi.Helpers;

namespace ControlApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExerciseController : ControllerBase
    {
        private readonly IWorkoutService _workoutService;

        public ExerciseController(IWorkoutService workoutService)
        {
            _workoutService = workoutService;
        }

        /// <summary>
        /// Lista todos os exercícios com filtros opcionais
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ExercisesPagedDTO>> GetExercises([FromQuery] ExerciseFiltersDTO filters)
        {
            try
            {
                var role = User.GetRole();
                if (role == "COMPANY")
                {
                    var empresaId = User.GetEmpresaId();
                    if (empresaId == null) return BadRequest(new { message = "EmpresaId não encontrado no token." });
                    filters.EmpresaId = empresaId;
                }

                var result = await _workoutService.GetExercisesAsync(filters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Busca um exercício por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ExerciseResponse>> GetExercise(int id, [FromQuery] int? empresaId = null)
        {
            try
            {
                var role = User.GetRole();
                if (role == "COMPANY")
                    empresaId = User.GetEmpresaId();

                var exercise = await _workoutService.GetExerciseByIdAsync(id, empresaId);
                if (exercise == null)
                    return NotFound(new { message = "Exercício não encontrado" });

                return Ok(exercise);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Cria um novo exercício
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ExerciseResponse>> CreateExercise([FromBody] CreateExerciseRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var role = User.GetRole();
                if (role == "COMPANY")
                {
                    var empresaId = User.GetEmpresaId();
                    if (empresaId == null) return BadRequest(new { message = "EmpresaId não encontrado no token." });
                    request.EmpresaId = empresaId.Value;
                }

                var exercise = await _workoutService.CreateExerciseAsync(request);
                return CreatedAtAction(nameof(GetExercise), new { id = exercise.Id }, exercise);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza um exercício existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ExerciseResponse>> UpdateExercise(int id, [FromBody] UpdateExerciseRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var role = User.GetRole();
                if (role == "COMPANY")
                {
                    var empresaId = User.GetEmpresaId();
                    if (empresaId == null) return BadRequest(new { message = "EmpresaId não encontrado no token." });
                    request.EmpresaId = empresaId.Value;
                }

                var exercise = await _workoutService.UpdateExerciseAsync(id, request);
                if (exercise == null)
                    return NotFound(new { message = "Exercício não encontrado" });

                return Ok(exercise);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Remove um exercício (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteExercise(int id)
        {
            try
            {
                var role = User.GetRole();
                if (role == "COMPANY")
                {
                    var empresaId = User.GetEmpresaId();
                    if (empresaId == null) return BadRequest(new { message = "EmpresaId não encontrado no token." });

                    // garante que não apague de outra empresa
                    var exists = await _workoutService.GetExerciseByIdAsync(id, empresaId);
                    if (exists == null) return NotFound(new { message = "Exercício não encontrado" });
                }

                var success = await _workoutService.DeleteExerciseAsync(id);
                if (!success)
                    return NotFound(new { message = "Exercício não encontrado" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }
    }
}
