using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums;

namespace Core.Models.Workout
{
    public class WorkoutPlan : BaseModel
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public WorkoutGoal Goal { get; set; }

        [Required]
        public WorkoutLevel Level { get; set; }

        [Required]
        public WorkoutPlanStatus Status { get; set; } = WorkoutPlanStatus.Draft;

        [Required]
        public int DurationWeeks { get; set; }

        [Required]
        public int WorkoutsPerWeek { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public int ClientId { get; set; }

        [Required]
        public int EmpresaId { get; set; }

        public int TotalWorkouts { get; set; }

        public int CompletedWorkouts { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal CompletionRate { get; set; }

        public string? Notes { get; set; }

        // Navigation properties
        public virtual ICollection<WorkoutPlanWorkout> WorkoutPlanWorkouts { get; set; } = new List<WorkoutPlanWorkout>();
    }
}
