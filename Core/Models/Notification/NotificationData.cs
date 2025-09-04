using System.ComponentModel.DataAnnotations;

namespace Core.Models.Notification
{
    public class NotificationData
    {
        public int? EntityId { get; set; }
        
        [MaxLength(100)]
        public string? EntityType { get; set; }
        
        [MaxLength(2000)]
        public string? Metadata { get; set; } // JSON string
        
        [MaxLength(500)]
        public string? ImageUrl { get; set; }
        
        public double? Progress { get; set; }
        
        public double? Value { get; set; }
        
        [MaxLength(50)]
        public string? Unit { get; set; }
    }
}
