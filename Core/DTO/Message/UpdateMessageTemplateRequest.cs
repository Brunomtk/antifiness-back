using System.Collections.Generic;
using Core.Enums;

namespace Core.DTO.Message
{
    public class UpdateMessageTemplateRequest
    {
        public string? Name { get; set; }
        public string? Content { get; set; }
        public TemplateCategory? Category { get; set; }
        public List<string>? Variables { get; set; }
        public bool? IsPublic { get; set; }
    }
}
