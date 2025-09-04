using Core.DTO.Course;

namespace Services
{
    public interface ICourseService
    {
        Task<CoursesPagedDTO> GetPagedCoursesAsync(CourseFiltersDTO filters, int page, int pageSize);
        Task<CourseResponse?> GetCourseByIdAsync(int id);
        Task<CourseResponse> CreateCourseAsync(CreateCourseRequest request);
        Task<CourseResponse?> UpdateCourseAsync(int id, UpdateCourseRequest request);
        Task<bool> DeleteCourseAsync(int id);
        Task<bool> PublishCourseAsync(int id);
        Task<List<LessonResponse>> GetCourseLessonsAsync(int courseId);
        Task<LessonResponse?> GetLessonByIdAsync(int courseId, int lessonId);
        Task<LessonResponse> CreateLessonAsync(int courseId, CreateLessonRequest request);
        Task<LessonResponse?> UpdateLessonAsync(int courseId, int lessonId, UpdateLessonRequest request);
        Task<bool> DeleteLessonAsync(int courseId, int lessonId);
        Task<List<EnrollmentResponse>> GetEnrollmentsAsync(int? empresasId, int? courseId, int? userId);
        Task<EnrollmentResponse?> GetEnrollmentByIdAsync(int id);
        Task<EnrollmentResponse> CreateEnrollmentAsync(CreateEnrollmentRequest request);
        Task<EnrollmentResponse?> UpdateEnrollmentAsync(int id, UpdateEnrollmentRequest request);
        Task<List<ProgressResponse>> GetProgressAsync(int userId, int courseId);
        Task<ProgressResponse?> UpdateProgressAsync(UpdateProgressRequest request);
        Task<List<ReviewResponse>> GetCourseReviewsAsync(int courseId);
        Task<ReviewResponse> CreateReviewAsync(int courseId, CreateReviewRequest request);
        Task<CourseStatsDTO> GetCourseStatsAsync();
        Task<CourseStatsDTO> GetCourseStatsAsync(int courseId);
    }
}
