namespace Core.DTO.Diet
{
    public class DietsPagedDTO
    {
        public List<DietResponse> Diets { get; set; } = new List<DietResponse>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}
