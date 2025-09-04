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
using Core.Models.Message;
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
        ConversationRepository Conversations { get; }
        MessageRepository Messages { get; }
        IGenericRepository<ConversationParticipant> ConversationParticipants { get; }
        IGenericRepository<MessageAttachment> MessageAttachments { get; }
        IGenericRepository<MessageReaction> MessageReactions { get; }
        MessageTemplateRepository MessageTemplates { get; }
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
        public ConversationRepository Conversations { get; }
        public MessageRepository Messages { get; }
        public IGenericRepository<ConversationParticipant> ConversationParticipants { get; }
        public IGenericRepository<MessageAttachment> MessageAttachments { get; }
        public IGenericRepository<MessageReaction> MessageReactions { get; }
        public MessageTemplateRepository MessageTemplates { get; }

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
            NotificationRepository notificationRepository,
            ConversationRepository conversationRepository,
            MessageRepository messageRepository,
            MessageTemplateRepository messageTemplateRepository)
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
            Conversations = conversationRepository;
            Messages = messageRepository;
            MessageTemplates = messageTemplateRepository;
            
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
            ConversationParticipants = new ConversationParticipantRepository(_dbContext);
            MessageAttachments = new MessageAttachmentRepository(_dbContext);
            MessageReactions = new MessageReactionRepository(_dbContext);
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

    // Concrete implementations for message entities
    public class ConversationParticipantRepository : GenericRepository<ConversationParticipant>
    {
        public ConversationParticipantRepository(DbContextClass context) : base(context)
        {
        }
    }

    public class MessageAttachmentRepository : GenericRepository<MessageAttachment>
    {
        public MessageAttachmentRepository(DbContextClass context) : base(context)
        {
        }
    }

    public class MessageReactionRepository : GenericRepository<MessageReaction>
    {
        public MessageReactionRepository(DbContextClass context) : base(context)
        {
        }
    }
}
