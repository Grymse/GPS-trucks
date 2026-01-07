
namespace GPSTrucks.Simulator.Core.Entities
{
    public record TruckSettings
    {
        public required Location[] AvailableLocations { init; get; }
        public required double MaxSpeedKmH { init; get; }
        public required double MaxFuelLiters { init; get; }
        public required double FuelConsumptionPrKm { init; get; }
    }
}
