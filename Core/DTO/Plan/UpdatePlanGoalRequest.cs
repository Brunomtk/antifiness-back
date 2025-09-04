// File: Core/DTO/UpdatePlanGoalRequest.cs
using Core.Enums;

namespace Core.DTO.Plan
{
    public class UpdatePlanGoalRequest
    {
        public int? Id { get; set; }
        public GoalType? Type { get; set; }
        public double? Target { get; set; }
        public double? Current { get; set; }
        public string? Unit { get; set; }
        public string? Description { get; set; }
    }
}
