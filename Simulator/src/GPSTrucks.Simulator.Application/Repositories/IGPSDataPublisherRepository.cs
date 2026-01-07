using GPSTrucks.Simulator.Core.Entities;

namespace GPSTrucks.Simulator.Application.Repositories
{
    public interface IGPSDataPublisherRepository
    {
        public Task<HttpResponseMessage> PublishAsync(GPSPayload gpsData);
    }
}
