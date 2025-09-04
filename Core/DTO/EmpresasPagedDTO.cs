// File: Core/DTO/EmpresasPagedDTO.cs
using System.Collections.Generic;

namespace Core.DTO
{
    public class EmpresasPagedDTO
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int RowCount { get; set; }
        public int PageCount { get; set; }
        public IList<EmpresasDTO> Result { get; set; } = new List<EmpresasDTO>();
    }
}
