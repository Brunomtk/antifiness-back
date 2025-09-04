// File: Core/Models/MacroNutrients.cs
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Core.Models
{
    [Owned]
    public class MacroNutrients
    {
        [Required]
        public int Carbs { get; set; }

        [Required]
        public int Protein { get; set; }

        [Required]
        public int Fat { get; set; }

        public int? Fiber { get; set; }
        public int? Sugar { get; set; }
        public int? Sodium { get; set; }
    }
}
