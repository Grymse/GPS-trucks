using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace GPSTrucks.Simulator.Application.Repositories
{
    public interface IRequestLoggerRepository
    {
        void Log(string truckId, HttpStatusCode statusCode);
    }
}
