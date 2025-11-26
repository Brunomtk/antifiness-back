// File: Core/Models/PlanMeal.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums;

namespace Core.Models.Plan
{
    [Table("PlanMeals")]
    public class PlanMeal : BaseModel
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public MealType? Type { get; set; }

        public TimeSpan? Time { get; set; }

        [Required]
        public int Calories { get; set; }

        [Required]
        public required MacroNutrients Macros { get; set; }

        public string? Instructions { get; set; }

        public bool IsCompleted { get; set; }

        // FK para Plan (ambos são int)
        [Required]
        public required int PlanId { get; set; }

        [ForeignKey(nameof(PlanId))]
        public Plan? Plan { get; set; }

        public ICollection<PlanFood>? Foods { get; set; }
    }
}
