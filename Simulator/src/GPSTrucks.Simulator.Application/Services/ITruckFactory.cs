using GPSTrucks.Simulator.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace GPSTrucks.Simulator.Application.Services
{
    public interface ITruckFactory
    {
        public ITruck CreateTruck(string id);
    }
}
