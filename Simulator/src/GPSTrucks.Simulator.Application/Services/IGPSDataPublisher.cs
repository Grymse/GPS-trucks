using GPSTrucks.Simulator.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GPSTrucks.Simulator.Application.Services
{
    public interface IGPSDataPublisher
    {
        public Task PublishAsync(GPSPayload gpsData);
    }
}
