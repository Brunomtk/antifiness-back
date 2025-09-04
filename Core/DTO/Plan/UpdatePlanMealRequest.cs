// File: Core/DTO/UpdatePlanMealRequest.cs
using System;
using System.Collections.Generic;
using Core.Enums;

namespace Core.DTO.Plan
{
    public class UpdatePlanMealRequest
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public MealType? Type { get; set; }
        public TimeSpan? Time { get; set; }
        public int? Calories { get; set; }
        public MacroNutrientsDTO? Macros { get; set; }
        public IList<UpdatePlanFoodRequest>? Foods { get; set; }
        public string? Instructions { get; set; }
        public bool? IsCompleted { get; set; }
    }
}
