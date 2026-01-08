using GPSTrucks.Simulator.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace GPSTrucks.Simulator.Application.Factory
{
    public interface ITruckFactory
    {
        public ITruck CreateTruck(string id);
    }
}
