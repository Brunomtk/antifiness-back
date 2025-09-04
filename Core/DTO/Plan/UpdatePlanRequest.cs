// File: Core/DTO/UpdatePlanRequest.cs
using System;
using System.Collections.Generic;
using Core.Enums;

namespace Core.DTO.Plan
{
    public class UpdatePlanRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public PlanType? Type { get; set; }
        public int? Duration { get; set; }
        public int? TargetCalories { get; set; }
        public decimal? TargetWeight { get; set; }
        public PlanStatus? Status { get; set; }

        // Agora como int? para corresponder ao modelo
        public int? ClientId { get; set; }
        public int? NutritionistId { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Notes { get; set; }
        public bool? IsActive { get; set; }

        // DTOs de atualização para os detalhes aninhados
        public IList<UpdatePlanGoalRequest>? Goals { get; set; }
        public IList<UpdatePlanMealRequest>? Meals { get; set; }
        public IList<UpdatePlanProgressRequest>? ProgressEntries { get; set; }
    }
}
