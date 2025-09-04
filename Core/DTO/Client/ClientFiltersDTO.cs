using System;

namespace Core.DTO.Client
{
    public class ClientFiltersDTO
    {
        public string? Status { get; set; }
        public string? GoalType { get; set; }
        public string? ActivityLevel { get; set; }
        public string? PlanId { get; set; }
        public int? EmpresaId { get; set; }
        public string? Search { get; set; }
        public string? KanbanStage { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? OrderBy { get; set; } = "name";
        public string? OrderDirection { get; set; } = "asc";
    }
}
