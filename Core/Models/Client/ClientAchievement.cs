using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Client
{
    [Table("ClientAchievements")]
    public class ClientAchievement : BaseModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Type { get; set; }
        public string? Category { get; set; }
        public DateTime UnlockedDate { get; set; } = DateTime.UtcNow;

        public Client Client { get; set; } = null!;
    }
}
