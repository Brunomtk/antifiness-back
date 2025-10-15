// File: Infrastructure/DbContextClass.cs
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.Client;
using Core.Models.Plan;
using Core.Models.Workout;
using Core.Models.Diet;
using Core.Models.Feedback;
using Core.Models.Course;
using Core.Models.Notification;
using Core.Models.Message;
using Core.Models.Nutrition;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public sealed class DbContextClass : DbContext
    {
        public DbContextClass(DbContextOptions<DbContextClass> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Empresas> Empresas { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<PlanGoal> PlanGoals { get; set; }
        public DbSet<PlanMeal> PlanMeals { get; set; }
        public DbSet<PlanFood> PlanFoods { get; set; }
        public DbSet<PlanProgress> PlanProgress { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ClientGoal> ClientGoals { get; set; }
        public DbSet<ClientMeasurement> ClientMeasurements { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<WorkoutExercise> WorkoutExercises { get; set; }
        public DbSet<WorkoutProgress> WorkoutProgress { get; set; }
        public DbSet<Core.Models.Diet.Diet> Diets { get; set; }
        public DbSet<Core.Models.Diet.DietMeal> DietMeals { get; set; }
        public DbSet<Core.Models.Diet.DietMealFood> DietMealFoods { get; set; }
        public DbSet<Core.Models.Diet.Food> Foods { get; set; }
        public DbSet<Core.Models.Diet.DietProgress> DietProgress { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Progress> Progress { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Core.Models.Notification.Notification> Notifications { get; set; }
        public DbSet<NotificationSettings> NotificationSettings { get; set; }
        public DbSet<NotificationTemplate> NotificationTemplates { get; set; }
        public DbSet<NotificationSubscription> NotificationSubscriptions { get; set; }
        public DbSet<MicronutrientType> MicronutrientTypes { get; set; }
        public DbSet<FoodMicronutrient> FoodMicronutrients { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ConversationParticipant> ConversationParticipants { get; set; }
        public DbSet<MessageAttachment> MessageAttachments { get; set; }
        public DbSet<MessageReaction> MessageReactions { get; set; }
        public DbSet<MessageTemplate> MessageTemplates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Keys
            modelBuilder.Entity<User>().HasKey(e => e.Id);
            modelBuilder.Entity<Empresas>().HasKey(e => e.Id);
            modelBuilder.Entity<Plan>().HasKey(e => e.Id);
            modelBuilder.Entity<PlanGoal>().HasKey(e => e.Id);
            modelBuilder.Entity<PlanMeal>().HasKey(e => e.Id);
            modelBuilder.Entity<PlanFood>().HasKey(e => e.Id);
            modelBuilder.Entity<PlanProgress>().HasKey(e => e.Id);
            modelBuilder.Entity<Client>().HasKey(e => e.Id);
            modelBuilder.Entity<ClientGoal>().HasKey(e => e.Id);
            modelBuilder.Entity<ClientMeasurement>().HasKey(e => e.Id);
            modelBuilder.Entity<Exercise>().HasKey(e => e.Id);
            modelBuilder.Entity<Workout>().HasKey(e => e.Id);
            modelBuilder.Entity<WorkoutExercise>().HasKey(e => e.Id);
            modelBuilder.Entity<WorkoutProgress>().HasKey(e => e.Id);
            modelBuilder.Entity<Core.Models.Diet.Diet>().HasKey(e => e.Id);
            modelBuilder.Entity<Core.Models.Diet.DietMeal>().HasKey(e => e.Id);
            modelBuilder.Entity<Core.Models.Diet.DietMealFood>().HasKey(e => e.Id);
            modelBuilder.Entity<Core.Models.Diet.Food>().HasKey(e => e.Id);
            modelBuilder.Entity<Core.Models.Diet.DietProgress>().HasKey(e => e.Id);
            modelBuilder.Entity<Feedback>().HasKey(e => e.Id);
            modelBuilder.Entity<Course>().HasKey(e => e.Id);
            modelBuilder.Entity<Lesson>().HasKey(e => e.Id);
            modelBuilder.Entity<Enrollment>().HasKey(e => e.Id);
            modelBuilder.Entity<Progress>().HasKey(e => e.Id);
            modelBuilder.Entity<Review>().HasKey(e => e.Id);
            modelBuilder.Entity<Core.Models.Notification.Notification>().HasKey(e => e.Id);
            modelBuilder.Entity<NotificationSettings>().HasKey(e => e.Id);
            modelBuilder.Entity<NotificationTemplate>().HasKey(e => e.Id);
            modelBuilder.Entity<NotificationSubscription>().HasKey(e => e.Id);
            modelBuilder.Entity<Conversation>().HasKey(e => e.Id);
            modelBuilder.Entity<Message>().HasKey(e => e.Id);
            modelBuilder.Entity<ConversationParticipant>().HasKey(e => e.Id);
            modelBuilder.Entity<MessageAttachment>().HasKey(e => e.Id);
            modelBuilder.Entity<MessageReaction>().HasKey(e => e.Id);
            modelBuilder.Entity<MessageTemplate>().HasKey(e => e.Id);

            // Plan owned types
            modelBuilder.Entity<PlanMeal>()
                .OwnsOne(m => m.Macros);
            modelBuilder.Entity<PlanFood>()
                .OwnsOne(f => f.Macros);

            // Ignore Photos collection on PlanProgress
            modelBuilder.Entity<PlanProgress>()
                .Ignore(p => p.Photos);

            // Client Preferences – ignore primitive collections, map owned objects
            modelBuilder.Entity<Client>()
                .OwnsOne(c => c.Preferences, prefs =>
                {
                    prefs.Ignore(p => p.DietaryRestrictions);
                    prefs.Ignore(p => p.FavoriteFood);
                    prefs.Ignore(p => p.DislikedFood);

                    prefs.OwnsOne(p => p.MealTimes, mt =>
                    {
                        mt.Ignore(m => m.Snacks);
                    });

                    prefs.OwnsOne(p => p.WorkoutPreferences, wp =>
                    {
                        wp.Ignore(w => w.Types);
                    });
                });

            
            // Owned types: Micronutrientes no mesmo registro
            modelBuilder.Entity<Core.Models.Diet.Food>().OwnsOne(f => f.Micros, nb =>
            {
                nb.Property(p => p.VitaminA).HasColumnName("Micros_VitaminA");
                nb.Property(p => p.VitaminC).HasColumnName("Micros_VitaminC");
                nb.Property(p => p.VitaminD).HasColumnName("Micros_VitaminD");
                nb.Property(p => p.VitaminE).HasColumnName("Micros_VitaminE");
                nb.Property(p => p.VitaminK).HasColumnName("Micros_VitaminK");
                nb.Property(p => p.VitaminB1).HasColumnName("Micros_VitaminB1");
                nb.Property(p => p.VitaminB2).HasColumnName("Micros_VitaminB2");
                nb.Property(p => p.VitaminB3).HasColumnName("Micros_VitaminB3");
                nb.Property(p => p.VitaminB6).HasColumnName("Micros_VitaminB6");
                nb.Property(p => p.Folate).HasColumnName("Micros_Folate");
                nb.Property(p => p.VitaminB12).HasColumnName("Micros_VitaminB12");
                nb.Property(p => p.Calcium).HasColumnName("Micros_Calcium");
                nb.Property(p => p.Iron).HasColumnName("Micros_Iron");
                nb.Property(p => p.Magnesium).HasColumnName("Micros_Magnesium");
                nb.Property(p => p.Potassium).HasColumnName("Micros_Potassium");
                nb.Property(p => p.Zinc).HasColumnName("Micros_Zinc");
                nb.Property(p => p.Sodium).HasColumnName("Micros_Sodium");
                nb.Property(p => p.Selenium).HasColumnName("Micros_Selenium");
                nb.Property(p => p.Phosphorus).HasColumnName("Micros_Phosphorus");
                nb.Property(p => p.Copper).HasColumnName("Micros_Copper");
                nb.Property(p => p.Manganese).HasColumnName("Micros_Manganese");
                nb.Property(p => p.Iodine).HasColumnName("Micros_Iodine");
            });

            modelBuilder.Entity<Core.Models.Diet.Diet>().OwnsOne(d => d.Micros, nb =>
            {
                nb.Property(p => p.VitaminA).HasColumnName("Micros_VitaminA");
                nb.Property(p => p.VitaminC).HasColumnName("Micros_VitaminC");
                nb.Property(p => p.VitaminD).HasColumnName("Micros_VitaminD");
                nb.Property(p => p.VitaminE).HasColumnName("Micros_VitaminE");
                nb.Property(p => p.VitaminK).HasColumnName("Micros_VitaminK");
                nb.Property(p => p.VitaminB1).HasColumnName("Micros_VitaminB1");
                nb.Property(p => p.VitaminB2).HasColumnName("Micros_VitaminB2");
                nb.Property(p => p.VitaminB3).HasColumnName("Micros_VitaminB3");
                nb.Property(p => p.VitaminB6).HasColumnName("Micros_VitaminB6");
                nb.Property(p => p.Folate).HasColumnName("Micros_Folate");
                nb.Property(p => p.VitaminB12).HasColumnName("Micros_VitaminB12");
                nb.Property(p => p.Calcium).HasColumnName("Micros_Calcium");
                nb.Property(p => p.Iron).HasColumnName("Micros_Iron");
                nb.Property(p => p.Magnesium).HasColumnName("Micros_Magnesium");
                nb.Property(p => p.Potassium).HasColumnName("Micros_Potassium");
                nb.Property(p => p.Zinc).HasColumnName("Micros_Zinc");
                nb.Property(p => p.Sodium).HasColumnName("Micros_Sodium");
                nb.Property(p => p.Selenium).HasColumnName("Micros_Selenium");
                nb.Property(p => p.Phosphorus).HasColumnName("Micros_Phosphorus");
                nb.Property(p => p.Copper).HasColumnName("Micros_Copper");
                nb.Property(p => p.Manganese).HasColumnName("Micros_Manganese");
                nb.Property(p => p.Iodine).HasColumnName("Micros_Iodine");
            });
// Diet relationships
            modelBuilder.Entity<Core.Models.Diet.Diet>()
                .HasMany(d => d.Meals)
                .WithOne(m => m.Diet)
                .HasForeignKey(m => m.DietId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Core.Models.Diet.Diet>()
                .HasMany(d => d.Progress)
                .WithOne(p => p.Diet)
                .HasForeignKey(p => p.DietId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Core.Models.Diet.DietMeal>()
                .HasMany(m => m.Foods)
                .WithOne(f => f.Meal)
                .HasForeignKey(f => f.MealId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Core.Models.Diet.DietMealFood>()
                .HasOne(f => f.Food)
                .WithMany()
                .HasForeignKey(f => f.FoodId)
                .OnDelete(DeleteBehavior.Restrict);

            // Feedback relationships
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Client)
                .WithMany()
                .HasForeignKey(f => f.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Trainer)
                .WithMany()
                .HasForeignKey(f => f.TrainerId)
                .OnDelete(DeleteBehavior.SetNull);

            // Course relationships
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Empresas)
                .WithMany()
                .HasForeignKey(c => c.EmpresasId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Lessons)
                .WithOne(l => l.Course)
                .HasForeignKey(l => l.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Enrollments)
                .WithOne(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Reviews)
                .WithOne(r => r.Course)
                .HasForeignKey(r => r.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Empresas)
                .WithMany()
                .HasForeignKey(e => e.EmpresasId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Enrollment>()
                .HasMany(e => e.Progress)
                .WithOne(p => p.Enrollment)
                .HasForeignKey(p => p.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Progress>()
                .HasOne(p => p.Lesson)
                .WithMany(l => l.Progress)
                .HasForeignKey(p => p.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Notification relationships
            modelBuilder.Entity<Core.Models.Notification.Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Core.Models.Notification.Notification>()
                .OwnsOne(n => n.Data);

            modelBuilder.Entity<NotificationSettings>()
                .HasOne(ns => ns.User)
                .WithMany()
                .HasForeignKey(ns => ns.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NotificationSettings>()
                .OwnsOne(ns => ns.Categories);

            modelBuilder.Entity<NotificationSettings>()
                .OwnsOne(ns => ns.Types);

            modelBuilder.Entity<NotificationSettings>()
                .OwnsOne(ns => ns.QuietHours);

            modelBuilder.Entity<NotificationSubscription>()
                .HasOne(ns => ns.User)
                .WithMany()
                .HasForeignKey(ns => ns.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Message relationships
            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.Empresas)
                .WithMany()
                .HasForeignKey(c => c.EmpresasId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Conversation>()
                .HasMany(c => c.Participants)
                .WithOne(p => p.Conversation)
                .HasForeignKey(p => p.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Conversation>()
                .HasMany(c => c.Messages)
                .WithOne(m => m.Conversation)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.LastMessage)
                .WithMany()
                .HasForeignKey(c => c.LastMessageId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Conversation>()
                .OwnsOne(c => c.Settings);

            modelBuilder.Entity<ConversationParticipant>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ConversationParticipant>()
                .OwnsOne(p => p.Permissions);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.ReplyTo)
                .WithMany(m => m.Replies)
                .HasForeignKey(m => m.ReplyToId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Message>()
                .HasMany(m => m.Attachments)
                .WithOne(a => a.Message)
                .HasForeignKey(a => a.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasMany(m => m.Reactions)
                .WithOne(r => r.Message)
                .HasForeignKey(r => r.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .OwnsOne(m => m.Metadata);

            modelBuilder.Entity<MessageAttachment>()
                .OwnsOne(a => a.Metadata);

            modelBuilder.Entity<MessageReaction>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MessageTemplate>()
                .HasOne(t => t.Empresas)
                .WithMany()
                .HasForeignKey(t => t.EmpresasId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<MessageTemplate>()
                .HasOne(t => t.Creator)
                .WithMany()
                .HasForeignKey(t => t.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MessageTemplate>()
                .OwnsOne(t => t.Variables);

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var now = DateTime.UtcNow;
            var entries = ChangeTracker
                .Entries<BaseModel>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                entry.Entity.UpdatedDate = now;
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = now;
                }
            }
        }
    }
}
