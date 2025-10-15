using Core.Models;

namespace Core.Models.Nutrition
{
    public class MicronutrientType : BaseModel
    {
        public string Code { get; set; } = string.Empty; // e.g. VITA, CA, FE
        public string Name { get; set; } = string.Empty; // Vitamina A, Cálcio
        public string Unit { get; set; } = "mg";         // mg | µg | IU
        public bool IsFatSolubleVitamin { get; set; }
        public bool IsTraceMineral { get; set; }
    }
}
