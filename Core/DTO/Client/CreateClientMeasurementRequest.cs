using System;
using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Client
{
    public class CreateClientMeasurementRequest
    {
        [Required(ErrorMessage = "Data da medição é obrigatória")]
        public DateTime Date { get; set; }

        [Range(20, 500, ErrorMessage = "Peso deve estar entre 20 e 500 kg")]
        public double? Weight { get; set; }

        [Range(0, 100, ErrorMessage = "Gordura corporal deve estar entre 0 e 100%")]
        public double? BodyFat { get; set; }

        [Range(0, 200, ErrorMessage = "Massa muscular deve estar entre 0 e 200 kg")]
        public double? MuscleMass { get; set; }

        [Range(30, 200, ErrorMessage = "Cintura deve estar entre 30 e 200 cm")]
        public double? Waist { get; set; }

        [Range(50, 200, ErrorMessage = "Peito deve estar entre 50 e 200 cm")]
        public double? Chest { get; set; }

        [Range(15, 100, ErrorMessage = "Braços devem estar entre 15 e 100 cm")]
        public double? Arms { get; set; }

        [Range(30, 150, ErrorMessage = "Coxas devem estar entre 30 e 150 cm")]
        public double? Thighs { get; set; }

        [StringLength(1000, ErrorMessage = "Observações devem ter no máximo 1000 caracteres")]
        public string? Notes { get; set; }
    }
}
