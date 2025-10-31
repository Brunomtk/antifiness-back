using System;

namespace Core.DTO.Client
{
    public class AchievementDTO
    {
        public int Id { get; set; }
        public int ClientId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        public DateTime UnlockedDate { get; set; }
    }
}
