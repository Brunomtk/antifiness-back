using System;

namespace Core.DTO.Diet
{
    public class DietSummaryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ClientId { get; set; }
        public int EmpresaId { get; set; }
        public double? DailyCalories { get; set; }
        public double? DailyProtein  { get; set; }
        public double? DailyCarbs    { get; set; }
        public double? DailyFat      { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
