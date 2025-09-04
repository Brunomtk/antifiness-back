using Core.Enums;

namespace Core.DTO.Empresa
{
    public class EmpresaFiltersDTO
    {
        public string? Name { get; set; }
        public EmpresaStatus? Status { get; set; }
        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "Name";
        public string? SortDirection { get; set; } = "asc";
    }
}
