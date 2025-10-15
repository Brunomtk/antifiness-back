using Core.Enums;
using Core.Models.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Core.Models.Nutrition;

namespace Core.Models.Diet
{
    public class Diet : BaseModel
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public int ClientId { get; set; }

        [Required]
        public int EmpresaId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public DietStatus Status { get; set; } = DietStatus.Active;

        // Metas nutricionais diárias
        public double? DailyCalories { get; set; }
        public double? DailyProtein { get; set; }
        public double? DailyCarbs { get; set; }
        public double? DailyFat { get; set; }
        public double? DailyFiber { get; set; }
        public double? DailySodium { get; set; }

        // Restrições e observações
        [StringLength(500)]
        public string? Restrictions { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        
        // Totais de micronutrientes (por dia) agregados ou informados manualmente
        public MicronutrientProfile? Micros { get; set; }
// Relacionamentos
        [ForeignKey("ClientId")]
        public virtual Client.Client Client { get; set; } = null!;

        [ForeignKey("EmpresaId")]
        public virtual Empresas Empresa { get; set; } = null!;

        public virtual ICollection<DietMeal> Meals { get; set; } = new List<DietMeal>();
        public virtual ICollection<DietProgress> Progress { get; set; } = new List<DietProgress>();
    }
}
