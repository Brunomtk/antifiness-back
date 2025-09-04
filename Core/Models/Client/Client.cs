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
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }
        public string? Avatar { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; } = string.Empty;

        [Required]
        public int Height { get; set; }

        [Required]
        public double CurrentWeight { get; set; }

        public double? TargetWeight { get; set; }

        [Required]
        public ActivityLevel ActivityLevel { get; set; }

        public ClientPreferences Preferences { get; set; } = new ClientPreferences();

        public ClientStatus Status { get; set; }

        [Required]
        public CrmStage KanbanStage { get; set; } = CrmStage.Lead;

        public string? PlanId { get; set; }
        public int EmpresaId { get; set; }

        public ICollection<ClientGoal>? Goals { get; set; }
        public ICollection<ClientMeasurement>? Measurements { get; set; }

        [NotMapped]
        public IList<string>? MedicalConditions { get; set; }
        [NotMapped]
        public IList<string>? Allergies { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
