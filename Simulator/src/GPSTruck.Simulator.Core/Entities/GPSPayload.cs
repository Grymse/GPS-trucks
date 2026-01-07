
namespace GPSTrucks.Simulator.Core.Entities
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

        public static GPSPayload Empty => new GPSPayload
        {
            TruckId = string.Empty,
            SpeedKmH = 0,
            FuelLevelPct = 0,
            FuelLevelLiters = 0,
            EngineTempC = 0,
            Location = Location.Zero,
            Timestamp = DateTime.MinValue,
            Status = TruckStatus.Stopped
        };
    }
}
