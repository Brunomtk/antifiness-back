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
    public class DietMicrosController : ControllerBase
    {
        private readonly INutritionService _nutrition;

        public DietMicrosController(INutritionService nutrition)
        {
            _nutrition = nutrition;
        }

        [HttpPost("calc")]
        public async Task<ActionResult<CalcMicrosResultDTO>> Calc([FromBody] CalcMicrosInputDTO body)
            => Ok(await _nutrition.CalculateMicrosAsync(body));

        [HttpGet("{dietId:int}")]
        public async Task<ActionResult<CalcMicrosResultDTO>> CalcByDiet(int dietId)
            => Ok(await _nutrition.CalculateMicrosForDietAsync(dietId));
    }
}
