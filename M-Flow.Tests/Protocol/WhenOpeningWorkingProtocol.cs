using System;
using MFlow.Data;
using MFlow.Integration;
using Xunit;

namespace M_Flow.Tests.Protocol
{
    public class WhenOpeningWorkingProtocol
    {
        [Fact]
        public void ShouldBeProtocolOpenedRight()
        {
            var today = new DateTime(2021, 1, 30);
            var timeServer = new TestTimeServer(today);

            var categoryId = new Guid("0E8AC7E2-BD81-4C2D-923E-356E532F3275");
            var eventStore = new InMemoryEventStore();
            eventStore.Set(new Event
            {
                Type = "CategoryCreated",
                EntityId = categoryId, 
                TimeStamp = today - TimeSpan.FromDays(2),
                Data = new()
                {
                    { "Name", "Category 1" }   
                }
            });
            
            var workItemId = new Guid("B7C416F5-CF8B-424C-B4D6-31E5CF4E0943");
            eventStore.Set(new Event
            {
                Type = "DayPointCreated",
                EntityId = workItemId,
                TimeStamp = today - TimeSpan.FromDays(2),
                Data = new()
                {
                    { "Name", "Point 1" },
                    { "CategoryId", categoryId.ToString() }
                }
            });

            eventStore.Set(new Event
            {
                Type = "WorkingPhaseFinished",
                EntityId = workItemId,
                TimeStamp = today - TimeSpan.FromDays(2) + TimeSpan.FromHours(6)
            });
            eventStore.Set(new Event
            {
                Type = "WorkingPhaseFinished",
                EntityId = workItemId,
                TimeStamp = today - TimeSpan.FromDays(2) + TimeSpan.FromHours(7)
            });
            eventStore.Set(new Event
            {
                Type = "WorkingPhaseFinished",
                EntityId = workItemId,
                TimeStamp = today + TimeSpan.FromHours(8)
            });

            var sut = new Processor(eventStore, timeServer);
            var (years, year, month, workingDays) = sut.OpenWorkingProtocol();
            
            Assert.Single(years);
            Assert.Equal(2021, years[0]);
            Assert.Equal(2021, year);
            Assert.Equal(1, month);

            Assert.Equal(21, workingDays.Length);

            for (var day = 1; day <= workingDays.Length; day++)
            {
                var workingDay = workingDays[day - 1];
                if (workingDay.Date.Day == 28)
                {
                    Assert.Equal(TimeSpan.FromMinutes(50.0), workingDay.TotalWorkTime);
                    Assert.Single(workingDay.WorkingPoints);

                    Assert.Equal(workItemId, workingDay.WorkingPoints[0].Id);
                    Assert.Equal("Point 1", workingDay.WorkingPoints[0].Name);
                    Assert.Equal("Category 1", workingDay.WorkingPoints[0].Category);
                    Assert.Equal(TimeSpan.FromMinutes(50.0), workingDay.WorkingPoints[0].WorkingTime);
                }
                else if (workingDay.Date.Day == 30)
                {
                    Assert.Equal(TimeSpan.FromMinutes(25.0), workingDay.TotalWorkTime);
                    Assert.Single(workingDay.WorkingPoints);

                    Assert.Equal(workItemId, workingDay.WorkingPoints[0].Id);
                    Assert.Equal("Point 1", workingDay.WorkingPoints[0].Name);
                    Assert.Equal("Category 1", workingDay.WorkingPoints[0].Category);
                    Assert.Equal(TimeSpan.FromMinutes(25.0), workingDay.WorkingPoints[0].WorkingTime);
                }
                else
                {
                    Assert.Equal(TimeSpan.Zero, workingDay.TotalWorkTime);
                    Assert.Empty(workingDay.WorkingPoints);
                }
            }
        }
    }
}