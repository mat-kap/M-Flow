using System;
using System.Collections.Generic;
using System.Linq;
using MFlow.Data;
using MFlow.Operation.Adapters.Providers;

namespace MFlow.Operation.Domain
{
    /// <summary>
    /// Manages the work items.
    /// </summary>
    public class WorkItemManager
    {
        #region Fields

        readonly IEventStore _Store;
        readonly ITimeServer _TimeServer;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="WorkItemManager"/>
        /// </summary>
        /// <param name="store">The event store.</param>
        /// <param name="timeServer">The time server.</param>
        public WorkItemManager(IEventStore store, ITimeServer timeServer)
        {
            _Store = store;
            _TimeServer = timeServer;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the day points.
        /// </summary>
        /// <returns>The day points.</returns>
        public WorkItem[] GetDayPoints()
        {
            var allItems = GetAll();
            var unfinishedItems = GetUnfinishedWorkItems(allItems);

            var today = _TimeServer.GetDay();
            return GetWorkItemsNotInFuture(unfinishedItems, today).ToArray();
        }

        /// <summary>
        /// Changes the work item name.
        /// </summary>
        /// <param name="id">The identifier of the work item.</param>
        /// <param name="name">The new name.</param>
        public void ChangeWorkItemName(Guid id, string name)
        {
            _Store.Set(new Event
            {
                Type = "NameChanged",
                EntityId = id,
                TimeStamp = DateTime.Now,
                Data = new()
                {
                    { "Name", name }
                }
            });
        }

        /// <summary>
        /// Finishes a working phase on the work item with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the work item.</param>
        public void FinishPhase(Guid id)
        {
            _Store.Set(new Event
            {
                Type = "WorkingPhaseFinished",
                EntityId = id,
                TimeStamp = DateTime.Now
            });
        }

        /// <summary>
        /// Finishes the work on the work item with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the work item.</param>
        public void FinishWork(Guid id)
        {
            _Store.Set(new Event
            {
                Type = "DayPointFinished",
                EntityId = id,
                TimeStamp = DateTime.Now
            });
        }

        /// <summary>
        /// Creates and saves a new work item.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="categoryId">The identifier of the category.</param>
        public void CreateWorkItem(string name, Guid categoryId)
        {
            _Store.Set(new Event
            {
                Type = "DayPointCreated",
                EntityId = Guid.NewGuid(),
                TimeStamp = DateTime.Now,
                Data = new()
                {
                    { "Name", name },
                    { "CategoryId", categoryId.ToString() }
                }
            });
        }

        /// <summary>
        /// Deletes a work item.
        /// </summary>
        /// <param name="id">The identifier of the work item.</param>
        public void DeleteWorkItem(Guid id)
        {
            _Store.Set(new Event
            {
                Type = "ItemDeleted",
                EntityId = id,
                TimeStamp = DateTime.Now
            });
        }
        
        /// <summary>
        /// Gets the work item with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the work item.</param>
        /// <returns>The work item.</returns>
        public WorkItem Get(Guid id)
        {
            WorkItem item = null;
            
            var events = _Store.Get(id);
            foreach (var @event in events)
            {
                switch (@event.Type)
                {
                    case "DayPointCreated":
                        item ??= new WorkItem
                        {
                            Id = id, 
                            Creation = @event.TimeStamp,
                            Name = @event.Data["Name"],
                            CategoryId = Guid.Parse(@event.Data["CategoryId"])
                        };
                        break;
                    case "DayPointFinished":
                        if (item != null)
                            item.IsFinished = true;
                        break;
                    case "WorkingPhaseFinished":
                        item?.WorkingPhases.Add(@event.TimeStamp);
                        break;
                    case "NameChanged":
                        if (item != null)
                            item.Name = @event.Data["Name"];
                        break;
                    case "ItemDeleted":
                        item = null;
                        break;
                }
            }

            return item;
        }

        /// <summary>
        /// Gets all work items.
        /// </summary>
        /// <returns>The work items.</returns>
        public WorkItem[] GetAll()
        {
            var ids = _Store.GetIds();
            return ids
                .Select(Get)
                .Where(o => o != null)
                .OrderBy(o => o.Name)
                .ToArray();
        }
        
        /// <summary>
        /// Gets the unfinished work items out of the specified items. 
        /// </summary>
        /// <param name="items">The work items to read from.</param>
        /// <returns>The unfinished work items.</returns>
        static IEnumerable<WorkItem> GetUnfinishedWorkItems(IEnumerable<WorkItem> items)
        {
            return items.Where(o => !o.IsFinished);
        }

        /// <summary>
        /// Gets the work items which aren't in the future out of the specified items. 
        /// </summary>
        /// <param name="items">The work items to read from.</param>
        /// <param name="today">The date of today.</param>
        /// <returns>The work items which aren't in the future.</returns>
        static IEnumerable<WorkItem> GetWorkItemsNotInFuture(IEnumerable<WorkItem> items, DateTime today)
        {
            return items.Where(o =>
            {
                if (o.Creation.Year < today.Year)
                    return true;

                if (o.Creation.Year > today.Year)
                    return false;

                if (o.Creation.Month < today.Month)
                    return true;

                if (o.Creation.Month > today.Month)
                    return false;

                if (o.Creation.Day > today.Day)
                    return false;

                return true;
            });
        }

        #endregion
    }
}