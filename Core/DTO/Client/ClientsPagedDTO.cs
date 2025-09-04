using System.Collections.Generic;

namespace Core.DTO.Client
{
    public class ClientsPagedDTO
    {
        public IList<ClientResponse> Results { get; set; } = new List<ClientResponse>();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}
