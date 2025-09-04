// File: Core/DTO/ClientGoalDTO.cs
using System;
using Core.Enums;

namespace Core.DTO.Client
{
    public class ClientGoalDTO
    {
        public int Id { get; set; }
        public ClientGoalType Type { get; set; }
        public string Description { get; set; } = null!;
        public double? TargetValue { get; set; }
        public DateTime? TargetDate { get; set; }
        public Priority Priority { get; set; }
        public ClientStatus Status { get; set; }
    }
}
