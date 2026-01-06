using GPSTrucks.Simulator.Application.Models;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GPSTrucks.Simulator.Application
{
    public class Truck
    {
        private readonly string _id;
        private readonly double _MAX_SPEED_KM = 120.0f;
        private readonly double _MAX_FUEL_LITERS = 150.0f;
        private readonly double _FUEL_CONSUMPTION_PR_KM = 0.1f;
        private double _speedKmH;
        private double _fuelLiters;
        private double _engineTempC;
        private Location _location;
        private DateTime _lastUpdate = DateTime.UtcNow;
        private Location _target;
        private static readonly Location[] target_locations = [
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
        ];

        public Truck(string id, Location location)
        {
            _id = id;
            _location = location;
        }

        public string GetId()
        {
            return _id;
        }

        public GPSPayload GetGPSPayload()
        {
            var status = TruckStatus.Moving;
            return new GPSPayload
            {
                TruckId = _id,
                SpeedKmH = _speedKmH,
                FuelLevelPct = (_fuelLiters / _MAX_FUEL_LITERS) * 100,
                FuelLevelLiters = _fuelLiters,
                EngineTempC = _engineTempC,
                Location = _location.Clone(),
                Timestamp = _lastUpdate,
                Status = status
            };
        }

        public GPSPayload Advance()
        {
            var now = DateTime.UtcNow;
            var timeDiff = (now - _lastUpdate);

            UpdateFuel(timeDiff);
            UpdateSpeedAndEngineTemp(timeDiff);

            var distanceKm = _speedKmH / 60.0f * timeDiff.TotalMinutes;


            _location = _location.MoveTowards(_target, distanceKm);

            if (_location.Equals(_target))
            {
                _speedKmH = 0.0f;
                _target = FindNewTarget();
            }

            _lastUpdate = now;

            return GetGPSPayload();
        }

        public Location FindNewTarget()
        {
            // Generate a new random location as the target
            Location newTarget;
            do
            {
                var randomIndex = Random.Shared.Next(target_locations.Length);
                newTarget = target_locations[randomIndex];
            } while (newTarget.Equals(_target));

            return newTarget;
        }


        public void UpdateSpeedAndEngineTemp(TimeSpan timeDiff)
        {
            var accelerate = timeDiff.TotalSeconds * 5;
            _speedKmH = Math.Max(_speedKmH + accelerate, _MAX_SPEED_KM) - Random.Shared.NextDouble();
            _engineTempC = 90.0f + (new Random().NextDouble() * 3.0);
        }

        public void UpdateFuel(TimeSpan timeDiff)
        {
            var shouldRefuel = _fuelLiters < 15.0f && new Random().NextDouble() < 0.025;
            if (shouldRefuel)
            {
                _speedKmH = 0.0f;
                _fuelLiters = _MAX_FUEL_LITERS;
            }

            _fuelLiters -= _FUEL_CONSUMPTION_PR_KM * ((_speedKmH / 60.0f) * (double)timeDiff.TotalMinutes);
        }
    }
}
