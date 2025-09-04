// File: Core/Models/PlanProgress.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Plan
{
    [Table("PlanProgress")]
    public class PlanProgress : BaseModel
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

        // Agora mapeado como text[] no PostgreSQL
        [Column(TypeName = "text[]")]
        public List<string>? Photos { get; set; }

        // FK
        [Required]
        public required int PlanId { get; set; }

        [ForeignKey(nameof(PlanId))]
        public Plan? Plan { get; set; }
    }
}
