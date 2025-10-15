using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DTO.Nutrition;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace ControlApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FoodMicronutrientsController : ControllerBase
    {
        private readonly INutritionService _nutrition;

        public FoodMicronutrientsController(INutritionService nutrition)
        {
            _nutrition = nutrition;
        }

        [HttpGet("types")]
        public async Task<ActionResult<List<MicronutrientDTO>>> GetTypes()
            => Ok(await _nutrition.GetMicronutrientTypesAsync());

        [HttpGet("{foodId:int}")]
        public async Task<ActionResult<List<FoodMicronutrientDTO>>> GetForFood(int foodId)
            => Ok(await _nutrition.GetFoodMicronutrientsAsync(foodId));

        [HttpPut("{foodId:int}")]
        public async Task<IActionResult> SetForFood(int foodId, [FromBody] List<FoodMicronutrientDTO> body)
        {
            await _nutrition.SetFoodMicronutrientsAsync(foodId, body);
            return NoContent();
        }
    }
}
