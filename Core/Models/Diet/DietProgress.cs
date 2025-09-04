using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Diet
{
    public class DietProgress : BaseModel
    {
        [Required]
        public int DietId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public double? Weight { get; set; }
        public double? CaloriesConsumed { get; set; }
        public int MealsCompleted { get; set; } = 0;
        public int TotalMeals { get; set; } = 0;

        [StringLength(1000)]
        public string? Notes { get; set; }

        public double? EnergyLevel { get; set; } // 1-10
        public double? HungerLevel { get; set; } // 1-10
        public double? SatisfactionLevel { get; set; } // 1-10

        // Relacionamentos
        [ForeignKey("DietId")]
        public virtual Diet Diet { get; set; } = null!;
    }
}
