using System;
using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Client
{
    public class AddWeightProgressRequest
    {
        [Required(ErrorMessage = "Data é obrigatória")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Peso é obrigatório")]
        [Range(20, 500, ErrorMessage = "Peso deve estar entre 20 e 500 kg")]
        public double Weight { get; set; }

        [StringLength(1000, ErrorMessage = "Observações devem ter no máximo 1000 caracteres")]
        public string? Notes { get; set; }
    }
}
