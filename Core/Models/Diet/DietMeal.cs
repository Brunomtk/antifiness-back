using Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Diet
{
    public class DietMeal : BaseModel
    {
        [Required]
        public int DietId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public MealType Type { get; set; }

        public TimeSpan? ScheduledTime { get; set; }

        [StringLength(500)]
        public string? Instructions { get; set; }

        public double? TotalCalories { get; set; }
        public double? TotalProtein { get; set; }
        public double? TotalCarbs { get; set; }
        public double? TotalFat { get; set; }

        public bool IsCompleted { get; set; } = false;
        public DateTime? CompletedAt { get; set; }

        // Relacionamentos
        [ForeignKey("DietId")]
        public virtual Diet Diet { get; set; } = null!;

        public virtual ICollection<DietMealFood> Foods { get; set; } = new List<DietMealFood>();
    }
}
