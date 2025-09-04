using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Workout
{
    public class WorkoutPlanWorkout : BaseModel
    {
        [Required]
        public int WorkoutPlanId { get; set; }

        [Required]
        public int WorkoutId { get; set; }

        [Required]
        public int DayOfWeek { get; set; } // 0 = Sunday, 1 = Monday, etc.

        [Required]
        public int Week { get; set; } // Week number in the plan

        public int Order { get; set; }

        public bool IsCompleted { get; set; } = false;

        public DateTime? CompletedDate { get; set; }

        // Navigation properties
        [ForeignKey("WorkoutPlanId")]
        public virtual WorkoutPlan WorkoutPlan { get; set; } = null!;

        [ForeignKey("WorkoutId")]
        public virtual Workout Workout { get; set; } = null!;
    }
}
