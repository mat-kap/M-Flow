using System;
using MFlow.Integration;
using MFlow.Operation.Domain;
using Xunit;

namespace M_Flow.Tests.Working
{
    public class WhenWorking
    {
        [Fact]
        public void ShouldStartWorkRight()
        {
            var eventStore = TestFactory.CreateEventStore();
            var timeServer = TestFactory.CreateTimeServer();
            var workItems = TestFactory.CreateTestWorkItems();
            
            var sut = new Processor(eventStore, timeServer);
            var dayPoints = sut.StartWork();

            var expectedDayPoints = new[] {workItems[0], workItems[2]};
            Assert.Equal(expectedDayPoints, dayPoints);
        }

        [Fact]
        public void ShouldStartFullConcentrationRight()
        {
            var eventStore = TestFactory.CreateEventStore();
            var timeServer = TestFactory.CreateTimeServer();
            var workItems = TestFactory.CreateTestWorkItems();

            var sut = new Processor(eventStore, timeServer);

            var isFinished = false;
            var elapsedTime = TimeSpan.Zero;
            var elapsedProgress = 0.0;
            var (name, phase, _) = sut.StartConcentration(workItems[0].Id,
                (time, progress) =>
                {
                    elapsedTime = time;
                    elapsedProgress = progress;
                },
                () =>
                {
                    isFinished = true;
                });
            
            Assert.Equal("Punkt 1", name);
            Assert.Equal("Phase 1", phase);
            
            Assert.Equal(TimeSpan.Zero, elapsedTime);
            Assert.Equal(0.0, elapsedProgress);
            Assert.False(isFinished);
            
            timeServer.SetTime(TimeSpan.FromMinutes(12.5));

            Assert.Equal(TimeSpan.FromMinutes(12.5), elapsedTime);
            Assert.Equal(50.0, elapsedProgress);
            Assert.False(isFinished);

            timeServer.SetTime(TimeSpan.FromMinutes(25.0));

            Assert.Equal(TimeSpan.FromMinutes(25.0), elapsedTime);
            Assert.Equal(100.0, elapsedProgress);
            Assert.True(isFinished);

            var workItemManager = new WorkItemManager(eventStore, timeServer);
            var workItem = workItemManager.Get(workItems[0].Id);
            Assert.Single(workItem.WorkingPhases);
        }
        
        [Fact]
        public void ShouldStartAndCancelConcentrationRight()
        {
            var eventStore = TestFactory.CreateEventStore();
            var timeServer = TestFactory.CreateTimeServer();
            var workItems = TestFactory.CreateTestWorkItems();

            var sut = new Processor(eventStore, timeServer);

            var isFinished = false;
            var elapsedTime = TimeSpan.Zero;
            var elapsedProgress = 0.0;
            var (_, _, cancelToken) = sut.StartConcentration(workItems[0].Id,
                (time, progress) =>
                {
                    elapsedTime = time;
                    elapsedProgress = progress;
                },
                () =>
                {
                    isFinished = true;
                });
            
            timeServer.SetTime(TimeSpan.FromMinutes(12.5));
            cancelToken.Break();
            
            Assert.Equal(TimeSpan.FromMinutes(12.5), elapsedTime);
            Assert.Equal(50.0, elapsedProgress);
            Assert.False(isFinished);

            var workItemManager = new WorkItemManager(eventStore, timeServer);
            var workItem = workItemManager.Get(workItems[0].Id);
            Assert.Empty(workItem.WorkingPhases);
        }
        
        [Fact]
        public void ShouldStartFullBreakRight()
        {
            var eventStore = TestFactory.CreateEventStore();
            var timeServer = TestFactory.CreateTimeServer();
            var workItems = TestFactory.CreateTestWorkItems();

            var sut = new Processor(eventStore, timeServer);

            var isFinished = false;
            var elapsedTime = TimeSpan.Zero;
            var elapsedProgress = 0.0;
            sut.StartBreak(workItems[0].Id,
                (time, progress) =>
                {
                    elapsedTime = time;
                    elapsedProgress = progress;
                },
                () =>
                {
                    isFinished = true;
                });
            
            Assert.Equal(TimeSpan.Zero, elapsedTime);
            Assert.Equal(0.0, elapsedProgress);
            Assert.False(isFinished);
            
            timeServer.SetTime(TimeSpan.FromMinutes(2.5));

            Assert.Equal(TimeSpan.FromMinutes(2.5), elapsedTime);
            Assert.Equal(50.0, elapsedProgress);
            Assert.False(isFinished);

            timeServer.SetTime(TimeSpan.FromMinutes(5.0));

            Assert.Equal(TimeSpan.FromMinutes(5.0), elapsedTime);
            Assert.Equal(100.0, elapsedProgress);
            Assert.True(isFinished);
        }
        
        [Fact]
        public void ShouldStartAndCancelBreakRight()
        {
            var eventStore = TestFactory.CreateEventStore();
            var timeServer = TestFactory.CreateTimeServer();
            var workItems = TestFactory.CreateTestWorkItems();

            var sut = new Processor(eventStore, timeServer);

            var isFinished = false;
            var elapsedTime = TimeSpan.Zero;
            var elapsedProgress = 0.0;
            var (name, cancelToken) = sut.StartBreak(workItems[0].Id,
                (time, progress) =>
                {
                    elapsedTime = time;
                    elapsedProgress = progress;
                },
                () =>
                {
                    isFinished = true;
                });
            
            timeServer.SetTime(TimeSpan.FromMinutes(2.5));
            cancelToken.Break();
            
            Assert.Equal(TimeSpan.FromMinutes(2.5), elapsedTime);
            Assert.Equal(50.0, elapsedProgress);
            Assert.False(isFinished);
        }

        [Fact]
        public void ShouldBeFinishPointRight()
        {
            var eventStore = TestFactory.CreateEventStore();
            var timeServer = TestFactory.CreateTimeServer();
            var workItems = TestFactory.CreateTestWorkItems();

            var sut = new Processor(eventStore, timeServer);
            sut.FinishPoint(workItems[0].Id);

            var workItemManager = new WorkItemManager(eventStore, timeServer);
            var workItem = workItemManager.Get(workItems[0].Id);
            Assert.True(workItem.IsFinished);
        }
    }
}