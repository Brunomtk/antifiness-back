using System.Collections.Generic;

namespace Core.DTO.Nutrition
{
    public class CalcMicrosResultDTO
    {
        public record MicronutrientTotal(int MicronutrientTypeId, string Code, string Name, string Unit, decimal TotalAmount);
        public List<MicronutrientTotal> Totals { get; set; } = new();
    }
}
