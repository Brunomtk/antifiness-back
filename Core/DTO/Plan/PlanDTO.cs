// File: Core/DTO/PlanDTO.cs
using System;
using System.Collections.Generic;
using Core.Enums;

namespace Core.DTO.Plan
{
    public class PlanDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public PlanType Type { get; set; }
        public int Duration { get; set; }
        public int TargetCalories { get; set; }
        public decimal? TargetWeight { get; set; }
        public PlanStatus Status { get; set; }
        public required int ClientId { get; set; }
        public required int NutritionistId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public IList<PlanGoalDTO> Goals { get; set; } = new List<PlanGoalDTO>();
        public IList<PlanMealDTO> Meals { get; set; } = new List<PlanMealDTO>();
        public IList<PlanProgressDTO> ProgressEntries { get; set; } = new List<PlanProgressDTO>();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
