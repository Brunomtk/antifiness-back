using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Diet
{
    public class DietSupplement : BaseModel
    {
        [Required]
        public int DietId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        // Ex: "1 cápsula", "5g", "2 scoops"
        [StringLength(200)]
        public string? Quantity { get; set; }

        // "Como usar"
        [StringLength(1000)]
        public string? Instructions { get; set; }

        [ForeignKey("DietId")]
        public virtual Diet Diet { get; set; } = null!;
    }
}
