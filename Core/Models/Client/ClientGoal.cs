// File: Core/Models/ClientGoal.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums;

namespace Core.Models.Client
{
    [Table("ClientGoals")]
    public class ClientGoal : BaseModel
    {
        [Required]
        public ClientGoalType Type { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        public double? TargetValue { get; set; }
        public DateTime? TargetDate { get; set; }

        [Required]
        public Priority Priority { get; set; }

        [Required]
        public ClientStatus Status { get; set; }

        [Required]
        public int ClientId { get; set; }

        [ForeignKey(nameof(ClientId))]
        public Client? Client { get; set; }
    }
}
