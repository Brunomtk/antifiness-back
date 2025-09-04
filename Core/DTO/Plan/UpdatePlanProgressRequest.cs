// File: Core/DTO/UpdatePlanProgressRequest.cs
using System;
using System.Collections.Generic;

namespace Core.DTO.Plan
{
    public class UpdatePlanProgressRequest
    {
        public int? Id { get; set; }
        public DateTime? Date { get; set; }
        public decimal? Weight { get; set; }
        public int? Calories { get; set; }
        public int? MealsCompleted { get; set; }
        public int? TotalMeals { get; set; }
        public string? Notes { get; set; }
        public IList<string>? Photos { get; set; }
    }
}
