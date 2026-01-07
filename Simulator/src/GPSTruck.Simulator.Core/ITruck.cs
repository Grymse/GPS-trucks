using GPSTrucks.Simulator.Core.Entities;

namespace GPSTrucks.Simulator.Core
{
    public interface ITruck
    {
        string GetId();
        GPSPayload GetGPSPayload();
        GPSPayload Advance(DateTime currentTime);
    }
}
