using System;

namespace Core.DTO.Diet
{
    public class DietSupplementResponse
    {
        public int Id { get; set; }
        public int DietId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Quantity { get; set; }
        public string? Instructions { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
