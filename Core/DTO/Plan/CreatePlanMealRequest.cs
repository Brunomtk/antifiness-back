// File: Core/DTO/CreatePlanMealRequest.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Core.DTO.Plan
{
    public class CreatePlanMealRequest
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public MealType Type { get; set; }

        [Required]
        public TimeSpan Time { get; set; }

        [Required]
        public int Calories { get; set; }

        [Required]
        public required MacroNutrientsDTO Macros { get; set; }

        [Required]
        public required IList<CreatePlanFoodRequest> Foods { get; set; }

        public string? Instructions { get; set; }

        public CreatePlanMealRequest()
        {
            // inicializa lista para evitar null
            Foods = new List<CreatePlanFoodRequest>();
            Macros = new MacroNutrientsDTO();
        }
    }
}
