using System;
using Core.Enums;

namespace Core.DTO.Client
{
    public class ClientBasicDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Avatar { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public int Height { get; set; }
        public double? CurrentWeight { get; set; }
        public double? TargetWeight { get; set; }
        public ActivityLevel ActivityLevel { get; set; }
        public ClientStatus Status { get; set; }
        public CrmStage KanbanStage { get; set; }
        public string? PlanId { get; set; }
        public int EmpresaId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
