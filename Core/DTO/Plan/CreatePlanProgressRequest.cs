// File: Core/DTO/CreatePlanProgressRequest.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Plan
{
    public class CreatePlanProgressRequest
    {
        [Required]
        public DateTime Date { get; set; }

        public decimal? Weight { get; set; }

        [Required]
        public int Calories { get; set; }

        [Required]
        public int MealsCompleted { get; set; }

        [Required]
        public int TotalMeals { get; set; }

        public string? Notes { get; set; }

        [Required]
        public required IList<string> Photos { get; set; }

        public CreatePlanProgressRequest()
        {
            Photos = new List<string>();
        }
    }
}
