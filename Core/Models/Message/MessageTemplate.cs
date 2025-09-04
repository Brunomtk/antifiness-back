using System.Collections.Generic;
using Core.Enums;

namespace Core.Models.Message
{
    public class MessageTemplate : BaseModel
    {
        public int? EmpresasId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public TemplateCategory Category { get; set; }
        public bool IsPublic { get; set; }
        public int UsageCount { get; set; }
        public int CreatedBy { get; set; }
        
        // Navigation properties
        public Empresas? Empresas { get; set; }
        public User Creator { get; set; } = null!;
        
        // Variables as owned entity (list of strings)
        public TemplateVariables Variables { get; set; } = new TemplateVariables();
    }
}
