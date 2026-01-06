using GPSTrucks.Simulator.Application.Models;

namespace GPSTrucks.Simulator.Application
{
    public class Simulator
    {
        private readonly Queue<(DateTime nextUpdate, Truck truck)> _trucks = [];
        private readonly TimeSpan _UPDATE_INTERVAL = TimeSpan.FromSeconds(5);

        public void Loop()
        {
            if (_trucks.Count == 0) return;

            while (_trucks.Peek().nextUpdate <= DateTime.UtcNow)
            {
                var (nextUpdate, truck) = _trucks.Dequeue();
                (nextUpdate, truck) = AdvanceTruck(nextUpdate, truck);
                _trucks.Enqueue((nextUpdate, truck));
            }
        }

        private (DateTime nextUpdate, Truck truck) AdvanceTruck(DateTime currentUpdateTime, Truck truck)
        {
            var nextUpdate = currentUpdateTime.Add(_UPDATE_INTERVAL);
            var payload = truck.Advance();
            SendTruckPayload(payload);
            return (nextUpdate, truck);
        }

        private void SendTruckPayload(GPSPayload payload)
        {
            // TODO: Implement sending GPS payload to the server or processing it as needed
        }

        public Truck? GetTruck(string id)
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

        public IEnumerable<Truck> GetTrucks()
        {
            return _trucks.Select(t => t.truck);
        }

        public void AddTruck(Truck truck)
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
