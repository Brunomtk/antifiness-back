using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// File: Core/DTO/MacroNutrientsDTO.cs
namespace Core.DTO
{
    public class MacroNutrientsDTO
    {
        public int Carbs { get; set; }
        public int Protein { get; set; }
        public int Fat { get; set; }
        public int? Fiber { get; set; }
        public int? Sugar { get; set; }
        public int? Sodium { get; set; }
    }
}
