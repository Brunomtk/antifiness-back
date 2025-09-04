using System.Collections.Generic;
using System.Linq;
using Core.DTO.Message;
using Core.Models.Message;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class MessageTemplateRepository : GenericRepository<MessageTemplate>
    {
        public MessageTemplateRepository(DbContextClass context) : base(context)
        {
        }

        public List<MessageTemplate> GetTemplatesByCategory(Core.Enums.TemplateCategory category, int? empresasId = null)
        {
            var query = _dbContext.MessageTemplates
                .Where(t => t.Category == category);

            if (empresasId.HasValue)
            {
                query = query.Where(t => t.EmpresasId == empresasId.Value || t.IsPublic);
            }

            return query.AsNoTracking().ToList();
        }

        public string RenderTemplate(int templateId, Dictionary<string, string> variables)
        {
            var template = _dbContext.MessageTemplates.FirstOrDefault(t => t.Id == templateId);
            if (template == null) return string.Empty;

            var content = template.Content ?? string.Empty;

            if (variables != null)
            {
                foreach (var kv in variables)
                {
                    content = content.Replace($"{{{kv.Key}}}", kv.Value ?? string.Empty);
                }
            }

            // Atualiza contagem de uso
            template.UsageCount += 1;
            Update(template); // GenericRepository<MessageTemplate>.Update

            return content;
        }
    }
}
