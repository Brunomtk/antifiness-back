using System;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.Client;
using Core.Models.Plan;
using Core.Models.Diet;
using Core.Models.Workout;
using Core.Models.Feedback;
using Core.Models.Course;
using Core.Models.Notification;
using Core.Models.Nutrition;
using Core.Models.AppSettings;
using Core.Models.Billing;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IEmpresasRepository Empresas { get; }
        IPlanRepository Plans { get; }
        IClientRepository Clients { get; }
        ExerciseRepository Exercises { get; }
        WorkoutRepository Workouts { get; }
        IGenericRepository<WorkoutExercise> WorkoutExercises { get; }
        IGenericRepository<WorkoutProgress> WorkoutProgress { get; }
        FeedbackRepository Feedbacks { get; }
        CourseRepository Courses { get; }
        IGenericRepository<Lesson> Lessons { get; }
        IGenericRepository<Enrollment> Enrollments { get; }
        IGenericRepository<Progress> Progress { get; }
        IGenericRepository<Review> Reviews { get; }
        NotificationRepository Notifications { get; }
        IGenericRepository<NotificationSettings> NotificationSettings { get; }
        IGenericRepository<NotificationTemplate> NotificationTemplates { get; }
        IGenericRepository<NotificationSubscription> NotificationSubscriptions { get; }
        IGenericRepository<MicronutrientType> MicronutrientTypes { get; }
        IGenericRepository<FoodMicronutrient> FoodMicronutrients { get; }
        IGenericRepository<ClientAchievement> ClientAchievements { get; }
        IGenericRepository<AppSetting> AppSettings { get; }
        IGenericRepository<SubscriptionPlan> SubscriptionPlans { get; }
        IGenericRepository<CompanySubscription> CompanySubscriptions { get; }
        int Save();
        Task<int> SaveAsync();
    }

    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly DbContextClass _dbContext;

        public IUserRepository Users { get; }
        public IEmpresasRepository Empresas { get; }
        public IPlanRepository Plans { get; }
        public IClientRepository Clients { get; }
        public ExerciseRepository Exercises { get; }
        public WorkoutRepository Workouts { get; }
        public IGenericRepository<WorkoutExercise> WorkoutExercises { get; }
        public IGenericRepository<WorkoutProgress> WorkoutProgress { get; }
        public FeedbackRepository Feedbacks { get; }
        public CourseRepository Courses { get; }
        public IGenericRepository<Lesson> Lessons { get; }
        public IGenericRepository<Enrollment> Enrollments { get; }
        public IGenericRepository<Progress> Progress { get; }
        public IGenericRepository<Review> Reviews { get; }
        public NotificationRepository Notifications { get; }
        public IGenericRepository<NotificationSettings> NotificationSettings { get; }
        public IGenericRepository<NotificationTemplate> NotificationTemplates { get; }
        public IGenericRepository<NotificationSubscription> NotificationSubscriptions { get; }
        public IGenericRepository<MicronutrientType> MicronutrientTypes { get; }
        public IGenericRepository<FoodMicronutrient> FoodMicronutrients { get; }
        public IGenericRepository<ClientAchievement> ClientAchievements { get; }
        public IGenericRepository<AppSetting> AppSettings { get; }
        public IGenericRepository<SubscriptionPlan> SubscriptionPlans { get; }
        public IGenericRepository<CompanySubscription> CompanySubscriptions { get; }

        public UnitOfWork(
            DbContextClass dbContext,
            IUserRepository userRepository,
            IEmpresasRepository empresasRepository,
            IPlanRepository planRepository,
            IClientRepository clientRepository,
            ExerciseRepository exerciseRepository,
            WorkoutRepository workoutRepository,
            FeedbackRepository feedbackRepository,
            CourseRepository courseRepository,
            NotificationRepository notificationRepository)
        {
            _dbContext = dbContext;
            Users = userRepository;
            Empresas = empresasRepository;
            Plans = planRepository;
            Clients = clientRepository;
            Exercises = exerciseRepository;
            Workouts = workoutRepository;
            Feedbacks = feedbackRepository;
            Courses = courseRepository;
            Notifications = notificationRepository;
            
            MicronutrientTypes = new MicronutrientTypeRepository(_dbContext);
            FoodMicronutrients = new FoodMicronutrientRepository(_dbContext);
            ClientAchievements = new GenericRepositoryImpl<ClientAchievement>(_dbContext);
            AppSettings = new GenericRepositoryImpl<AppSetting>(_dbContext);
            SubscriptionPlans = new GenericRepositoryImpl<SubscriptionPlan>(_dbContext);
            CompanySubscriptions = new GenericRepositoryImpl<CompanySubscription>(_dbContext);

// Create concrete implementations of generic repositories
            WorkoutExercises = new WorkoutExerciseRepository(_dbContext);
            WorkoutProgress = new WorkoutProgressRepository(_dbContext);
            Lessons = new LessonRepository(_dbContext);
            Enrollments = new EnrollmentRepository(_dbContext);
            Progress = new ProgressRepository(_dbContext);
            Reviews = new ReviewRepository(_dbContext);
            NotificationSettings = new NotificationSettingsRepository(_dbContext);
            NotificationTemplates = new NotificationTemplateRepository(_dbContext);
            NotificationSubscriptions = new NotificationSubscriptionRepository(_dbContext);
        }

        public int Save()
        {
            return _dbContext.SaveChanges();
        }

        public Task<int> SaveAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    // Concrete implementations for workout entities
    public class WorkoutExerciseRepository : GenericRepository<WorkoutExercise>
    {
        public WorkoutExerciseRepository(DbContextClass context) : base(context)
        {
        }
    }

    public class WorkoutProgressRepository : GenericRepository<WorkoutProgress>
    {
        public WorkoutProgressRepository(DbContextClass context) : base(context)
        {
        }
    }

    // Concrete implementations for course entities
    public class LessonRepository : GenericRepository<Lesson>
    {
        public LessonRepository(DbContextClass context) : base(context)
        {
        }
    }

    public class EnrollmentRepository : GenericRepository<Enrollment>
    {
        public EnrollmentRepository(DbContextClass context) : base(context)
        {
        }
    }

    public class ProgressRepository : GenericRepository<Progress>
    {
        public ProgressRepository(DbContextClass context) : base(context)
        {
        }
    }

    public class ReviewRepository : GenericRepository<Review>
    {
        public ReviewRepository(DbContextClass context) : base(context)
        {
        }
    }

    // Concrete implementations for notification entities
    public class NotificationSettingsRepository : GenericRepository<NotificationSettings>
    {
        public NotificationSettingsRepository(DbContextClass context) : base(context)
        {
        }
    }

    public class NotificationTemplateRepository : GenericRepository<NotificationTemplate>
    {
        public NotificationTemplateRepository(DbContextClass context) : base(context)
        {
        }
    }

    public class NotificationSubscriptionRepository : GenericRepository<NotificationSubscription>
    {
        public NotificationSubscriptionRepository(DbContextClass context) : base(context)
        {
        }
    }
}
