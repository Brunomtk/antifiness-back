using System.Collections.Generic;
using Core.Enums;

namespace Core.DTO.Message
{
    public class CreateMessageTemplateRequest
    {
        public int? EmpresasId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public TemplateCategory Category { get; set; }
        public List<string> Variables { get; set; } = new List<string>();
        public bool IsPublic { get; set; }
    }
}
