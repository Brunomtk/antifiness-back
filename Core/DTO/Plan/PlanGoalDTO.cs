// File: Core/DTO/PlanGoalDTO.cs
using Core.Enums;

namespace Core.DTO.Plan
{
    public class PlanGoalDTO
    {
        public int Id { get; set; }

        // FK para o plano
        public int PlanId { get; set; }

        public GoalType Type { get; set; }
        public double Target { get; set; }
        public double Current { get; set; }
        public required string Unit { get; set; }
        public string? Description { get; set; }
    }
}
