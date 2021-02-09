using System;
using System.Linq;
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
        /// <param name="store">The event store to use.</param>
        /// <param name="timeServer">The time server to use.</param>
        public Processor(IEventStore store, ITimeServer timeServer)
        {
            _WorkItems = new WorkItemManager(store, timeServer);
            _Categories = new CategoryManager(store);
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
        /// <param name="id">The identifier of the day point.</param>
        /// <param name="onProgress">The callback which is called on a progress.</param>
        /// <param name="onFinished">The callback which is called if the concentration is finished.</param>
        /// <returns>The name and the cancellation token.</returns>
        public Tuple<string, CancelToken> StartBreak(Guid id, Action<TimeSpan, double> onProgress, Action onFinished)
        {
            var cancelToken = WorkTracker.StartPhase(TimeSpan.FromMinutes(BreakPhaseDuration), onProgress, onFinished, _TimeServer);
            
            var workItem = _WorkItems.Get(id);
            var (name, _) = WorkTracker.GetWorkItemDescription(workItem);
            return Tuple.Create(name, cancelToken);
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

        /// <summary>
        /// Opens the working protocol.
        /// </summary>
        /// <returns>The years with data, selected year and month and the working days of the selected month.</returns>
        public Tuple<int[], int, int, WorkingDay[]> OpenWorkingProtocol()
        {
            var year = _TimeServer.GetCurrentYear();
            var month = _TimeServer.GetCurrentMonth();
            var workItems = _WorkItems.GetAll();
            var years = workItems.SelectMany(o => o.WorkingPhases).Select(o => o.Year).Distinct().ToArray();
            var categories = _Categories.GetAll();
            var workingDays = WorkingProtocol.CreateProtocol(year, month, workItems, categories,
                ConcentrationPhaseDuration, BreakPhaseDuration);
            
            return Tuple.Create(years, year, month, workingDays);
        }
        
        /// <summary>
        /// Creates the working protocol.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <returns>The the working days of the selected month.</returns>
        public WorkingDay[] CreateWorkingProtocol(int year, int month)
        {
            var workItems = _WorkItems.GetAll();
            var categories = _Categories.GetAll();
            return WorkingProtocol.CreateProtocol(year, month, workItems, categories, ConcentrationPhaseDuration,
                BreakPhaseDuration);
        }

        /// <summary>
        /// Opens the work item details. 
        /// </summary>
        /// <param name="workItemId">The identifier of the work item.</param>
        /// <returns>The work item details.</returns>
        public WorkItemDetails OpenWorkItemDetails(Guid workItemId)
        {
            var workItem = _WorkItems.Get(workItemId);
            var category = _Categories.Get(workItem.CategoryId);
            return WorkingProtocol.CreateDetails(workItem, category, ConcentrationPhaseDuration, BreakPhaseDuration);
        }

        /// <summary>
        /// Creates a performance report for the specified date.
        /// </summary>
        /// <param name="workingDays">The working days.</param>
        /// <returns>The report and the suggested file name.</returns>
        public static Tuple<string, string> CreatePerformanceReport(WorkingDay[] workingDays)
        {
            var (monthName, year) = PerformanceReport.DetermineDate(workingDays);
            var workingPoints = PerformanceReport.ExtractWorkingPoints(workingDays);
            var groups = PerformanceReport.CreateGroups(workingPoints);
            groups = PerformanceReport.CalculateGroupWorkingTimes(groups);
            var report = PerformanceReport.ComposeReport(monthName, year, groups);
            var suggestedFileName = PerformanceReport.SuggestFileName(monthName, year);
            return Tuple.Create(report, suggestedFileName);
        }
        
        #endregion
        
        #endregion
    }
}