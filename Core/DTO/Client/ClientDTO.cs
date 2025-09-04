using System;
using System.Collections.Generic;
using Core.Enums;

namespace Core.DTO.Client
{
    public class ClientDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Avatar { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = null!;
        public int Height { get; set; }
        public double CurrentWeight { get; set; }
        public double? TargetWeight { get; set; }
        public ActivityLevel ActivityLevel { get; set; }
        public ClientStatus Status { get; set; }
        public CrmStage KanbanStage { get; set; }
        public string? PlanId { get; set; }
        public int EmpresaId { get; set; }
        public IList<ClientGoalDTO> Goals { get; set; } = new List<ClientGoalDTO>();
        public IList<ClientMeasurementDTO> Measurements { get; set; } = new List<ClientMeasurementDTO>();
        public ClientPreferencesDTO Preferences { get; set; } = new ClientPreferencesDTO();
        public IList<string>? MedicalConditions { get; set; }
        public IList<string>? Allergies { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
