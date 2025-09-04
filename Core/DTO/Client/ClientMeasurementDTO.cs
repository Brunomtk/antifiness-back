// File: Core/DTO/ClientMeasurementDTO.cs
using System;

namespace Core.DTO.Client
{
    public class ClientMeasurementDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double Weight { get; set; }
        public double? BodyFat { get; set; }
        public double? MuscleMass { get; set; }
        public double? Waist { get; set; }
        public double? Chest { get; set; }
        public double? Arms { get; set; }
        public double? Thighs { get; set; }
        public string? Notes { get; set; }
    }
}
