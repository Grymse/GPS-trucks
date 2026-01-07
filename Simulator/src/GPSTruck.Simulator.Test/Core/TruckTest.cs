using GPSTruck.Simulator.Core;
using GPSTrucks.Simulator.Core.Entities;
using System.Runtime.Intrinsics.Arm;

namespace GPSTruck.Simulator.Test.Application
{
    public class TruckTest
    {
        private readonly TruckSettings _defaultTruckSettings4Locations = new TruckSettings
        {
            MaxSpeedKmH = 100,
            FuelConsumptionPrKm = 0.1f,
            MaxFuelLiters = 100,
            AvailableLocations =
                [
                    new Location(0, 0),
                    new Location(0, 5),
                    new Location(5, 5),
                    new Location(5, 0),
                ]
        };
        private readonly TruckSettings _defaultTruckSettings2Locations = new TruckSettings
        {
            MaxSpeedKmH = 100,
            FuelConsumptionPrKm = 0.1f,
            MaxFuelLiters = 100,
            AvailableLocations =
                [
                    new Location(0, 0),
                    new Location(0, 5)
                ]
        };

        [Fact]
        public void Truck_Advances_LocationMoves()
        {
            // Arrange
            var truck = new Truck("TestTruck", DateTime.UtcNow, _defaultTruckSettings2Locations);

            var initialPayload = truck.GetGPSPayload();

            // Act
            var payload = truck.Advance(DateTime.UtcNow.AddMinutes(1));

            // Assert
            Assert.NotEqual(initialPayload.Location, payload.Location); 
        }

        [Fact]
        public void Truck_AdvancesManyTime_TruckArrives()
        {
            // Arrange
            var truck = new Truck("TestTruck", DateTime.UtcNow, _defaultTruckSettings2Locations);

            var initialPayload = truck.GetGPSPayload();

            var initialLocation = initialPayload.Location;

            var targetLocation = _defaultTruckSettings2Locations.AvailableLocations.Where(l => !l.Equals(initialPayload.Location)).FirstOrDefault();
            Assert.NotNull(targetLocation);

            var prevDistance = initialLocation.CalculateDistance(targetLocation);

            var time = DateTime.UtcNow;

            // Act (Comes closer)
            for (int i = 0; i < 10; i++)
            {
                time = time.AddMinutes(10);
                var payload = truck.Advance(time);
                var currentDistance = payload.Location.CalculateDistance(targetLocation);

                Assert.True(currentDistance < prevDistance, $"Expected distance {currentDistance} to be less than previous distance {prevDistance} at iteration {i}");

                prevDistance = currentDistance;
            }

            // Act (Arrives)
            var finalPayload = truck.Advance(DateTime.UtcNow.AddHours(1000));

            Assert.Equal(finalPayload.Location, targetLocation);
        }

        [Fact]
        public void Truck_AdvancesManyTime_UsesFuelAndRefuels()
        {
            // Arrange
            var truck = new Truck("TestTruck", DateTime.UtcNow, _defaultTruckSettings2Locations);

            var prevFuel = truck.GetGPSPayload().FuelLevelLiters;

            var time = DateTime.UtcNow;

            // Act & Assert (Spends fuel)
            for (int i = 0; i < 10; i++)
            {
                time = time.AddMinutes(10);
                var payload = truck.Advance(time);
                var fuel = payload.FuelLevelLiters;

                Assert.True(fuel < prevFuel, $"Expected fuel {fuel} to be less than previous fuel {prevFuel} at iteration {i}");

                prevFuel = fuel;
            }

            // Act Drain fuel until refills or out of fuel
            GPSPayload prevPayload;
            do
            {
                time = time.AddMinutes(10);
                prevPayload = truck.Advance(time);
            } while (prevPayload.FuelLevelLiters < prevFuel && 0 < prevFuel);

            // Assert fuel is more than previously
            Assert.True(prevFuel < prevPayload.FuelLevelLiters, $"Expected refuelling such that fuel level exceeds {prevFuel}, but current fuel-level is {prevPayload.FuelLevelLiters}");
        }

        [Fact]
        public void Truck_ChangesTemperature_WhenAdvancing()
        {
            // Arrange
            var truck = new Truck("TestTruck", DateTime.UtcNow, _defaultTruckSettings2Locations);

            var prevTemp = truck.GetGPSPayload().EngineTempC;

            var time = DateTime.UtcNow;

            // Act & Assert (Changes temp)
            for (int i = 0; i < 10; i++)
            {
                time = time.AddMinutes(10);
                var payload = truck.Advance(time);
                var temp = payload.EngineTempC;

                Assert.NotEqual(prevTemp, temp);

                prevTemp = temp;
            }
        }

        [Fact]
        public void Truck_ChoosesNewDestination_WhenArriving()
        {
            // Arrange
            var time = DateTime.UtcNow;
            var seenLocations = new HashSet<Location>();
            var truck = new Truck("TestTruck", time, _defaultTruckSettings4Locations);

            var prevPayload = truck.GetGPSPayload();

            for (var i = 0; i <= 100; i++)
            {
                time = time.AddDays(2);

                var payload = truck.Advance(time);

                if (payload.FuelLevelLiters == 100) continue; // Skip refuelling events

                var newLocation = payload.Location;
                seenLocations.Add(newLocation);
                Assert.NotEqual(newLocation, prevPayload.Location);

                prevPayload = payload;
            }


            Assert.Equal(4, seenLocations.Count);
        }
    }
}
