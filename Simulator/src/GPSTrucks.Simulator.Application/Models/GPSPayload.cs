using System;
using System.Collections.Generic;
using System.Text;

namespace GPSTrucks.Simulator.Application.Models
{
    public record GPSPayload
    {
        public required string TruckId { get; init; }
        public required double SpeedKmH { get; init; }
        public required double FuelLevelPct { get; init; }
        public required double FuelLevelLiters { get; init; }
        public required double EngineTempC { get; init; }

        public required Location Location { get; init; }
        public required DateTime Timestamp { get; init; }
        public required TruckStatus Status { get; init; }
    }
}
