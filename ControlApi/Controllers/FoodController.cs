using Core.DTO.Diet;
using Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ControlApi.Helpers;
using Services;

namespace ControlApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FoodController : ControllerBase
    {
        private readonly IDietService _dietService;

        public FoodController(IDietService dietService)
        {
            _dietService = dietService;
        }

        /// <summary>
        /// Lista todos os alimentos
        /// </summary>
        /// <param name="search">Busca por nome ou descrição</param>
        /// <param name="category">Filtro por categoria</param>
        /// <returns>Lista de alimentos</returns>
        [HttpGet]
        public async Task<ActionResult<List<FoodResponse>>> GetAllFoods(
            [FromQuery] string? search = null,
            [FromQuery] FoodCategory? category = null)
        {
            try
            {
                var role = User.GetRole();
                if (role != "COMPANY" && role != "ADMIN")
                    return Forbid();

                var empresaId = User.GetEmpresaId();
                var foods = await _dietService.GetAllFoodsAsync(empresaId, search, category);
                return Ok(foods);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Busca um alimento específico por ID
        /// </summary>
        /// <param name="id">ID do alimento</param>
        /// <returns>Dados do alimento</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<FoodResponse>> GetFoodById(int id)
        {
            try
            {
                var role = User.GetRole();
                if (role != "COMPANY" && role != "ADMIN")
                    return Forbid();

                var empresaId = User.GetEmpresaId();
                var food = await _dietService.GetFoodByIdAsync(id, empresaId);
                if (food == null)
                    return NotFound(new { message = "Alimento não encontrado" });

                return Ok(food);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Cria um novo alimento
        /// </summary>
        /// <param name="request">Dados do novo alimento</param>
        /// <returns>Alimento criado</returns>
        [HttpPost]
        public async Task<ActionResult<FoodResponse>> CreateFood([FromBody] CreateFoodRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var role = User.GetRole();
                if (role != "COMPANY" && role != "ADMIN")
                    return Forbid();

                var empresaId = User.GetEmpresaId();
                if (empresaId == null)
                    return BadRequest(new { message = "EmpresaId não encontrado no token." });

                var food = await _dietService.CreateFoodAsync(empresaId.Value, request);
                return CreatedAtAction(nameof(GetFoodById), new { id = food.Id }, food);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza um alimento existente
        /// </summary>
        /// <param name="id">ID do alimento</param>
        /// <param name="request">Dados para atualização</param>
        /// <returns>Alimento atualizado</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<FoodResponse>> UpdateFood(int id, [FromBody] UpdateFoodRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var role = User.GetRole();
                if (role != "COMPANY" && role != "ADMIN")
                    return Forbid();

                var empresaId = User.GetEmpresaId();
                if (empresaId == null)
                    return BadRequest(new { message = "EmpresaId não encontrado no token." });

                var food = await _dietService.UpdateFoodAsync(empresaId.Value, id, request);
                if (food == null)
                    return NotFound(new { message = "Alimento não encontrado" });

                return Ok(food);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Remove um alimento (soft delete)
        /// </summary>
        /// <param name="id">ID do alimento</param>
        /// <returns>Confirmação da remoção</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFood(int id)
        {
            try
            {
                var role = User.GetRole();
                if (role != "COMPANY" && role != "ADMIN")
                    return Forbid();

                var empresaId = User.GetEmpresaId();
                if (empresaId == null)
                    return BadRequest(new { message = "EmpresaId não encontrado no token." });

                var success = await _dietService.DeleteFoodAsync(empresaId.Value, id);
                if (!success)
                    return NotFound(new { message = "Alimento não encontrado" });

                return Ok(new { message = "Alimento removido com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }
    }
}
