using GPSTrucks.Simulator.Application.Models;
using GPSTrucks.Simulator.Application.Repositories;
using GPSTrucks.Simulator.Application.Services;
using GPSTrucks.Simulator.Core;
using GPSTrucks.Simulator.Core.Entities;
using NSubstitute;
using Shouldly;
using System.Net;

namespace GPSTrucks.Simulator.Test.Application
{
    public class SimulatorTest
    {
        private IRequestLoggerRepository loggerRepository = Substitute.For<IRequestLoggerRepository>();

        private static ITruck GetTruckMock()
        {
            var mockTruck = Substitute.For<ITruck>();
            mockTruck.GetId().Returns(Guid.NewGuid().ToString());
            mockTruck.GetGPSPayload().Returns(GPSPayload.Empty);
            mockTruck.Advance(Arg.Any<DateTime>()).Returns(GPSPayload.Empty);

            return mockTruck;
        }

        private static IGPSDataPublisherRepository GetGPSDataPublisherMock()
        {
            var mockPublisher = Substitute.For<IGPSDataPublisherRepository>();
            mockPublisher
                .PublishAsync(Arg.Any<GPSPayload>())
                .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));
            return mockPublisher;
        }

        [Fact]
        public async Task Simulator_AdvancesSimulation_InAccordanceWithTime()
        {
            var gpsDataPublisher = GetGPSDataPublisherMock();

            var truckCount = 100;

            var trucks = Enumerable.Range(0, truckCount).Select(_ => GetTruckMock()).ToList();

            var simulator = new SimulatorService(gpsDataPublisher, loggerRepository, new SimulatorSettings { TickIntervalMs = 5 });

            foreach(var truck in trucks)
            {
                simulator.AddTruck(truck);
            }

            var timeToRun = DateTime.UtcNow.AddMilliseconds(202);

            while(DateTime.UtcNow < timeToRun)
            {
                simulator.RunTick();
                await Task.Delay(1);
            }

            var amountOfUpdates = 200 /* ms */ / 5 /* ms per tick */;
            var estimatedAmountOfPublishedData = amountOfUpdates * truckCount;

            gpsDataPublisher.ReceivedCalls().Count().ShouldBeGreaterThan((int)(estimatedAmountOfPublishedData * 0.9));
            gpsDataPublisher.ReceivedCalls().Count().ShouldBeLessThan((int)(estimatedAmountOfPublishedData * 1.1));

            var callCount = new List<int>();

            foreach (var truck in trucks)
            {
                // Check that the Advance method was called approximately 'amountOfUpdates' times
                var calls = truck.ReceivedCalls().Where(c => c.GetMethodInfo().Name == nameof(ITruck.Advance));

                calls.Count().ShouldBeGreaterThan((int)(amountOfUpdates * 0.9));
                calls.Count().ShouldBeLessThan((int)(amountOfUpdates * 1.1));

                callCount.Add(calls.Count());
            }

            var spread = callCount.Max() - callCount.Min();
            spread.ShouldBeLessThanOrEqualTo(1);
        }


        [Fact]
        public async Task Simulator_AddSeeRemove_ChangesInternalDatastructure()
        {
            var gpsDataPublisher = GetGPSDataPublisherMock();

            var t1 = GetTruckMock();
            var t2 = GetTruckMock();
            var t3 = GetTruckMock();

            var simulator = new SimulatorService(gpsDataPublisher, loggerRepository, new SimulatorSettings { TickIntervalMs = 5 });

            simulator.GetTrucks().Count().ShouldBe(0);

            simulator.AddTruck(t1);
            simulator.AddTruck(t2);

            simulator.GetTrucks().Count().ShouldBe(2);
            simulator.GetTruck(t1.GetId()).ShouldBe(t1);
            simulator.GetTruck(t2.GetId()).ShouldBe(t2);

            simulator.RemoveTruck(t1.GetId());

            simulator.GetTrucks().Count().ShouldBe(1);
            simulator.GetTruck(t1.GetId()).ShouldBeNull();
            simulator.GetTruck(t2.GetId()).ShouldBe(t2);

            simulator.AddTruck(t3);

            simulator.GetTrucks().Count().ShouldBe(2);
            simulator.GetTruck(t3.GetId()).ShouldBe(t3);

            simulator.RemoveTruck(t1.GetId());
            simulator.RemoveTruck(t2.GetId());
            simulator.RemoveTruck(t3.GetId());

            simulator.GetTrucks().Count().ShouldBe(0);
        }
    }
}
