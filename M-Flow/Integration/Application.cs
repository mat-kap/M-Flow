using System;
using System.IO;
using Hardcodet.Wpf.TaskbarNotification;
using MFlow.Data;
using MFlow.Operation.Adapters.Portals;
using MFlow.Operation.Adapters.Portals.CategoryManagement;
using MFlow.Operation.Adapters.Portals.DayPlanning;
using MFlow.Operation.Adapters.Portals.Protocol;
using MFlow.Operation.Adapters.Portals.Working;
using MFlow.Operation.Adapters.Providers;
using Microsoft.Win32;

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
            var notifyIcon = StartNotifyIcon();
            StartDayPlanning();
            onExit();
            notifyIcon?.Dispose();
        }

        /// <summary>
        /// Starts the notify icon.
        /// </summary>
        /// <returns>The notify icon.</returns>
        TaskbarIcon StartNotifyIcon()
        {
            var notifyIconViewModel = new NotifyIconViewModel();
            notifyIconViewModel.ShowProtocol += OpenWorkingProtocol;
            
            var notifyIcon = (TaskbarIcon)System.Windows.Application.Current.FindResource("NotifyIcon");
            if (notifyIcon != null)
                notifyIcon.DataContext = notifyIconViewModel;

            return notifyIcon;
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

            view.Loaded += (_, _) => ViewLocationManager.Add(view);

            var (name, cancelToken) = _Processor.StartBreak(id,
                (elapsed, progress) => viewModel.Update(elapsed, progress),
                () => view.Dispatcher.BeginInvoke(new Action(viewModel.SetTimeElapsed)));
            
            viewModel.Update(name);

            viewModel.FinishPoint += () =>
            {
                view.Close();
                _Processor.FinishPoint(id);
                StartWork();
            };

            viewModel.ChangePoint += () =>
            {
                view.Close();
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

        /// <summary>
        /// Opens the working protocol.
        /// </summary>
        void OpenWorkingProtocol()
        {
            var viewModel = new WorkingProtocolViewModel();
            var view = new WorkingProtocolView { DataContext = viewModel };

            viewModel.OpenWorkItemDetails += (id, y, m) =>
            {
                OpenWorkItemDetails(id);
                
                var items = _Processor.CreateWorkingProtocol(y, m);
                viewModel.Update(items);
            };
            viewModel.DateChanged += (y, m) =>
            {
                var items = _Processor.CreateWorkingProtocol(y, m);
                viewModel.Update(items);
            };
            viewModel.AddManualPoint += (y, m) =>
            {
                var items = AddManualPoint(y, m);
                viewModel.Update(items);
            };
            viewModel.SaveReport += CreatePerformanceReport;
            
            var (years, year, month, workingDays) = _Processor.OpenWorkingProtocol();
            viewModel.Update(years, year, month);
            viewModel.Update(workingDays);

            view.ShowDialog();
        }

        /// <summary>
        /// Opens the work item details for the specified work item identifier. 
        /// </summary>
        /// <param name="id">The work item identifier.</param>
        void OpenWorkItemDetails(Guid id)
        {
            var viewModel = new WorkItemDetailsViewModel();
            var view = new WorkItemDetailsView {DataContext = viewModel};
            
            viewModel.Delete += () =>
            {
                _Processor.RemoveDayPoint(id);
                view.Close();
            };

            var details = _Processor.OpenWorkItemDetails(id);
            viewModel.Update(details);

            view.ShowDialog();
        }

        /// <summary>
        /// Creates the performance report.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        void CreatePerformanceReport(int year, int month)
        {
            var workingDays = _Processor.CreateWorkingProtocol(year, month);
            var (report, fileName) = Processor.CreatePerformanceReport(workingDays);
            
            var dlg = new SaveFileDialog
            {
                Title = "Leistungsbericht speichern",
                Filter = "Textdatei|*.txt",
                FilterIndex = 0,
                AddExtension = true,
                RestoreDirectory = true,
                FileName = fileName
            };

            if (dlg.ShowDialog() != true)
                return;

            File.WriteAllText(dlg.FileName, report);
        }

        /// <summary>
        /// Adds a manual point.
        /// </summary>
        /// <param name="year">The current year.</param>
        /// <param name="month">The current month.</param>
        /// <returns>The possibly changed working days.</returns>
        WorkingDay[] AddManualPoint(int year, int month)
        {
            var viewModel = new AddManualPointViewModel();
            var view = new AddManualPointView {DataContext = viewModel};
            
            viewModel.CancelAdding += () => view.Close();
            viewModel.AddManualPoint += (date, name, category, workingHours) =>
            {
                _Processor.AddManualPoint(date, name, category, workingHours);
                view.Close();
            };

            var categories = _Processor.GetCategories();
            viewModel.Update(categories);
            viewModel.Update(year, month);

            view.ShowDialog();

            return _Processor.CreateWorkingProtocol(year, month);
        }
        
        #endregion
    }
}