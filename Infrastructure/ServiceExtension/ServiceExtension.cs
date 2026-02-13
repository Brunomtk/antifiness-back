// File: Infrastructure/ServiceExtension/ServiceExtension.cs
using Core.Models;
using Core.Models.Client;
using Core.Models.Plan;
using Core.Models.Diet;
using Core.Models.Workout;
using Core.Models.Feedback;
using Core.Models.Course;
using Core.Models.Billing;
using Infrastructure.Authenticate;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Billing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.ServiceExtension
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddDIServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")!;

            // DbContext
            services.AddDbContext<DbContextClass>(opts =>
                opts.UseNpgsql(connectionString)
            );

            // Add repositories
            services.AddRepositories();

            // JWT
            services.AddSingleton<IJWTManager, JWTManager>();

            return services;
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEmpresasRepository, EmpresasRepository>();
            services.AddScoped<IPlanRepository, PlanRepository>();
            services.AddScoped<IPlanGoalRepository, PlanGoalRepository>();
            services.AddScoped<IPlanMealRepository, PlanMealRepository>();
            services.AddScoped<IPlanFoodRepository, PlanFoodRepository>();
            services.AddScoped<IPlanProgressRepository, PlanProgressRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IClientGoalRepository, ClientGoalRepository>();
            services.AddScoped<IClientMeasurementRepository, ClientMeasurementRepository>();
            services.AddScoped<DietRepository>();
            services.AddScoped<FoodRepository>();
            services.AddScoped<ExerciseRepository>();
            services.AddScoped<WorkoutRepository>();
            services.AddScoped<FeedbackRepository>();
            services.AddScoped<CourseRepository>();
            services.AddScoped<NotificationRepository>();
            services.AddScoped<ICompanySubscriptionRepository, CompanySubscriptionRepository>();

            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
