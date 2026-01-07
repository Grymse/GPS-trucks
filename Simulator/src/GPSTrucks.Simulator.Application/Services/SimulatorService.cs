using GPSTrucks.Simulator.Application.Models;
using GPSTrucks.Simulator.Core;
using GPSTrucks.Simulator.Core.Entities;

namespace GPSTrucks.Simulator.Application.Services
{
    public class SimulatorService(IGPSDataPublisher dataPublisher, SimulatorSettings settings) : ISimulatorService
    {
        private readonly Queue<(DateTime nextUpdate, ITruck truck)> _trucks = [];
        private readonly TimeSpan _UPDATE_INTERVAL = TimeSpan.FromMilliseconds(settings.TickIntervalMs);

        public void RunTick()
        {
            if (_trucks.Count == 0) return;

            while (_trucks.Peek().nextUpdate <= DateTime.UtcNow)
            {
                var (nextUpdate, truck) = _trucks.Dequeue();
                (nextUpdate, truck) = AdvanceTruck(nextUpdate, truck);
                _trucks.Enqueue((nextUpdate, truck));
            }
        }

        private (DateTime nextUpdate, ITruck truck) AdvanceTruck(DateTime currentUpdateTime, ITruck truck)
        {
            var payload = truck.Advance(currentUpdateTime);
            SendTruckPayload(payload);

            var nextUpdate = currentUpdateTime.Add(_UPDATE_INTERVAL);
            return (nextUpdate, truck);
        }

        private void SendTruckPayload(GPSPayload payload)
        {
            dataPublisher.PublishAsync(payload);
            // TODO: Send status-codes or failed requests to logging system
        }

        public ITruck? GetTruck(string id)
        {
            foreach (var truck in _trucks)
            {
                if (truck.truck.GetId() == id)
                {
                    return truck.truck;
                }
            }
            return null;
        }

        public IEnumerable<ITruck> GetTrucks()
        {
            return _trucks.Select(t => t.truck);
        }

        public void AddTruck(ITruck truck)
        {
            var payload = truck.GetGPSPayload();
            SendTruckPayload(payload);
            _trucks.Enqueue((DateTime.UtcNow.Add(_UPDATE_INTERVAL), truck));
        }

        public void RemoveTruck(string id)
        {
            var truckCount = _trucks.Count;

            for(int i = 0; i < truckCount; i++)
            {
                var truckWithNextTimestamp = _trucks.Dequeue();
                if (truckWithNextTimestamp.truck.GetId() == id) continue;
                
                _trucks.Enqueue(truckWithNextTimestamp);
            }
        }
    }
}
