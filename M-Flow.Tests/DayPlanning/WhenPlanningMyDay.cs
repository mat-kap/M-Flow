using System;
using MFlow.Integration;
using Xunit;

namespace M_Flow.Tests.DayPlanning
{
    public class WhenPlanningMyDay
    {
        [Fact]
        public void ShouldStartDayPlanningRight()
        {
            var eventStore = TestFactory.CreateEventStore();
            var timeServer = TestFactory.CreateTimeServer();

            var sut = new Processor(eventStore, timeServer);
            var (dayPoints, categories) = sut.StartDayPlanning();

            var workItems = TestFactory.CreateTestWorkItems();
            var expectedDayPoints = new[] {workItems[0], workItems[2]};
            Assert.Equal(expectedDayPoints, dayPoints);

            var category = TestFactory.CreateTestCategory();
            var expectedCategories = new[] {category};
            Assert.Equal(expectedCategories, categories);
        }

        [Fact]
        public void ShouldBeDayPointAddedRight()
        {
            var eventStore = TestFactory.CreateEventStore();
            var timeServer = TestFactory.CreateTimeServer();
            var category = TestFactory.CreateTestCategory();

            var sut = new Processor(eventStore, timeServer);
            var dayPoints = sut.AddNewDayPoint("Punkt 4", category);
            
            Assert.Equal(3, dayPoints.Length);
            Assert.Contains(dayPoints, o => o.Name == "Punkt 4");
        }

        [Fact]
        public void ShouldBeDayPointRemovedRight()
        {
            var eventStore = TestFactory.CreateEventStore();
            var timeServer = TestFactory.CreateTimeServer();
            var workItems = TestFactory.CreateTestWorkItems();

            var sut = new Processor(eventStore, timeServer);
            var dayPoints = sut.RemoveDayPoint(workItems[0].Id);
            
            var expectedDayPoints = new[] {workItems[2]};
            Assert.Equal(expectedDayPoints, dayPoints);
        }

        [Fact]
        public void ShouldBeDayPointNameChangedRight()
        {
            var eventStore = TestFactory.CreateEventStore();
            var timeServer = TestFactory.CreateTimeServer();
            var workItems = TestFactory.CreateTestWorkItems();

            var sut = new Processor(eventStore, timeServer);
            var dayPoints = sut.ChangeDayPointName(workItems[0].Id, "Punkt 1 (geändert)");

            Assert.Equal(2, dayPoints.Length);
            Assert.Contains(dayPoints, o => o.Name == "Punkt 1 (geändert)");
            Assert.Contains(dayPoints, o => o.Name == "Punkt 3");
        }
    }
}