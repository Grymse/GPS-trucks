using System;
using System.Collections.Generic;
using System.Text;

namespace GPSTrucks.Simulator.Application.Models
{
    public record SimulatorSettings
    {
        public required int TickIntervalMs { get; init; }
    }
}
