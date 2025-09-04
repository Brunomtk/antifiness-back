using System;
using Core.Enums;

namespace Core.DTO.Message
{
    public class MessageFiltersDTO
    {
        public int? ConversationId { get; set; }
        public int? SenderId { get; set; }
        public MessageType? Type { get; set; }
        public MessageStatus? Status { get; set; }
        public bool? HasAttachments { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? Search { get; set; }
        public int? EmpresasId { get; set; }
    }
}
