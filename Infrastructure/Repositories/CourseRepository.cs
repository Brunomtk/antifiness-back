using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.DTO.Course;
using Core.Models.Course;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CourseRepository : GenericRepository<Course>
    {
        public CourseRepository(DbContextClass context) : base(context)
        {
        }

        public async Task<CoursesPagedDTO> GetPagedCoursesAsync(CourseFiltersDTO filters, int page, int pageSize)
        {
            var courses = await GetAll();
            var query = courses.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(filters.Search))
            {
                query = query.Where(c => c.Title.Contains(filters.Search, StringComparison.OrdinalIgnoreCase) ||
                                        c.Description.Contains(filters.Search, StringComparison.OrdinalIgnoreCase) ||
                                        c.Instructor.Contains(filters.Search, StringComparison.OrdinalIgnoreCase));
            }

            if (filters.Category.HasValue)
                query = query.Where(c => c.Category == filters.Category.Value);

            if (filters.Level.HasValue)
                query = query.Where(c => c.Level == filters.Level.Value);

            if (filters.Status.HasValue)
                query = query.Where(c => c.Status == filters.Status.Value);

            if (filters.MinPrice.HasValue)
                query = query.Where(c => c.Price >= filters.MinPrice.Value);

            if (filters.MaxPrice.HasValue)
                query = query.Where(c => c.Price <= filters.MaxPrice.Value);

            if (filters.StartDate.HasValue)
                query = query.Where(c => c.CreatedDate >= filters.StartDate.Value);

            if (filters.EndDate.HasValue)
                query = query.Where(c => c.CreatedDate <= filters.EndDate.Value);

            if (filters.EmpresasId.HasValue)
                query = query.Where(c => c.EmpresasId == filters.EmpresasId.Value);

            if (!string.IsNullOrEmpty(filters.Instructor))
                query = query.Where(c => c.Instructor.Contains(filters.Instructor, StringComparison.OrdinalIgnoreCase));

            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var pagedCourses = query
                .OrderByDescending(c => c.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var courseResponses = pagedCourses.Select(c => new CourseResponse
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                Thumbnail = c.Thumbnail,
                Instructor = c.Instructor,
                Category = c.Category,
                CategoryName = c.Category.ToString(),
                Level = c.Level,
                LevelName = c.Level.ToString(),
                DurationMinutes = c.DurationMinutes,
                Price = c.Price,
                Currency = c.Currency,
                Tags = string.IsNullOrEmpty(c.Tags) ? new List<string>() : 
                       System.Text.Json.JsonSerializer.Deserialize<List<string>>(c.Tags) ?? new List<string>(),
                EmpresasId = c.EmpresasId,
                EmpresasName = c.Empresas?.Name,
                Status = c.Status,
                StatusName = c.Status.ToString(),
                PublishedDate = c.PublishedDate,
                CreatedDate = c.CreatedDate,
                UpdatedDate = c.UpdatedDate,
                LessonsCount = c.Lessons?.Count ?? 0,
                EnrollmentsCount = c.Enrollments?.Count ?? 0,
                AverageRating = c.Reviews?.Any() == true ? (decimal)c.Reviews.Average(r => r.Rating) : 0,
                ReviewsCount = c.Reviews?.Count ?? 0
            }).ToList();

            return new CoursesPagedDTO
            {
                Courses = courseResponses,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                HasNextPage = page < totalPages,
                HasPreviousPage = page > 1
            };
        }

        public async Task<CourseStatsDTO> GetCourseStatsAsync()
        {
            var courses = await GetAll();
            var coursesList = courses.ToList();

            var totalCourses = coursesList.Count;
            var publishedCourses = coursesList.Count(c => c.Status == Core.Enums.CourseStatus.Published);
            var draftCourses = coursesList.Count(c => c.Status == Core.Enums.CourseStatus.Draft);

            var allEnrollments = coursesList.SelectMany(c => c.Enrollments ?? new List<Enrollment>()).ToList();
            var totalEnrollments = allEnrollments.Count;
            var activeEnrollments = allEnrollments.Count(e => e.Status == Core.Enums.EnrollmentStatus.Active);
            var completedEnrollments = allEnrollments.Count(e => e.Status == Core.Enums.EnrollmentStatus.Completed);

            var totalRevenue = allEnrollments.Sum(e => e.Course?.Price ?? 0);

            var allReviews = coursesList.SelectMany(c => c.Reviews ?? new List<Review>()).ToList();
            var averageRating = allReviews.Any() ? (decimal)allReviews.Average(r => r.Rating) : 0;
            var totalReviews = allReviews.Count;

            var completionRate = totalEnrollments > 0 ? (decimal)completedEnrollments / totalEnrollments * 100 : 0;

            var popularCategories = coursesList
                .GroupBy(c => c.Category)
                .Select(g => new CategoryStats
                {
                    Category = g.Key.ToString(),
                    Count = g.Count(),
                    Percentage = totalCourses > 0 ? (decimal)g.Count() / totalCourses * 100 : 0
                })
                .OrderByDescending(c => c.Count)
                .Take(5)
                .ToList();

            var monthlyEnrollments = allEnrollments
                .GroupBy(e => new { e.StartDate.Year, e.StartDate.Month })
                .Select(g => new MonthlyEnrollment
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Count = g.Count(),
                    Revenue = g.Sum(e => e.Course?.Price ?? 0)
                })
                .OrderBy(m => m.Month)
                .Take(12)
                .ToList();

            return new CourseStatsDTO
            {
                TotalCourses = totalCourses,
                PublishedCourses = publishedCourses,
                DraftCourses = draftCourses,
                TotalEnrollments = totalEnrollments,
                ActiveEnrollments = activeEnrollments,
                CompletedEnrollments = completedEnrollments,
                TotalRevenue = totalRevenue,
                AverageRating = averageRating,
                TotalReviews = totalReviews,
                CompletionRate = completionRate,
                PopularCategories = popularCategories,
                MonthlyEnrollments = monthlyEnrollments
            };
        }
    }
}
