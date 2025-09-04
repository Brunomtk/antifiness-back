using Core.Enums;

namespace Core.DTO.Diet
{
    public class DietFiltersDTO
    {
        public string? Search { get; set; }
        public DietStatus? Status { get; set; }
        public int? ClientId { get; set; }
        public int? EmpresaId { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
        public bool? HasEndDate { get; set; }
        public double? MinCalories { get; set; }
        public double? MaxCalories { get; set; }
    }
}
