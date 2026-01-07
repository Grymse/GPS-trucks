using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace GPSTrucks.Simulator.Infrastructure.Models
{
    public record Log
    {
        public int Id { get; set; }
        public string TruckName { get; init; }
        public DateTime Timestamp { get; init; }
        public HttpStatusCode StatusCode { get; init; }
    }
}
