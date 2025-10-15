using Core.DTO.Nutrition;
using Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Diet
{
    public class CreateDietRequest
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Cliente é obrigatório")]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Empresa é obrigatória")]
        public int EmpresaId { get; set; }

        [Required(ErrorMessage = "Data de início é obrigatória")]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DietStatus Status { get; set; } = DietStatus.Active;

        // Metas nutricionais diárias
        [Range(0, 10000, ErrorMessage = "Calorias devem estar entre 0 e 10000")]
        public double? DailyCalories { get; set; }

        [Range(0, 1000, ErrorMessage = "Proteína deve estar entre 0 e 1000g")]
        public double? DailyProtein { get; set; }

        [Range(0, 1000, ErrorMessage = "Carboidratos devem estar entre 0 e 1000g")]
        public double? DailyCarbs { get; set; }

        [Range(0, 500, ErrorMessage = "Gordura deve estar entre 0 e 500g")]
        public double? DailyFat { get; set; }

        [Range(0, 100, ErrorMessage = "Fibra deve estar entre 0 e 100g")]
        public double? DailyFiber { get; set; }

        [Range(0, 10000, ErrorMessage = "Sódio deve estar entre 0 e 10000mg")]
        public double? DailySodium { get; set; }

        [StringLength(500, ErrorMessage = "Restrições devem ter no máximo 500 caracteres")]
        public string? Restrictions { get; set; }

        [StringLength(1000, ErrorMessage = "Observações devem ter no máximo 1000 caracteres")]
        public string? Notes { get; set; }

        public DietMicronutrientsDTO? Micronutrients { get; set; }
    }
}
