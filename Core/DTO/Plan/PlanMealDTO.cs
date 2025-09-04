// File: Core/DTO/PlanMealDTO.cs
using System;
using System.Collections.Generic;
using Core.Enums;

namespace Core.DTO.Plan
{
    public class PlanMealDTO
    {
        public int Id { get; set; }

        // FK para o plano pai
        public int PlanId { get; set; }

        public required string Name { get; set; }
        public MealType Type { get; set; }
        public TimeSpan Time { get; set; }
        public int Calories { get; set; }
        public MacroNutrientsDTO Macros { get; set; } = new MacroNutrientsDTO();
        public IList<PlanFoodDTO> Foods { get; set; } = new List<PlanFoodDTO>();
        public string? Instructions { get; set; }
        public bool IsCompleted { get; set; }
    }
}
