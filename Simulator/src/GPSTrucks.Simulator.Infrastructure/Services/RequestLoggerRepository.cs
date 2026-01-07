using GPSTrucks.Simulator.Application.Repositories;
using GPSTrucks.Simulator.Infrastructure.Contexts;
using GPSTrucks.Simulator.Infrastructure.Models;
using System.Net;

namespace GPSTrucks.Simulator.Infrastructure.Services
{
    public class RequestLoggerRepository(LoggingDbContext dbContext) : IRequestLoggerRepository
    {
        public void Log(string truckId, HttpStatusCode statusCode)
        {
            dbContext.Add(new Log
            {
                TruckName = truckId,
                Timestamp = DateTime.UtcNow,
                StatusCode = statusCode
            });

            dbContext.SaveChanges();
        }
    }
}
