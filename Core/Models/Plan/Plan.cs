// File: Core/Models/Plan.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums;

namespace Core.Models.Plan
{
    [Table("Plans")]
    public class Plan : BaseModel
    {
        [Required]
        public required string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public PlanType Type { get; set; }

        [Required]
        public int Duration { get; set; } // dias

        [Required]
        public int TargetCalories { get; set; }

        public decimal? TargetWeight { get; set; }

        [Required]
        public PlanStatus Status { get; set; }

        // Agora como int
        [Required]
        public int ClientId { get; set; }

        [ForeignKey(nameof(ClientId))]
        public User? Client { get; set; }

        [Required]
        public int NutritionistId { get; set; }

        [ForeignKey(nameof(NutritionistId))]
        public User? Nutritionist { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string? Notes { get; set; }

        public bool IsActive { get; set; }

        public ICollection<PlanGoal>? Goals { get; set; }
        public ICollection<PlanMeal>? Meals { get; set; }
        public ICollection<PlanProgress>? ProgressEntries { get; set; }
    }
}
