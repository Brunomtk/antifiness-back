using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Core.DTO.Course;
using Core.Models.Course;
using Infrastructure.Repositories;

namespace Services
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CourseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<CoursesPagedDTO> GetPagedCoursesAsync(CourseFiltersDTO filters, int page, int pageSize)
        {
            return _unitOfWork.Courses.GetPagedCoursesAsync(filters, page, pageSize);
        }

        public async Task<CourseResponse?> GetCourseByIdAsync(int id)
        {
            var courses = await _unitOfWork.Courses.GetAll();
            var course = courses.FirstOrDefault(c => c.Id == id);

            if (course == null) return null;

            return new CourseResponse
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Thumbnail = course.Thumbnail,
                Instructor = course.Instructor,
                Category = course.Category,
                CategoryName = course.Category.ToString(),
                Level = course.Level,
                LevelName = course.Level.ToString(),
                DurationMinutes = course.DurationMinutes,
                Price = course.Price,
                Currency = course.Currency,
                Tags = string.IsNullOrEmpty(course.Tags)
                    ? new List<string>()
                    : (JsonSerializer.Deserialize<List<string>>(course.Tags) ?? new List<string>()),
                EmpresasId = course.EmpresasId,
                EmpresasName = course.Empresas != null ? course.Empresas.Name : null,
                Status = course.Status,
                StatusName = course.Status.ToString(),
                PublishedDate = course.PublishedDate,
                CreatedDate = course.CreatedDate,
                UpdatedDate = course.UpdatedDate,
                LessonsCount = course.Lessons != null ? course.Lessons.Count : 0,
                EnrollmentsCount = course.Enrollments != null ? course.Enrollments.Count : 0,
                AverageRating = (course.Reviews != null && course.Reviews.Any())
                    ? (decimal)course.Reviews.Average(r => r.Rating)
                    : 0,
                ReviewsCount = course.Reviews != null ? course.Reviews.Count : 0
            };
        }

        public async Task<CourseResponse> CreateCourseAsync(CreateCourseRequest request)
        {
            var course = new Course
            {
                Title = request.Title,
                Description = request.Description,
                Thumbnail = request.Thumbnail,
                Instructor = request.Instructor,
                Category = request.Category,
                Level = request.Level,
                DurationMinutes = request.DurationMinutes,
                Price = request.Price,
                Currency = request.Currency,
                Tags = (request.Tags != null && request.Tags.Any()) ? JsonSerializer.Serialize(request.Tags) : null,
                EmpresasId = request.EmpresasId,
                Status = Core.Enums.CourseStatus.Draft
            };

            _unitOfWork.Courses.Add(course); // sem await
            await _unitOfWork.SaveAsync();

            return await GetCourseByIdAsync(course.Id) ?? throw new InvalidOperationException("Failed to create course");
        }

        public async Task<CourseResponse?> UpdateCourseAsync(int id, UpdateCourseRequest request)
        {
            var courses = await _unitOfWork.Courses.GetAll();
            var course = courses.FirstOrDefault(c => c.Id == id);

            if (course == null) return null;

            if (!string.IsNullOrEmpty(request.Title)) course.Title = request.Title;
            if (!string.IsNullOrEmpty(request.Description)) course.Description = request.Description;
            if (request.Thumbnail != null) course.Thumbnail = request.Thumbnail;
            if (!string.IsNullOrEmpty(request.Instructor)) course.Instructor = request.Instructor;
            if (request.Category.HasValue) course.Category = request.Category.Value;
            if (request.Level.HasValue) course.Level = request.Level.Value;
            if (request.DurationMinutes.HasValue) course.DurationMinutes = request.DurationMinutes.Value;
            if (request.Price.HasValue) course.Price = request.Price.Value;
            if (!string.IsNullOrEmpty(request.Currency)) course.Currency = request.Currency;
            if (request.Tags != null) course.Tags = request.Tags.Any() ? JsonSerializer.Serialize(request.Tags) : null;
            if (request.Status.HasValue) course.Status = request.Status.Value;

            _unitOfWork.Courses.Update(course); // sem await
            await _unitOfWork.SaveAsync();

            return await GetCourseByIdAsync(id);
        }

        public async Task<bool> DeleteCourseAsync(int id)
        {
            var courses = await _unitOfWork.Courses.GetAll();
            var course = courses.FirstOrDefault(c => c.Id == id);

            if (course == null) return false;

            _unitOfWork.Courses.Delete(course); // sem await
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> PublishCourseAsync(int id)
        {
            var courses = await _unitOfWork.Courses.GetAll();
            var course = courses.FirstOrDefault(c => c.Id == id);

            if (course == null) return false;

            course.Status = Core.Enums.CourseStatus.Published;
            course.PublishedDate = DateTime.UtcNow;

            _unitOfWork.Courses.Update(course); // sem await
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<List<LessonResponse>> GetCourseLessonsAsync(int courseId)
        {
            var lessons = await _unitOfWork.Lessons.GetAll();
            var courseLessons = lessons.Where(l => l.CourseId == courseId).OrderBy(l => l.Order).ToList();

            return courseLessons.Select(l => new LessonResponse
            {
                Id = l.Id,
                Title = l.Title,
                Description = l.Description,
                Content = l.Content,
                DurationMinutes = l.DurationMinutes,
                Order = l.Order,
                Resources = string.IsNullOrEmpty(l.Resources)
                    ? new List<string>()
                    : (JsonSerializer.Deserialize<List<string>>(l.Resources) ?? new List<string>()),
                VideoUrl = l.VideoUrl,
                CourseId = l.CourseId,
                CourseTitle = l.Course != null ? l.Course.Title : null,
                CreatedDate = l.CreatedDate,
                UpdatedDate = l.UpdatedDate
            }).ToList();
        }

        public async Task<LessonResponse?> GetLessonByIdAsync(int courseId, int lessonId)
        {
            var lessons = await _unitOfWork.Lessons.GetAll();
            var lesson = lessons.FirstOrDefault(l => l.Id == lessonId && l.CourseId == courseId);

            if (lesson == null) return null;

            return new LessonResponse
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Description = lesson.Description,
                Content = lesson.Content,
                DurationMinutes = lesson.DurationMinutes,
                Order = lesson.Order,
                Resources = string.IsNullOrEmpty(lesson.Resources)
                    ? new List<string>()
                    : (JsonSerializer.Deserialize<List<string>>(lesson.Resources) ?? new List<string>()),
                VideoUrl = lesson.VideoUrl,
                CourseId = lesson.CourseId,
                CourseTitle = lesson.Course != null ? lesson.Course.Title : null,
                CreatedDate = lesson.CreatedDate,
                UpdatedDate = lesson.UpdatedDate
            };
        }

        public async Task<LessonResponse> CreateLessonAsync(int courseId, CreateLessonRequest request)
        {
            var lesson = new Lesson
            {
                Title = request.Title,
                Description = request.Description,
                Content = request.Content,
                DurationMinutes = request.DurationMinutes,
                Order = request.Order,
                Resources = (request.Resources != null && request.Resources.Any()) ? JsonSerializer.Serialize(request.Resources) : null,
                VideoUrl = request.VideoUrl,
                CourseId = courseId
            };

            _unitOfWork.Lessons.Add(lesson); // sem await
            await _unitOfWork.SaveAsync();

            return await GetLessonByIdAsync(courseId, lesson.Id) ?? throw new InvalidOperationException("Failed to create lesson");
        }

        public async Task<LessonResponse?> UpdateLessonAsync(int courseId, int lessonId, UpdateLessonRequest request)
        {
            var lessons = await _unitOfWork.Lessons.GetAll();
            var lesson = lessons.FirstOrDefault(l => l.Id == lessonId && l.CourseId == courseId);

            if (lesson == null) return null;

            if (!string.IsNullOrEmpty(request.Title)) lesson.Title = request.Title;
            if (request.Description != null) lesson.Description = request.Description;
            if (!string.IsNullOrEmpty(request.Content)) lesson.Content = request.Content;
            if (request.DurationMinutes.HasValue) lesson.DurationMinutes = request.DurationMinutes.Value;
            if (request.Order.HasValue) lesson.Order = request.Order.Value;
            if (request.Resources != null) lesson.Resources = request.Resources.Any() ? JsonSerializer.Serialize(request.Resources) : null;
            if (request.VideoUrl != null) lesson.VideoUrl = request.VideoUrl;

            _unitOfWork.Lessons.Update(lesson); // sem await
            await _unitOfWork.SaveAsync();

            return await GetLessonByIdAsync(courseId, lessonId);
        }

        public async Task<bool> DeleteLessonAsync(int courseId, int lessonId)
        {
            var lessons = await _unitOfWork.Lessons.GetAll();
            var lesson = lessons.FirstOrDefault(l => l.Id == lessonId && l.CourseId == courseId);

            if (lesson == null) return false;

            _unitOfWork.Lessons.Delete(lesson); // sem await
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<List<EnrollmentResponse>> GetEnrollmentsAsync(int? empresasId, int? courseId, int? userId)
        {
            var enrollments = await _unitOfWork.Enrollments.GetAll();
            var query = enrollments.AsQueryable(); // in-memory

            if (empresasId.HasValue) query = query.Where(e => e.EmpresasId == empresasId.Value);
            if (courseId.HasValue) query = query.Where(e => e.CourseId == courseId.Value);
            if (userId.HasValue) query = query.Where(e => e.UserId == userId.Value);

            // Evita operador ?. em expression tree
            return query.Select(e => new EnrollmentResponse
            {
                Id = e.Id,
                EmpresasId = e.EmpresasId,
                EmpresasName = e.Empresas != null ? e.Empresas.Name : null,
                CourseId = e.CourseId,
                CourseTitle = e.Course != null ? e.Course.Title : null,
                UserId = e.UserId,
                UserName = e.User != null ? e.User.Name : null,
                StartDate = e.StartDate,
                CompletionDate = e.CompletionDate,
                Status = e.Status,
                StatusName = e.Status.ToString(),
                ProgressPercentage = e.ProgressPercentage,
                CreatedDate = e.CreatedDate,
                UpdatedDate = e.UpdatedDate,
                CompletedLessons = e.Progress != null ? e.Progress.Count(p => p.IsCompleted) : 0,
                TotalLessons = (e.Course != null && e.Course.Lessons != null) ? e.Course.Lessons.Count : 0
            }).ToList();
        }

        public async Task<EnrollmentResponse?> GetEnrollmentByIdAsync(int id)
        {
            var enrollments = await _unitOfWork.Enrollments.GetAll();
            var enrollment = enrollments.FirstOrDefault(e => e.Id == id);

            if (enrollment == null) return null;

            return new EnrollmentResponse
            {
                Id = enrollment.Id,
                EmpresasId = enrollment.EmpresasId,
                EmpresasName = enrollment.Empresas != null ? enrollment.Empresas.Name : null,
                CourseId = enrollment.CourseId,
                CourseTitle = enrollment.Course != null ? enrollment.Course.Title : null,
                UserId = enrollment.UserId,
                UserName = enrollment.User != null ? enrollment.User.Name : null,
                StartDate = enrollment.StartDate,
                CompletionDate = enrollment.CompletionDate,
                Status = enrollment.Status,
                StatusName = enrollment.Status.ToString(),
                ProgressPercentage = enrollment.ProgressPercentage,
                CreatedDate = enrollment.CreatedDate,
                UpdatedDate = enrollment.UpdatedDate,
                CompletedLessons = enrollment.Progress != null ? enrollment.Progress.Count(p => p.IsCompleted) : 0,
                TotalLessons = (enrollment.Course != null && enrollment.Course.Lessons != null) ? enrollment.Course.Lessons.Count : 0
            };
        }

        public async Task<EnrollmentResponse> CreateEnrollmentAsync(CreateEnrollmentRequest request)
        {
            var enrollment = new Enrollment
            {
                EmpresasId = request.EmpresasId,
                CourseId = request.CourseId,
                UserId = request.UserId,
                StartDate = request.StartDate ?? DateTime.UtcNow,
                Status = Core.Enums.EnrollmentStatus.Active
            };

            _unitOfWork.Enrollments.Add(enrollment); // sem await
            await _unitOfWork.SaveAsync();

            return await GetEnrollmentByIdAsync(enrollment.Id)
                   ?? throw new InvalidOperationException("Failed to create enrollment");
        }

        public async Task<EnrollmentResponse?> UpdateEnrollmentAsync(int id, UpdateEnrollmentRequest request)
        {
            var enrollments = await _unitOfWork.Enrollments.GetAll();
            var enrollment = enrollments.FirstOrDefault(e => e.Id == id);

            if (enrollment == null) return null;

            if (request.Status.HasValue) enrollment.Status = request.Status.Value;
            if (request.CompletionDate.HasValue) enrollment.CompletionDate = request.CompletionDate.Value;
            if (request.ProgressPercentage.HasValue) enrollment.ProgressPercentage = request.ProgressPercentage.Value;

            _unitOfWork.Enrollments.Update(enrollment); // sem await
            await _unitOfWork.SaveAsync();

            return await GetEnrollmentByIdAsync(id);
        }

        public async Task<List<ProgressResponse>> GetProgressAsync(int userId, int courseId)
        {
            var progress = await _unitOfWork.Progress.GetAll();
            var enrollments = await _unitOfWork.Enrollments.GetAll();

            var enrollment = enrollments.FirstOrDefault(e => e.UserId == userId && e.CourseId == courseId);
            if (enrollment == null) return new List<ProgressResponse>();

            var userProgress = progress.Where(p => p.EnrollmentId == enrollment.Id).ToList();

            return userProgress.Select(p => new ProgressResponse
            {
                Id = p.Id,
                EnrollmentId = p.EnrollmentId,
                LessonId = p.LessonId,
                LessonTitle = p.Lesson != null ? p.Lesson.Title : null,
                IsCompleted = p.IsCompleted,
                CompletedDate = p.CompletedDate,
                WatchTimeMinutes = p.WatchTimeMinutes,
                CreatedDate = p.CreatedDate,
                UpdatedDate = p.UpdatedDate
            }).ToList();
        }

        public async Task<ProgressResponse?> UpdateProgressAsync(UpdateProgressRequest request)
        {
            var progress = await _unitOfWork.Progress.GetAll();
            var existingProgress = progress.FirstOrDefault(p => p.EnrollmentId == request.EnrollmentId && p.LessonId == request.LessonId);

            if (existingProgress == null)
            {
                var newProgress = new Progress
                {
                    EnrollmentId = request.EnrollmentId,
                    LessonId = request.LessonId,
                    IsCompleted = request.IsCompleted,
                    CompletedDate = request.IsCompleted ? DateTime.UtcNow : (DateTime?)null,
                    WatchTimeMinutes = request.WatchTimeMinutes
                };

                _unitOfWork.Progress.Add(newProgress); // sem await
                await _unitOfWork.SaveAsync();

                return new ProgressResponse
                {
                    Id = newProgress.Id,
                    EnrollmentId = newProgress.EnrollmentId,
                    LessonId = newProgress.LessonId,
                    IsCompleted = newProgress.IsCompleted,
                    CompletedDate = newProgress.CompletedDate,
                    WatchTimeMinutes = newProgress.WatchTimeMinutes,
                    CreatedDate = newProgress.CreatedDate,
                    UpdatedDate = newProgress.UpdatedDate
                };
            }
            else
            {
                existingProgress.IsCompleted = request.IsCompleted;
                existingProgress.CompletedDate = request.IsCompleted ? DateTime.UtcNow : (DateTime?)null;
                existingProgress.WatchTimeMinutes = request.WatchTimeMinutes;

                _unitOfWork.Progress.Update(existingProgress); // sem await
                await _unitOfWork.SaveAsync();

                return new ProgressResponse
                {
                    Id = existingProgress.Id,
                    EnrollmentId = existingProgress.EnrollmentId,
                    LessonId = existingProgress.LessonId,
                    IsCompleted = existingProgress.IsCompleted,
                    CompletedDate = existingProgress.CompletedDate,
                    WatchTimeMinutes = existingProgress.WatchTimeMinutes,
                    CreatedDate = existingProgress.CreatedDate,
                    UpdatedDate = existingProgress.UpdatedDate
                };
            }
        }

        public async Task<List<ReviewResponse>> GetCourseReviewsAsync(int courseId)
        {
            var reviews = await _unitOfWork.Reviews.GetAll();
            var courseReviews = reviews.Where(r => r.CourseId == courseId)
                                       .OrderByDescending(r => r.ReviewDate)
                                       .ToList();

            return courseReviews.Select(r => new ReviewResponse
            {
                Id = r.Id,
                CourseId = r.CourseId,
                CourseTitle = r.Course != null ? r.Course.Title : null,
                UserId = r.UserId,
                UserName = r.User != null ? r.User.Name : null,
                Rating = r.Rating,
                Comment = r.Comment,
                ReviewDate = r.ReviewDate,
                CreatedDate = r.CreatedDate,
                UpdatedDate = r.UpdatedDate
            }).ToList();
        }

        public async Task<ReviewResponse> CreateReviewAsync(int courseId, CreateReviewRequest request)
        {
            var review = new Review
            {
                CourseId = courseId,
                UserId = request.UserId,
                Rating = request.Rating,
                Comment = request.Comment,
                ReviewDate = DateTime.UtcNow
            };

            _unitOfWork.Reviews.Add(review); // sem await
            await _unitOfWork.SaveAsync();

            var reviews = await _unitOfWork.Reviews.GetAll();
            var createdReview = reviews.FirstOrDefault(r => r.Id == review.Id);

            return new ReviewResponse
            {
                Id = review.Id,
                CourseId = review.CourseId,
                CourseTitle = createdReview != null && createdReview.Course != null ? createdReview.Course.Title : null,
                UserId = review.UserId,
                UserName = createdReview != null && createdReview.User != null ? createdReview.User.Name : null,
                Rating = review.Rating,
                Comment = review.Comment,
                ReviewDate = review.ReviewDate,
                CreatedDate = review.CreatedDate,
                UpdatedDate = review.UpdatedDate
            };
        }

        public Task<CourseStatsDTO> GetCourseStatsAsync()
        {
            return _unitOfWork.Courses.GetCourseStatsAsync();
        }

        public async Task<CourseStatsDTO> GetCourseStatsAsync(int courseId)
        {
            var courses = await _unitOfWork.Courses.GetAll();
            var course = courses.FirstOrDefault(c => c.Id == courseId);

            if (course == null) return new CourseStatsDTO();

            var enrollments = course.Enrollments != null ? course.Enrollments.ToList() : new List<Enrollment>();
            var reviews = course.Reviews != null ? course.Reviews.ToList() : new List<Review>();

            return new CourseStatsDTO
            {
                TotalCourses = 1,
                PublishedCourses = course.Status == Core.Enums.CourseStatus.Published ? 1 : 0,
                DraftCourses = course.Status == Core.Enums.CourseStatus.Draft ? 1 : 0,
                TotalEnrollments = enrollments.Count,
                ActiveEnrollments = enrollments.Count(e => e.Status == Core.Enums.EnrollmentStatus.Active),
                CompletedEnrollments = enrollments.Count(e => e.Status == Core.Enums.EnrollmentStatus.Completed),
                TotalRevenue = enrollments.Sum(e => course.Price),
                AverageRating = reviews.Any() ? (decimal)reviews.Average(r => r.Rating) : 0,
                TotalReviews = reviews.Count,
                CompletionRate = enrollments.Count > 0
                    ? (decimal)enrollments.Count(e => e.Status == Core.Enums.EnrollmentStatus.Completed) / enrollments.Count * 100
                    : 0
            };
        }
    }
}
