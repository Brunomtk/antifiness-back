using System;
using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Core.DTO.Client
{
    public class CreateClientGoalRequest
    {
        [Required(ErrorMessage = "Tipo da meta é obrigatório")]
        public ClientGoalType Type { get; set; }

        [Required(ErrorMessage = "Descrição da meta é obrigatória")]
        [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
        public string Description { get; set; } = string.Empty;

        public double? TargetValue { get; set; }

        public DateTime? TargetDate { get; set; }

        [Required(ErrorMessage = "Prioridade é obrigatória")]
        public Priority Priority { get; set; }

        public ClientStatus? Status { get; set; }
    }
}
