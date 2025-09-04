using Microsoft.AspNetCore.Mvc;
using Core.DTO.Course;
using Services;

namespace ControlApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public EnrollmentController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<ActionResult<List<EnrollmentResponse>>> GetEnrollments(
            [FromQuery] int? empresasId,
            [FromQuery] int? courseId,
            [FromQuery] int? userId)
        {
            var enrollments = await _courseService.GetEnrollmentsAsync(empresasId, courseId, userId);
            return Ok(enrollments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EnrollmentResponse>> GetEnrollment(int id)
        {
            var enrollment = await _courseService.GetEnrollmentByIdAsync(id);
            if (enrollment == null)
                return NotFound();

            return Ok(enrollment);
        }

        [HttpPost]
        public async Task<ActionResult<EnrollmentResponse>> CreateEnrollment([FromBody] CreateEnrollmentRequest request)
        {
            var enrollment = await _courseService.CreateEnrollmentAsync(request);
            return CreatedAtAction(nameof(GetEnrollment), new { id = enrollment.Id }, enrollment);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EnrollmentResponse>> UpdateEnrollment(int id, [FromBody] UpdateEnrollmentRequest request)
        {
            var enrollment = await _courseService.UpdateEnrollmentAsync(id, request);
            if (enrollment == null)
                return NotFound();

            return Ok(enrollment);
        }
    }
}
