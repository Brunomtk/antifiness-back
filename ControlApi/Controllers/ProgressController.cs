using Microsoft.AspNetCore.Mvc;
using Core.DTO.Course;
using Services;

namespace ControlApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProgressController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public ProgressController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProgressResponse>>> GetProgress(
            [FromQuery] int userId,
            [FromQuery] int courseId)
        {
            var progress = await _courseService.GetProgressAsync(userId, courseId);
            return Ok(progress);
        }

        [HttpPut]
        public async Task<ActionResult<ProgressResponse>> UpdateProgress([FromBody] UpdateProgressRequest request)
        {
            var progress = await _courseService.UpdateProgressAsync(request);
            if (progress == null)
                return BadRequest();

            return Ok(progress);
        }
    }
}
