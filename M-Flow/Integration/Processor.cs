﻿using System;
using MFlow.Data;
using MFlow.Operation.Adapters.Providers;
using MFlow.Operation.Domain;

namespace MFlow.Integration
{
    /// <summary>
    /// The integration level which integrates the domain (single interactions) and optionally the providers.
    /// </summary>
    public class Processor
    {
        #region Fields

        const double ConcentrationPhaseDuration = 25.0;
        const double BreakPhaseDuration = 5.0;
        readonly WorkItemManager _WorkItems;
        readonly CategoryManager _Categories;
        readonly ITimeServer _TimeServer;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="Processor" />
        /// </summary>
        /// <param name="workItemStore">The work item store to use.</param>
        /// <param name="categoryStore">The category store to use.</param>
        /// <param name="timeServer">The time server to use.</param>
        public Processor(IItemStore<WorkItem> workItemStore, IItemStore<Category> categoryStore, ITimeServer timeServer)
        {
            _WorkItems = new WorkItemManager(workItemStore, timeServer);
            _Categories = new CategoryManager(categoryStore);
            _TimeServer = timeServer;
        }

        #endregion

        #region Methods

        #region Day Planning

        /// <summary>
        /// Starts the day planning.
        /// </summary>
        /// <returns>The day points and categories.</returns>
        public Tuple<WorkItem[], Category[]> StartDayPlanning()
        {
            var dayPoints = _WorkItems.GetDayPoints();
            var categories = _Categories.GetAll();
            return Tuple.Create(dayPoints, categories);
        }

        /// <summary>
        /// Adds a new day point.
        /// </summary>
        /// <param name="name">The name of the day point.</param>
        /// <param name="category">The associated category.</param>
        /// <returns>The day points.</returns>
        public WorkItem[] AddNewDayPoint(string name, Category category)
        {
            _WorkItems.CreateWorkItem(name, category.Id);
            return _WorkItems.GetDayPoints();
        }

        /// <summary>
        /// Removes the day point.
        /// </summary>
        /// <param name="id">The identifier of the day point.</param>
        /// <returns>The day points.</returns>
        public WorkItem[] RemoveDayPoint(Guid id)
        {
            _WorkItems.DeleteWorkItem(id);
            return _WorkItems.GetDayPoints();
        }

        /// <summary>
        /// Change the name of a day point.
        /// </summary>
        /// <param name="id">The identifier of the day point.</param>
        /// <param name="name">The new name.</param>
        /// <returns>The day points.</returns>
        public WorkItem[] ChangeDayPointName(Guid id, string name)
        {
            _WorkItems.ChangeWorkItemName(id, name);
            return _WorkItems.GetDayPoints();
        }

        #endregion

        #region Working

        /// <summary>
        /// Starts the work on day points.
        /// </summary>
        /// <returns>The day points.</returns>
        public WorkItem[] StartWork()
        {
            return _WorkItems.GetDayPoints();
        }

        /// <summary>
        /// Starts the concentration phase on a day point.
        /// </summary>
        /// <param name="id">The identifier of the day point.</param>
        /// <param name="onProgress">The callback which is called on a progress.</param>
        /// <param name="onFinished">The callback which is called if the concentration is finished.</param>
        /// <returns>The name and the description of the active point and the cancellation token.</returns>
        public Tuple<string, string, CancelToken> StartConcentration(Guid id, Action<TimeSpan, double> onProgress, Action onFinished)
        {
            var cancelToken = WorkTracker.StartPhase(TimeSpan.FromMinutes(ConcentrationPhaseDuration), onProgress, () =>
            {
                _WorkItems.FinishPhase(id);
                onFinished();
            }, _TimeServer);
            
            var workItem = _WorkItems.Get(id);
            var (name, phase) = WorkTracker.GetWorkItemDescription(workItem);
            return Tuple.Create(name, phase, cancelToken);
        }

        /// <summary>
        /// Starts the concentration phase on a day point.
        /// </summary>
        /// <param name="onProgress">The callback which is called on a progress.</param>
        /// <param name="onFinished">The callback which is called if the concentration is finished.</param>
        /// <returns>The cancellation token.</returns>
        public CancelToken StartBreak(Action<TimeSpan, double> onProgress, Action onFinished)
        {
            return WorkTracker.StartPhase(TimeSpan.FromMinutes(BreakPhaseDuration), onProgress, onFinished, _TimeServer);
        }

        /// <summary>
        /// Finishes the day point with the specified identifier. 
        /// </summary>
        /// <param name="id">The identifier of the day point.</param>
        public void FinishPoint(Guid id)
        {
            _WorkItems.FinishWork(id);
        }

        #endregion

        #region Category Management

        /// <summary>
        /// Gets the categories.
        /// </summary>
        /// <returns>The categories.</returns>
        public Category[] GetCategories()
        {
            return _Categories.GetAll();
        }
        
        /// <summary>
        /// Adds a new category with the specified name.
        /// </summary>
        /// <param name="name">The name of teh new category.</param>
        /// <returns>The categories.</returns>
        public Category[] AddCategory(string name)
        {
            _Categories.CreateCategory(name);
            return _Categories.GetAll();
        }

        /// <summary>
        /// Removes the category with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the category.</param>
        /// <returns>The categories.</returns>
        public Category[] RemoveCategory(Guid id)
        {
            _Categories.DeleteCategory(id);
            return _Categories.GetAll();
        }

        /// <summary>
        /// Changes the name of the category with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the category.</param>
        /// <param name="name">The new name.</param>
        /// <returns>The categories.</returns>
        public Category[] ChangeCategoryName(Guid id, string name)
        {
            _Categories.ChangeCategoryName(id, name);
            return _Categories.GetAll();
        }
        
        #endregion
        
        #region Protocoll
        
        #endregion
        
        #endregion
    }
}