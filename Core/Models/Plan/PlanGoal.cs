// File: Core/Models/PlanGoal.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums;

namespace Core.Models.Plan
{
    [Table("PlanGoals")]
    public class PlanGoal : BaseModel
    {
        [Required]
        public GoalType Type { get; set; }

        [Required]
        public double Target { get; set; }

        [Required]
        public double Current { get; set; }

        [Required]
        public string Unit { get; set; }

        public string? Description { get; set; }

        // FK para Plan (ambos são int)
        [Required]
        public required int PlanId { get; set; }

        [ForeignKey(nameof(PlanId))]
        public Plan? Plan { get; set; }
    }
}
