using System.Collections.Generic;

namespace Core.DTO.Message
{
    public class RenderTemplateRequest
    {
        public Dictionary<string, string> Variables { get; set; } = new Dictionary<string, string>();
    }
}
