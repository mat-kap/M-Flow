using System;
using MFlow.Data;
using MFlow.Integration;
using Xunit;

namespace M_Flow.Tests.Protocol
{
    public class WhenOpeningWorkItemDetails
    {
        [Fact]
        public void ShouldWorkItemDetailsBeCorrect()
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
            var details = sut.OpenWorkItemDetails(workItemId);
            
            Assert.Equal(workItemId, details.Id);
            Assert.Equal("Point 1", details.Name);
            Assert.Equal("Category 1", details.Category);

            Assert.Equal(2, details.WorkingDays.Length);

            Assert.Equal(2021, details.WorkingDays[0].Date.Year);
            Assert.Equal(1, details.WorkingDays[0].Date.Month);
            Assert.Equal(28, details.WorkingDays[0].Date.Day);
            Assert.Equal(TimeSpan.FromMinutes(50.0), details.WorkingDays[0].Time);

            Assert.Equal(2021, details.WorkingDays[1].Date.Year);
            Assert.Equal(1, details.WorkingDays[1].Date.Month);
            Assert.Equal(30, details.WorkingDays[1].Date.Day);
            Assert.Equal(TimeSpan.FromMinutes(25.0), details.WorkingDays[1].Time);
        }
    }
}