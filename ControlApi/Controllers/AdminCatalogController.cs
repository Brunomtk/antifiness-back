using ControlApi.Helpers;
using Core.DTO.Diet;
using Core.DTO.Workout;
using Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace ControlApi.Controllers
{
    [ApiController]
    [Route("api/admin/catalog")]
    [Authorize]
    public class AdminCatalogController : ControllerBase
    {
        private readonly IDietService _dietService;
        private readonly IWorkoutService _workoutService;

        public AdminCatalogController(IDietService dietService, IWorkoutService workoutService)
        {
            _dietService = dietService;
            _workoutService = workoutService;
        }

        private bool IsAdmin() => string.Equals(User.GetRole(), "ADMIN", StringComparison.OrdinalIgnoreCase);

        // ===== Foods =====
        [HttpGet("foods")]
        public async Task<ActionResult<List<FoodResponse>>> GetAllFoodsAdmin(
            [FromQuery] int? empresaId = null,
            [FromQuery] string? search = null,
            [FromQuery] FoodCategory? category = null)
        {
            if (!IsAdmin()) return Forbid();
            var foods = await _dietService.GetAllFoodsAdminAsync(empresaId, search, category);
            return Ok(foods);
        }

        public class CopyFoodsRequest
        {
            public int? SourceEmpresaId { get; set; }
            public int TargetEmpresaId { get; set; }
            public bool Overwrite { get; set; } = false;
            public bool IncludeInactive { get; set; } = false;
        }

        [HttpPost("foods/copy")]
        public async Task<ActionResult> CopyFoodsToCompany([FromBody] CopyFoodsRequest request)
        {
            if (!IsAdmin()) return Forbid();
            if (request.TargetEmpresaId <= 0)
                return BadRequest(new { message = "TargetEmpresaId inválido." });

            var count = await _dietService.CopyFoodsToEmpresaAsync(request.SourceEmpresaId, request.TargetEmpresaId, request.Overwrite, request.IncludeInactive);
            return Ok(new { copied = count });
        }

        // ===== Exercises =====
        [HttpGet("exercises")]
        public async Task<ActionResult<List<ExerciseResponse>>> GetAllExercisesAdmin(
            [FromQuery] int? empresaId = null,
            [FromQuery] bool includeInactive = false)
        {
            if (!IsAdmin()) return Forbid();
            var exercises = await _workoutService.GetAllExercisesAdminAsync(empresaId, includeInactive);
            return Ok(exercises);
        }

        public class CopyExercisesRequest
        {
            public int SourceEmpresaId { get; set; }
            public int TargetEmpresaId { get; set; }
            public bool Overwrite { get; set; } = false;
            public bool IncludeInactive { get; set; } = false;
        }

        [HttpPost("exercises/copy")]
        public async Task<ActionResult> CopyExercisesToCompany([FromBody] CopyExercisesRequest request)
        {
            if (!IsAdmin()) return Forbid();
            if (request.SourceEmpresaId <= 0 || request.TargetEmpresaId <= 0)
                return BadRequest(new { message = "SourceEmpresaId/TargetEmpresaId inválidos." });

            var count = await _workoutService.CopyExercisesToEmpresaAsync(request.SourceEmpresaId, request.TargetEmpresaId, request.Overwrite, request.IncludeInactive);
            return Ok(new { copied = count });
        }
    }
}
