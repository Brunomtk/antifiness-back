using Core.Enums;

namespace Core.DTO.Message
{
    public class ConversationFiltersDTO
    {
        public ConversationType? Type { get; set; }
        public bool? IsArchived { get; set; }
        public bool? IsMuted { get; set; }
        public bool? HasUnread { get; set; }
        public int? ParticipantId { get; set; }
        public string? Search { get; set; }
        public int? EmpresasId { get; set; }
    }
}
