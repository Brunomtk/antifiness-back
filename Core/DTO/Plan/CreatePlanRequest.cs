// File: Core/DTO/CreatePlanRequest.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Core.DTO.Plan
{
    public class CreatePlanRequest
    {
        [Required]
        public required string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public PlanType Type { get; set; }

        [Required]
        public int Duration { get; set; }

        [Required]
        public int TargetCalories { get; set; }

        public decimal? TargetWeight { get; set; }

        [Required]
        public int ClientId { get; set; }

        [Required]
        public int NutritionistId { get; set; }

        public DateTime? StartDate { get; set; }

        public IList<CreatePlanGoalRequest> Goals { get; set; } = new List<CreatePlanGoalRequest>();

        public IList<CreatePlanMealRequest> Meals { get; set; } = new List<CreatePlanMealRequest>();

        public IList<CreatePlanProgressRequest> ProgressEntries { get; set; } = new List<CreatePlanProgressRequest>();

        public string? Notes { get; set; }
    }
}
