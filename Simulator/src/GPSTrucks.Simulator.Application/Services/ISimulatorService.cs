using GPSTrucks.Simulator.Core;
using GPSTrucks.Simulator.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GPSTrucks.Simulator.Application.Services
{
    public interface ISimulatorService
    {
        public void RunTick();
        public ITruck? GetTruck(string id);
        public IEnumerable<ITruck> GetTrucks();
        public void AddTruck(ITruck truck);
        public void RemoveTruck(string id);
    }
}
