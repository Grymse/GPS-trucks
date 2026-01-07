using GPSTrucks.Simulator.Core;
using GPSTrucks.Simulator.Core.Entities;

namespace GPSTruck.Simulator.Core
{
    public class Truck : ITruck
    {
        private readonly string _id;
        private readonly TruckSettings _settings;
        private double _speedKmH;
        private double _fuelLiters;
        private double _engineTempC;
        private Location _location;
        private DateTime _lastUpdate;
        private Location _target;
        
        public Truck(string id, DateTime currentTime, TruckSettings settings)
        {
            _id = id;
            _settings = settings;
            _lastUpdate = currentTime;
            _location = PickRandomLocation();
            _target = PickRandomLocation(_location);
            _fuelLiters = settings.MaxFuelLiters;
            _engineTempC = 20;
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
                FuelLevelPct = _fuelLiters / _settings.MaxFuelLiters * 100,
                FuelLevelLiters = _fuelLiters,
                EngineTempC = _engineTempC,
                Location = _location.Clone(),
                Timestamp = _lastUpdate,
                Status = status
            };
        }

        public GPSPayload Advance(DateTime currentTime)
        {
            var timeDiff = (currentTime - _lastUpdate);

            UpdateSpeedAndEngineTemp(timeDiff);
            UpdateFuel(timeDiff);

            var distanceKm = _speedKmH / 60.0f * timeDiff.TotalMinutes;

            _location = _location.MoveTowards(_target, distanceKm);

            if (_location.Equals(_target))
            {
                _speedKmH = 0.0f;
                _target = PickRandomLocation(_target);
            }

            _lastUpdate = currentTime;

            return GetGPSPayload();
        }

        private Location PickRandomLocation(Location? excludedLocation = null)
        {
            Location newTarget;
            do
            {
                var randomIndex = Random.Shared.Next(_settings.AvailableLocations.Length);
                newTarget = _settings.AvailableLocations[randomIndex];
            } while (newTarget.Equals(excludedLocation));

            return newTarget;
        }

        private void UpdateSpeedAndEngineTemp(TimeSpan timeDiff)
        {
            var accelerate = timeDiff.TotalSeconds * 5;
            _speedKmH = Math.Min(_speedKmH + accelerate, _settings.MaxSpeedKmH) - Random.Shared.NextDouble();
            _engineTempC = 90.0f + (new Random().NextDouble() * 3.0);
        }

        private void UpdateFuel(TimeSpan timeDiff)
        {
            var shouldRefuel = _fuelLiters < 15.0f && new Random().NextDouble() < 0.025;
            if (shouldRefuel)
            {
                _speedKmH = 0.0f;
                _fuelLiters = _settings.MaxFuelLiters;
            }

            _fuelLiters -= _settings.FuelConsumptionPrKm * ((_speedKmH / 60.0f) * (double)timeDiff.TotalMinutes);
        }
    }
}
