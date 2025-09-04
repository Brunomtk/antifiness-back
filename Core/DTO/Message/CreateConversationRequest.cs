using System.Collections.Generic;
using Core.Enums;

namespace Core.DTO.Message
{
    public class CreateConversationRequest
    {
        public int? EmpresasId { get; set; }
        public List<int> ParticipantIds { get; set; } = new List<int>();
        public ConversationType Type { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public ConversationSettingsRequest? Settings { get; set; }
    }

    public class ConversationSettingsRequest
    {
        public bool? Notifications { get; set; }
        public bool? SoundEnabled { get; set; }
        public bool? AutoArchive { get; set; }
        public int? RetentionDays { get; set; }
    }
}
