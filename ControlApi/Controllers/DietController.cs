using Core.DTO.Diet;
using Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Services;
using Infrastructure.Repositories;
using System.Linq;

namespace ControlApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DietController : ControllerBase
    {
        private readonly IDietService _dietService;

        public DietController(IDietService dietService)
        {
            _dietService = dietService;
        }

        /// <summary>
        /// Lista todas as dietas com filtros e paginação
        /// </summary>
        /// <param name="pageNumber">Número da página (padrão: 1)</param>
        /// <param name="pageSize">Tamanho da página (padrão: 10)</param>
        /// <param name="search">Busca por nome da dieta, cliente ou empresa</param>
        /// <param name="status">Filtro por status da dieta</param>
        /// <param name="clientId">Filtro por ID do cliente</param>
        /// <param name="empresaId">Filtro por ID da empresa</param>
        /// <param name="startDateFrom">Data de início a partir de</param>
        /// <param name="startDateTo">Data de início até</param>
        /// <param name="endDateFrom">Data de fim a partir de</param>
        /// <param name="endDateTo">Data de fim até</param>
        /// <param name="hasEndDate">Filtro por dietas com/sem data de fim</param>
        /// <param name="minCalories">Calorias mínimas</param>
        /// <param name="maxCalories">Calorias máximas</param>
        /// <returns>Lista paginada de dietas</returns>
        [HttpGet]
        public async Task<ActionResult<DietsPagedDTO>> GetAllDiets(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] DietStatus? status = null,
            [FromQuery] int? clientId = null,
            [FromQuery] int? empresaId = null,
            [FromQuery] DateTime? startDateFrom = null,
            [FromQuery] DateTime? startDateTo = null,
            [FromQuery] DateTime? endDateFrom = null,
            [FromQuery] DateTime? endDateTo = null,
            [FromQuery] bool? hasEndDate = null,
            [FromQuery] double? minCalories = null,
            [FromQuery] double? maxCalories = null)
        {
            try
            {
                var filters = new DietFiltersDTO
                {
                    Search = search,
                    Status = status,
                    ClientId = clientId,
                    EmpresaId = empresaId,
                    StartDateFrom = startDateFrom,
                    StartDateTo = startDateTo,
                    EndDateFrom = endDateFrom,
                    EndDateTo = endDateTo,
                    HasEndDate = hasEndDate,
                    MinCalories = minCalories,
                    MaxCalories = maxCalories
                };

                var result = await _dietService.GetAllDietsAsync(pageNumber, pageSize, filters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Busca uma dieta específica por ID
        /// </summary>
        /// <param name="id">ID da dieta</param>
        /// <returns>Dados completos da dieta</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<DietResponse>> GetDietById(int id, [FromQuery] int? empresaId = null)
        {
            try
            {
                var diet = await _dietService.GetDietByIdAsync(id, empresaId);
                if (diet == null)
                    return NotFound(new { message = "Dieta não encontrada" });

                return Ok(diet);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Cria uma nova dieta
        /// </summary>
        /// <param name="request">Dados da nova dieta</param>
        /// <returns>Dieta criada</returns>
        [HttpPost]
        public async Task<ActionResult<DietResponse>> CreateDiet([FromBody] CreateDietRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var diet = await _dietService.CreateDietAsync(request);
                return CreatedAtAction(nameof(GetDietById), new { id = diet.Id }, diet);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza uma dieta existente
        /// </summary>
        /// <param name="id">ID da dieta</param>
        /// <param name="request">Dados para atualização</param>
        /// <returns>Dieta atualizada</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<DietResponse>> UpdateDiet(int id, [FromBody] UpdateDietRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var diet = await _dietService.UpdateDietAsync(id, request);
                if (diet == null)
                    return NotFound(new { message = "Dieta não encontrada" });

                return Ok(diet);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Remove uma dieta
        /// </summary>
        /// <param name="id">ID da dieta</param>
        /// <returns>Confirmação da remoção</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDiet(int id)
        {
            try
            {
                var success = await _dietService.DeleteDietAsync(id);
                if (!success)
                    return NotFound(new { message = "Dieta não encontrada" });

                return Ok(new { message = "Dieta removida com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém estatísticas das dietas
        /// </summary>
        /// <returns>Estatísticas completas das dietas</returns>
        [HttpGet("stats")]
        public async Task<ActionResult<DietStatsDTO>> GetDietStats()
        {
            try
            {
                var stats = await _dietService.GetDietStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        
        // MEALS ENDPOINTS

        /// <summary>
        /// Lista todas as refeições de uma dieta (via rota com path param)
        /// </summary>
        /// <param name="dietId">ID da dieta</param>
        /// <returns>Lista de refeições</returns>
        [HttpGet("{dietId}/meals")]
        public async Task<ActionResult<List<DietMealResponse>>> GetDietMeals(int dietId)
        {
            try
            {
                var meals = await _dietService.GetDietMealsAsync(dietId);
                return Ok(meals);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Lista todas as refeições de uma dieta (via query string)
        /// </summary>
        /// <param name="dietId">ID da dieta</param>
        /// <returns>Lista de refeições</returns>
        [HttpGet("meals")]
        public async Task<ActionResult<List<DietMealResponse>>> GetMealsByFilter([FromQuery] int dietId)
        {
            try
            {
                var meals = await _dietService.GetDietMealsAsync(dietId);
                return Ok(meals);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }
[HttpPost("{dietId}/meals")]
        public async Task<ActionResult<DietMealResponse>> CreateDietMeal(int dietId, [FromBody] CreateDietMealRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var meal = await _dietService.CreateDietMealAsync(dietId, request);
                return CreatedAtAction(nameof(GetDietMeals), new { dietId }, meal);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza uma refeição
        /// </summary>
        /// <param name="mealId">ID da refeição</param>
        /// <param name="request">Dados para atualização</param>
        /// <returns>Refeição atualizada</returns>
        [HttpPut("meals/{mealId}")]
        public async Task<ActionResult<DietMealResponse>> UpdateDietMeal(int mealId, [FromBody] UpdateDietMealRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var meal = await _dietService.UpdateDietMealAsync(mealId, request);
                if (meal == null)
                    return NotFound(new { message = "Refeição não encontrada" });

                return Ok(meal);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        // Alias para compatibilizar com front que envia dietId na rota
        [HttpPut("{dietId}/meals/{mealId}")]
        public Task<ActionResult<DietMealResponse>> UpdateDietMealWithDiet(int dietId, int mealId, [FromBody] UpdateDietMealRequest request)
            => UpdateDietMeal(mealId, request);


        /// <summary>
        /// Remove uma refeição
        /// </summary>
        /// <param name="mealId">ID da refeição</param>
        /// <returns>Confirmação da remoção</returns>
        [HttpDelete("meals/{mealId}")]
        public async Task<ActionResult> DeleteDietMeal(int mealId)
        {
            try
            {
                var success = await _dietService.DeleteDietMealAsync(mealId);
                if (!success)
                    return NotFound(new { message = "Refeição não encontrada" });

                return Ok(new { message = "Refeição removida com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Marca uma refeição como concluída
        /// </summary>
        /// <param name="mealId">ID da refeição</param>
        /// <returns>Confirmação da conclusão</returns>
        [HttpPost("meals/{mealId}/complete")]
        public async Task<ActionResult> CompleteMeal(int mealId)
        {
            try
            {
                var success = await _dietService.CompleteMealAsync(mealId);
                if (!success)
                    return NotFound(new { message = "Refeição não encontrada" });

                return Ok(new { message = "Refeição marcada como concluída" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        // PROGRESS ENDPOINTS

        /// <summary>
        /// Lista o progresso de uma dieta
        /// </summary>
        /// <param name="dietId">ID da dieta</param>
        /// <returns>Lista de progresso</returns>
        [HttpGet("{dietId}/progress")]
        public async Task<ActionResult<List<DietProgressResponse>>> GetDietProgress(int dietId)
        {
            try
            {
                var progress = await _dietService.GetDietProgressAsync(dietId);
                return Ok(progress);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Lista o progresso de uma dieta (via query string)
        /// </summary>
        /// <param name="dietId">ID da dieta</param>
        /// <returns>Lista de progresso</returns>
        [HttpGet("progress")]
        public async Task<ActionResult<List<DietProgressResponse>>> GetDietProgressByFilter([FromQuery] int dietId)
        {
            try
            {
                var progress = await _dietService.GetDietProgressAsync(dietId);
                return Ok(progress);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }



        /// <summary>
        /// Adiciona uma entrada de progresso diário
        /// </summary>
        /// <param name="dietId">ID da dieta</param>
        /// <param name="request">Dados do progresso</param>
        /// <returns>Progresso criado</returns>
        [HttpPost("{dietId}/progress")]
        public async Task<ActionResult<DietProgressResponse>> CreateDietProgress(
            int dietId,
            [FromBody] CreateDietProgressRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var progress = await _dietService.CreateDietProgressAsync(dietId, request);
                return CreatedAtAction(nameof(GetDietProgressByFilter), new { dietId }, progress);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        // SUPPLEMENTS ENDPOINTS

        /// <summary>
        /// Lista as suplementações de uma dieta.
        /// </summary>
        [HttpGet("{dietId}/supplements")]
        public async Task<ActionResult<List<DietSupplementResponse>>> GetDietSupplements(int dietId)
        {
            try
            {
                var supplements = await _dietService.GetDietSupplementsAsync(dietId);
                return Ok(supplements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Cria uma nova suplementação para a dieta.
        /// </summary>
        [HttpPost("{dietId}/supplements")]
        public async Task<ActionResult<DietSupplementResponse>> CreateDietSupplement(
            int dietId,
            [FromBody] CreateDietSupplementRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var supplement = await _dietService.CreateDietSupplementAsync(dietId, request);
                return CreatedAtAction(nameof(GetDietSupplements), new { dietId }, supplement);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza uma suplementação da dieta.
        /// </summary>
        [HttpPut("{dietId}/supplements/{supplementId}")]
        public async Task<ActionResult<DietSupplementResponse>> UpdateDietSupplement(
            int dietId,
            int supplementId,
            [FromBody] UpdateDietSupplementRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _dietService.UpdateDietSupplementAsync(dietId, supplementId, request);
                if (result == null)
                    return NotFound(new { message = "Suplementação não encontrada para esta dieta" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Remove uma suplementação da dieta.
        /// </summary>
        [HttpDelete("{dietId}/supplements/{supplementId}")]
        public async Task<ActionResult> DeleteDietSupplement(int dietId, int supplementId)
        {
            try
            {
                var removed = await _dietService.DeleteDietSupplementAsync(dietId, supplementId);
                if (!removed)
                    return NotFound(new { message = "Suplementação não encontrada para esta dieta" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

    }
}