// File: Core/Models/ClientMeasurement.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Client
{
    [Table("ClientMeasurements")]
    public class ClientMeasurement : BaseModel
    {
        [Required]
        public DateTime Date { get; set; }

        [Required]
        public double Weight { get; set; }

        public double? BodyFat { get; set; }
        public double? MuscleMass { get; set; }
        public double? Waist { get; set; }
        public double? Chest { get; set; }
        public double? Arms { get; set; }
        public double? Thighs { get; set; }

        public string? Notes { get; set; }

        [Required]
        public int ClientId { get; set; }

        [ForeignKey(nameof(ClientId))]
        public Client? Client { get; set; }
    }
}
