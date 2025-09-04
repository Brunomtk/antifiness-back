// File: Core/DTO/CreatePlanGoalRequest.cs
using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Core.DTO.Plan
{
    public class CreatePlanGoalRequest
    {
        [Required]
        public GoalType Type { get; set; }

        [Required]
        public double Target { get; set; }

        [Required]
        public string Unit { get; set; }

        public string? Description { get; set; }
    }
}
