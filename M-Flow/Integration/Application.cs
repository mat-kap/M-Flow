using System;
using System.IO;
using Hardcodet.Wpf.TaskbarNotification;
using MFlow.Data;
using MFlow.Operation.Adapters.Portals;
using MFlow.Operation.Adapters.Portals.CategoryManagement;
using MFlow.Operation.Adapters.Portals.DayPlanning;
using MFlow.Operation.Adapters.Portals.Working;
using MFlow.Operation.Adapters.Providers;

namespace MFlow.Integration
{
    /// <summary>
    /// The main integration which integrates the processor and the UI.
    /// </summary>
    public class Application
    {
        #region Fields

        readonly Processor _Processor;

        #endregion

        #region Constructors

        /// <summary>
        /// Create an instance of <see cref="Application" />
        /// </summary>
        public Application()
        {
            var appFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "M-Flow");
            var eventStore = new EventStore(Path.Combine(appFolder, "Events"));
            var timeServer = new TimeServer();
            _Processor = new Processor(eventStore, timeServer);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Runs the application and starts the process.
        /// </summary>
        /// <param name="onExit">Callback which is called on exit.</param>
        public void Run(Action onExit)
        {
            var notifyIconViewModel = new NotifyIconViewModel();
            var notifyIcon = (TaskbarIcon)System.Windows.Application.Current.FindResource("NotifyIcon");
            if (notifyIcon != null)
                notifyIcon.DataContext = notifyIconViewModel;

            StartDayPlanning();
            onExit();
            
            notifyIcon?.Dispose();
        }
        
        /// <summary>
        /// Starts the day planning.
        /// </summary>
        void StartDayPlanning()
        {
            var viewModel = new DayPlanningViewModel();
            var view = new DayPlanningView { DataContext = viewModel };

            view.OnReturnPressed += () =>
            {
                if (viewModel.AddNewItemCommand.CanExecute())
                    viewModel.AddNewItemCommand.Execute(null);
            };

            viewModel.AddDayPoint += (name, category) =>
            {
                var items = _Processor.AddNewDayPoint(name, category);
                viewModel.Update(items);
            };
            
            viewModel.FinishDayPlanning += () =>
            {
                view.Close();
                StartWork(); 
            };

            viewModel.RemoveDayPoint += id =>
            {
                var items = _Processor.RemoveDayPoint(id);
                viewModel.Update(items);
            };

            viewModel.ChangeDayPointName += (id, name) =>
            {
                var items = _Processor.ChangeDayPointName(id, name);
                viewModel.Update(items);
            };

            viewModel.ManageCategories += () =>
            {
                var items = StartCategoryManagement();
                viewModel.Update(items);
            };
            
            var (dayPoints, categories) = _Processor.StartDayPlanning();
            viewModel.Update(categories);
            viewModel.Update(dayPoints);

            view.ShowDialog();
        }

        /// <summary>
        /// Starts the working on day points.
        /// </summary>
        void StartWork()
        {
            var viewModel = new SelectWorkingPointViewModel();
            var view = new SelectWorkingPointView { DataContext = viewModel };
            
            var dayPoints = _Processor.StartWork();
            viewModel.Update(dayPoints);

            viewModel.StartConcentrationPhase += id =>
            {
                view.Close();
                StartConcentration(id);
            };

            viewModel.ManageDayPoints += () =>
            {
                view.Close();
                StartDayPlanning();
            };

            view.ShowDialog();
        }

        /// <summary>
        /// Starts the concentration phase of a day point.
        /// </summary>
        /// <param name="id">The identifier of the day point.</param>
        void StartConcentration(Guid id)
        {
            var viewModel = new ConcentrationViewModel();
            var view = new ConcentrationView { DataContext = viewModel };

            view.Loaded += (_, _) => ViewLocationManager.Add(view); // SetupLocationToRightBottom(view);
            
            var (activeName, activePhase, cancelToken) = _Processor.StartConcentration(id,
                (elapsed, progress) =>
                {
                    viewModel.Update(elapsed, progress);
                },
                () =>
                {
                    view.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        view.Close();
                        StartBreak(id);
                    }));
                });
            
            viewModel.Update(activeName, activePhase);
            
            view.ShowDialog();
            cancelToken.Break();
        }

        /// <summary>
        /// Starts the break phase of a day point.
        /// </summary>
        /// <param name="id">The identifier of the day point.</param>
        void StartBreak(Guid id)
        {
            var viewModel = new BreakViewModel();
            var view = new BreakView { DataContext = viewModel };

            view.Loaded += (_, _) => ViewLocationManager.Add(view); // SetupLocationToRightBottom(view);

            var cancelToken = _Processor.StartBreak(
                (elapsed, progress) => viewModel.Update(elapsed, progress),
                () => view.Dispatcher.BeginInvoke(new Action(viewModel.SetTimeElapsed)));

            viewModel.FinishPoint += () =>
            {
                view.Close();
                _Processor.FinishPoint(id);
                StartWork();
            };

            viewModel.NextPhase += () =>
            {
                view.Close();
                StartConcentration(id);
            };

            view.ShowDialog();
            cancelToken.Break();
        }

        /// <summary>
        /// Starts the category management.
        /// </summary>
        /// <returns>The categories.</returns>
        Category[] StartCategoryManagement()
        {
            var viewModel = new CategoryManagementViewModel();
            var view = new CategoryManagementView { DataContext = viewModel };

            view.OnReturnPressed += () =>
            {
                if (viewModel.AddNewItemCommand.CanExecute())
                    viewModel.AddNewItemCommand.Execute(null);
            };
            
            viewModel.AddCategory += name =>
            {
                var items = _Processor.AddCategory(name);
                viewModel.Update(items);
            };
            
            viewModel.FinishCategoryManagement += () => view.Close();

            viewModel.RemoveCategory += id =>
            {
                var items = _Processor.RemoveCategory(id);
                viewModel.Update(items);
            };

            viewModel.ChangeCategoryName += (id, name) =>
            {
                var items = _Processor.ChangeCategoryName(id, name);
                viewModel.Update(items);
            };
            
            var categories = _Processor.GetCategories();
            viewModel.Update(categories);

            view.ShowDialog();

            return _Processor.GetCategories();
        }
        
        #endregion
    }
}