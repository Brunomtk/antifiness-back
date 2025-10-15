using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Core.DTO.Client
{
    public class CreateClientRequest
    {
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string? Name { get; set; }

        [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Telefone deve ter um formato válido")]
        public string? Phone { get; set; }

        public string? Avatar { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Gender { get; set; }

        [Range(50, 250, ErrorMessage = "Altura deve estar entre 50 e 250 cm")]
        public int? Height { get; set; }

        [Range(20, 500, ErrorMessage = "Peso atual deve estar entre 20 e 500 kg")]
        public double? CurrentWeight { get; set; }

        [Range(20, 500, ErrorMessage = "Peso alvo deve estar entre 20 e 500 kg")]
        public double? TargetWeight { get; set; }

        public ActivityLevel? ActivityLevel { get; set; }

        public CrmStage? KanbanStage { get; set; }

        public int? EmpresaId { get; set; }

        public IList<CreateClientGoalRequest>? Goals { get; set; }
        public IList<CreateClientMeasurementRequest>? Measurements { get; set; }
        public ClientPreferencesDTO? Preferences { get; set; }
        public IList<string>? MedicalConditions { get; set; }
        public IList<string>? Allergies { get; set; }
    }
}
