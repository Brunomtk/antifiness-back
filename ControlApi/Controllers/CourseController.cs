using System;
using System.Threading.Tasks;
using ControlApi.Helpers;
using Core.DTO.Course;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace ControlApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "ADMIN,COMPANY,CLIENTE")]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        private int? GetScopedEmpresaId()
        {
            return string.Equals(User.GetRole(), "COMPANY", StringComparison.OrdinalIgnoreCase)
                ? User.GetEmpresaId()
                : null;
        }

        [HttpGet]
        public async Task<ActionResult<CoursesPagedDTO>> GetCourses(
            [FromQuery] string? search,
            [FromQuery] Core.Enums.CourseCategory? category,
            [FromQuery] Core.Enums.CourseLevel? level,
            [FromQuery] Core.Enums.CourseStatus? status,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] int? empresasId,
            [FromQuery] string? instructor,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var scopedEmpresaId = GetScopedEmpresaId();
            if (scopedEmpresaId.HasValue)
                empresasId = scopedEmpresaId.Value;

            var filters = new CourseFiltersDTO
            {
                Search = search,
                Category = category,
                Level = level,
                Status = status,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                StartDate = startDate,
                EndDate = endDate,
                EmpresasId = empresasId,
                Instructor = instructor
            };

            var result = await _courseService.GetPagedCoursesAsync(filters, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseResponse>> GetCourse(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
                return NotFound();

            return Ok(course);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN,COMPANY")]
        public async Task<ActionResult<CourseResponse>> CreateCourse([FromBody] CreateCourseRequest request)
        {
            var course = await _courseService.CreateCourseAsync(request);
            return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, course);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN,COMPANY")]
        public async Task<ActionResult<CourseResponse>> UpdateCourse(int id, [FromBody] UpdateCourseRequest request)
        {
            var course = await _courseService.UpdateCourseAsync(id, request);
            if (course == null)
                return NotFound();

            return Ok(course);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN,COMPANY")]
        public async Task<ActionResult> DeleteCourse(int id)
        {
            var success = await _courseService.DeleteCourseAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpPost("{id}/publish")]
        [Authorize(Roles = "ADMIN,COMPANY")]
        public async Task<ActionResult> PublishCourse(int id)
        {
            var success = await _courseService.PublishCourseAsync(id);
            if (!success)
                return NotFound();

            return Ok(new { message = "Course published successfully" });
        }

        [HttpGet("{courseId}/lessons")]
        public async Task<ActionResult<List<LessonResponse>>> GetCourseLessons(int courseId)
        {
            var lessons = await _courseService.GetCourseLessonsAsync(courseId);
            return Ok(lessons);
        }

        [HttpGet("{courseId}/lessons/{lessonId}")]
        public async Task<ActionResult<LessonResponse>> GetLesson(int courseId, int lessonId)
        {
            var lesson = await _courseService.GetLessonByIdAsync(courseId, lessonId);
            if (lesson == null)
                return NotFound();

            return Ok(lesson);
        }

        [HttpPost("{courseId}/lessons")]
        [Authorize(Roles = "ADMIN,COMPANY")]
        public async Task<ActionResult<LessonResponse>> CreateLesson(int courseId, [FromBody] CreateLessonRequest request)
        {
            var lesson = await _courseService.CreateLessonAsync(courseId, request);
            return CreatedAtAction(nameof(GetLesson), new { courseId, lessonId = lesson.Id }, lesson);
        }

        [HttpPut("{courseId}/lessons/{lessonId}")]
        [Authorize(Roles = "ADMIN,COMPANY")]
        public async Task<ActionResult<LessonResponse>> UpdateLesson(int courseId, int lessonId, [FromBody] UpdateLessonRequest request)
        {
            var lesson = await _courseService.UpdateLessonAsync(courseId, lessonId, request);
            if (lesson == null)
                return NotFound();

            return Ok(lesson);
        }

        [HttpDelete("{courseId}/lessons/{lessonId}")]
        [Authorize(Roles = "ADMIN,COMPANY")]
        public async Task<ActionResult> DeleteLesson(int courseId, int lessonId)
        {
            var success = await _courseService.DeleteLessonAsync(courseId, lessonId);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpGet("{courseId}/reviews")]
        public async Task<ActionResult<List<ReviewResponse>>> GetCourseReviews(int courseId)
        {
            var reviews = await _courseService.GetCourseReviewsAsync(courseId);
            return Ok(reviews);
        }

        [HttpPost("{courseId}/reviews")]
        [Authorize(Roles = "CLIENTE")]
        public async Task<ActionResult<ReviewResponse>> CreateReview(int courseId, [FromBody] CreateReviewRequest request)
        {
            var review = await _courseService.CreateReviewAsync(courseId, request);
            return CreatedAtAction(nameof(GetCourseReviews), new { courseId }, review);
        }

        [HttpGet("stats")]
        [Authorize(Roles = "ADMIN,COMPANY")]
        public async Task<ActionResult<CourseStatsDTO>> GetCourseStats()
        {
            var stats = await _courseService.GetCourseStatsAsync();
            return Ok(stats);
        }

        [HttpGet("{courseId}/stats")]
        [Authorize(Roles = "ADMIN,COMPANY")]
        public async Task<ActionResult<CourseStatsDTO>> GetCourseStats(int courseId)
        {
            var stats = await _courseService.GetCourseStatsAsync(courseId);
            return Ok(stats);
        }
    }
}
