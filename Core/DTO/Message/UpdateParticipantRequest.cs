using Core.Enums;

namespace Core.DTO.Message
{
    public class UpdateParticipantRequest
    {
        public ParticipantRole? Role { get; set; }
        public ParticipantPermissionsRequest? Permissions { get; set; }
    }
}
