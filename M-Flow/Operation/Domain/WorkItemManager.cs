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

        readonly IItemStore<WorkItem> _Store;
        readonly ITimeServer _TimeServer;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="WorkItemManager"/>
        /// </summary>
        /// <param name="store">The work item store.</param>
        /// <param name="timeServer">The time server.</param>
        public WorkItemManager(IItemStore<WorkItem> store, ITimeServer timeServer)
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
            var item = Get(id);
            item.Name = name;
            Save(item);
        }

        /// <summary>
        /// Finishes a working phase on the work item with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the work item.</param>
        public void FinishPhase(Guid id)
        {
            var item = Get(id);
            item.WorkingPhases++;
            Save(item);
        }

        /// <summary>
        /// Finishes the work on the work item with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the work item.</param>
        public void FinishWork(Guid id)
        {
            var item = Get(id);
            item.IsFinished = true;
            Save(item);
        }

        /// <summary>
        /// Creates and saves a new work item.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="categoryId">The identifier of the category.</param>
        public void CreateWorkItem(string name, Guid categoryId)
        {
            var item = new WorkItem
            {
                Id = Guid.NewGuid(),
                Creation = DateTime.Now,
                Name = name,
                CategoryId = categoryId
            };
            
            Save(item);
        }

        /// <summary>
        /// Deletes a work item.
        /// </summary>
        /// <param name="id">The identifier of the work item.</param>
        public void DeleteWorkItem(Guid id)
        {
            var item = Get(id);
            Delete(item);
        }
        
        /// <summary>
        /// Gets the work item with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the work item.</param>
        /// <returns>The work item.</returns>
        public WorkItem Get(Guid id)
        {
            return _Store.Read(id);
        }

        /// <summary>
        /// Gets all work items.
        /// </summary>
        /// <returns>The work items.</returns>
        IEnumerable<WorkItem> GetAll()
        {
            var ids = _Store.GetIds();
            return ids.Select(_Store.Read).OrderBy(o => o.Name);
        }

        /// <summary>
        /// Saves the work item.
        /// </summary>
        /// <param name="item">The work item.</param>
        void Save(WorkItem item)
        {
            _Store.Write(item.Id, item);
        }

        /// <summary>
        /// Deletes the work item.
        /// </summary>
        /// <param name="item">The work item.</param>
        void Delete(WorkItem item)
        {
            _Store.Delete(item.Id);
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
                if (o.Creation.Year > today.Year)
                    return false;

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