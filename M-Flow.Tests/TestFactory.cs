using System;
using MFlow.Data;
using MFlow.Operation.Adapters.Providers;

namespace M_Flow.Tests
{
    /// <summary>
    /// Factory which creates test instances.
    /// </summary>
    public static class TestFactory
    {
        /// <summary>
        /// The date for today to use in the testing environment.
        /// </summary>
        public static readonly DateTime Today = DateTime.Today;
        
        /// <summary>
        /// Creates the test category.
        /// </summary>
        /// <returns>The test category.</returns>
        public static Category CreateTestCategory()
        {
            return new()
            {
                Id = new Guid("94201CF9-D097-4EF1-BF75-75999D675AA2"), 
                Name = "Projekt 1"
            };
        }

        /// <summary>
        /// Creates the test work items.
        /// </summary>
        /// <returns>The test work items.</returns>
        public static WorkItem[] CreateTestWorkItems()
        {
            var category = CreateTestCategory();
            return new[]
            {
                new WorkItem
                {
                    Id = new Guid("E59F637F-523D-4A01-9641-EF69EE332735"),
                    Name = "Punkt 1",
                    CategoryId = category.Id,
                    Creation = Today - TimeSpan.FromDays(1)
                },
                new WorkItem
                {
                    Id = new Guid("022E3B7C-F606-4490-96EA-415B29F425FA"),
                    Name = "Punkt 2",
                    CategoryId = category.Id,
                    IsFinished = true,
                    Creation = Today - TimeSpan.FromDays(1)
                },
                new WorkItem
                {
                    Id = new Guid("C241338B-8FCF-4240-96DA-BD9C1B18B2D8"),
                    Name = "Punkt 3",
                    CategoryId = category.Id,
                    Creation = Today
                }
            };
        }
        
        /// <summary>
        /// Creates the event store.
        /// </summary>
        /// <returns>The event store.</returns>
        public static IEventStore CreateEventStore()
        {
            var store = new InMemoryEventStore();

            // fill in the test category 
            var testCategory = CreateTestCategory();
            store.Set(new Event
            {
                Type = "CategoryCreated",
                EntityId = testCategory.Id, 
                TimeStamp = Today - TimeSpan.FromDays(1),
                Data = new()
                {
                    { "Name", testCategory.Name }   
                } 
            });

            // fill in the test work items
            var items = CreateTestWorkItems();
            foreach (var item in items)
            {
                store.Set(new Event
                {
                    Type = "DayPointCreated",
                    EntityId = item.Id,
                    TimeStamp = item.Creation,
                    Data = new()
                    {
                        { "Name", item.Name },
                        { "CategoryId", item.CategoryId.ToString() }
                    }
                });

                if (item.IsFinished)
                {
                    store.Set(new Event
                    {
                        Type = "DayPointFinished",
                        EntityId = item.Id,
                        TimeStamp = Today
                    });
                }
            }

            return store;
        }

        /// <summary>
        /// Creates the time server.
        /// </summary>
        /// <returns>The time server.</returns>
        public static TestTimeServer CreateTimeServer()
        {
            return new(Today);
        }
    }
}