using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums;

namespace Core.Models.Client
{
    [Table("Clients")]
    public class Client : BaseModel
    {
public string? Name { get; set; } = string.Empty;
public string? Email { get; set; } = string.Empty;

        public string? Phone { get; set; }
        public string? Avatar { get; set; }
public DateTime? DateOfBirth { get; set; }
public string? Gender { get; set; } = string.Empty;
public int? Height { get; set; }
public double? CurrentWeight { get; set; }

        public double? TargetWeight { get; set; }
public ActivityLevel ActivityLevel { get; set; }

        public ClientPreferences Preferences { get; set; } = new ClientPreferences();

        public ClientStatus Status { get; set; }
public CrmStage KanbanStage { get; set; } = CrmStage.Lead;

        public string? PlanId { get; set; }
        public int? EmpresaId { get; set; }

        public ICollection<ClientGoal>? Goals { get; set; }
        public ICollection<ClientMeasurement>? Measurements { get; set; }

        public ICollection<ClientAchievement>? Achievements { get; set; }

        [NotMapped]
        public IList<string>? MedicalConditions { get; set; }
        [NotMapped]
        public IList<string>? Allergies { get; set; }
}
}
