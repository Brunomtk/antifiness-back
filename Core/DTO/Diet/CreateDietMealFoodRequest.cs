using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Diet
{
    public class CreateDietMealFoodRequest
    {
        [Required(ErrorMessage = "Alimento é obrigatório")]
        public int FoodId { get; set; }

        [Required(ErrorMessage = "Quantidade é obrigatória")]
        [Range(0.1, 10000, ErrorMessage = "Quantidade deve estar entre 0.1 e 10000")]
        public double Quantity { get; set; }

        [StringLength(50, ErrorMessage = "Unidade deve ter no máximo 50 caracteres")]
        public string Unit { get; set; } = "g";
    }
}
