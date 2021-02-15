using System;
using System.Collections.Generic;
using System.Linq;
using MFlow.Integration;
using MFlow.Operation.Domain;
using Xunit;

namespace M_Flow.Tests.Protocol
{
    public class WhenAddingManualPoint
    {
        [Fact]
        public void ShouldBePhasesCalculatedRight()
        {
            var start = new DateTime(2021, 1, 1);
            
            var phases = new List<DateTime>();
            var result = WorkItemManager.GenerateWorkingPhases(start, 5, 25.0, 5.0, o => phases.Add(o));
            
            Assert.Equal(10, phases.Count);

            var duration = TimeSpan.FromMinutes(30.0);
            Assert.Equal(start + 1 * duration, phases[0]);
            Assert.Equal(start + 2 * duration, phases[1]);
            Assert.Equal(start + 3 * duration, phases[2]);
            Assert.Equal(start + 4 * duration, phases[3]);
            Assert.Equal(start + 5 * duration, phases[4]);
            Assert.Equal(start + 6 * duration, phases[5]);
            Assert.Equal(start + 7 * duration, phases[6]);
            Assert.Equal(start + 8 * duration, phases[7]);
            Assert.Equal(start + 9 * duration, phases[8]);
            Assert.Equal(start + 10 * duration, phases[9]);
            
            Assert.Equal(start + 10 * duration, result);
        }
        
        [Fact]
        public void ShouldBeAddedRight()
        {
            var eventStore = TestFactory.CreateEventStore();
            var timeServer = TestFactory.CreateTimeServer();
            var category = TestFactory.CreateTestCategory();

            var sut = new Processor(eventStore, timeServer);
            sut.AddManualPoint(new DateTime(2021, 1, 1), "Manual Test Entry", category, 2);

            var items = sut.CreateWorkingProtocol(2021, 1);
            
            var dayItem = items[0];
            var manualPoint = dayItem.WorkingPoints.FirstOrDefault(o => o.Name == "Manual Test Entry");
            
            Assert.NotNull(manualPoint);
            Assert.Equal(category.Name, manualPoint.Category);
            Assert.Equal(TimeSpan.FromHours(2), manualPoint.WorkingTime);
        }
    }
}