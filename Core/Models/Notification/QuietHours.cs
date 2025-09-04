using System.ComponentModel.DataAnnotations;

namespace Core.Models.Notification
{
    public class QuietHours
    {
        public bool Enabled { get; set; } = false;
        
        [MaxLength(5)]
        public string StartTime { get; set; } = "22:00";
        
        [MaxLength(5)]
        public string EndTime { get; set; } = "08:00";
    }
}
