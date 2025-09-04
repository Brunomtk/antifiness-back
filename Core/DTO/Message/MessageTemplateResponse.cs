using System;
using System.Collections.Generic;
using Core.Enums;

namespace Core.DTO.Message
{
    public class MessageTemplateResponse
    {
        public int Id { get; set; }
        public int? EmpresasId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public TemplateCategory Category { get; set; }
        public List<string> Variables { get; set; } = new List<string>();
        public bool IsPublic { get; set; }
        public int UsageCount { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
