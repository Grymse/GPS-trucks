using GPSTrucks.Simulator.Application.Repositories;
using GPSTrucks.Simulator.Core.Entities;
using System.Net.Http.Json;

namespace GPSTrucks.Simulator.Infrastructure.Services
{
    public class GPSDataHTTPPublisherRepository(HttpClient httpClient) : IGPSDataPublisherRepository
    {
        public Task<HttpResponseMessage> PublishAsync(GPSPayload gpsData)
        {   
            return httpClient.PostAsJsonAsync("/gps", gpsData);
        }
    }
}
