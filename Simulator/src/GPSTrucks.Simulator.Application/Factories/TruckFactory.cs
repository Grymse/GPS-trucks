using GPSTruck.Simulator.Core;
using GPSTrucks.Simulator.Core;
using GPSTrucks.Simulator.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GPSTrucks.Simulator.Application.Factory
{
    internal class DefaultTruckFactory : ITruckFactory
    {
        public ITruck CreateTruck(string id)
        {
            var randomMaxSpeed = 87.0f + Random.Shared.NextSingle() * 15; // Between 87 km/h and 102 km/h

            var randomMaxFuel = 150.0f + Random.Shared.NextSingle() * 150; // Between 150 liters and 200 liters

            var randomFuelConsumption = 0.1f + Random.Shared.NextSingle() * 0.2f; // Between 0.1 liters/km and 0.3 liters/km

            var truckSettings = new TruckSettings()
            {
                MaxSpeedKmH = randomMaxSpeed,
                MaxFuelLiters = randomMaxFuel,
                FuelConsumptionPrKm = randomFuelConsumption,
                AvailableLocations = [
                    new Location( 55.6761f, 12.5683f),
                    new Location(56.1567f,  10.2108f),
                    new Location(55.3959f,  10.3883f),
                    new Location(57.0488f,  9.9177f),
                    new Location(55.4765f,  8.4515f),
                    new Location(56.4607f,  10.0364f),
                    new Location(55.8606f,  9.8503f),
                    new Location(55.4914f,  9.4721f),
                    new Location(55.7113f,  9.5363f),
                    new Location(55.6419f,  12.0878f),
                    new Location(56.1393f,  8.9738f),
                    new Location(56.1697f,  9.5451f),
                    new Location(55.8841f,  12.4991f),
                    new Location(56.0361f,  12.6136f),
                    new Location(55.2327f,  11.7600f),
                    new Location(56.4532f,  9.4020f),
                    new Location(55.5668f,  9.7547f),
                    new Location(55.4571f,  12.1818f),
                    new Location(56.3601f,  8.6160f),
                    new Location(55.6496f,  12.2741f),
                ]
            };

            return new Truck(id, DateTime.UtcNow, truckSettings);
        }
    }
}
