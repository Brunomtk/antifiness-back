namespace Core.DTO.Empresa
{
    public class EmpresasPagedDTO
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int RowCount { get; set; }
        public int PageCount { get; set; }
        public List<EmpresaResponse> Result { get; set; } = new();
    }
}
