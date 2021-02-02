using System;
using System.Linq;
using System.Windows.Input;
using MFlow.API;
using MFlow.API.MVVM;
using MFlow.Data;

namespace MFlow.Operation.Adapters.Portals.Protocol
{
    /// <summary>
    /// View model for the working protocol.
    /// </summary>
    class WorkingProtocolViewModel : BindableBase
    {
        #region Fields

        int[] _Years;
        int _Year;
        string _Month;
        WorkingDayViewModel[] _WorkingDays;
        bool _IsUpdating;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="WorkingProtocolViewModel" />
        /// </summary>
        public WorkingProtocolViewModel()
        {
            SaveReportCommand = new DelegateCommand(() => SaveReport?.Invoke(Year, GetMonthNumber()));
            Months = new[]
            {
                "Januar",
                "Februar",
                "März",
                "April",
                "Mai",
                "Juni",
                "Juli",
                "August",
                "September",
                "Oktober",
                "November",
                "Dezember"
            };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the years.
        /// </summary>
        public int[] Years
        {
            get => _Years;
            private set => SetProperty(ref _Years, value);
        }

        /// <summary>
        /// Gets the months.
        /// </summary>
        public string[] Months { get; }

        /// <summary>
        /// Gets or sets the year.
        /// </summary>
        public int Year
        {
            get => _Year;
            set
            {
                if (!SetProperty(ref _Year, value))
                    return;

                RaiseDateChanged();
            }
        }

        /// <summary>
        /// Gets or sets the month.
        /// </summary>
        public string Month
        {
            get => _Month;
            set
            {
                if (!SetProperty(ref _Month, value))
                    return;

                RaiseDateChanged();
            }
        }

        /// <summary>
        /// Gets the working days.
        /// </summary>
        public WorkingDayViewModel[] WorkingDays
        {
            get => _WorkingDays;
            private set => SetProperty(ref _WorkingDays, value);
        }
        
        /// <summary>
        /// Gets the save report command.
        /// </summary>
        public ICommand SaveReportCommand { get; }

        #endregion

        #region Events

        /// <summary>
        /// Raised if the selected year or month has been changed.    
        /// </summary>
        public event Action<int, int> DateChanged;

        /// <summary>
        /// Raised if the details of a work item should be opened. 
        /// </summary>
        public event Action<Guid, int, int> OpenWorkItemDetails;

        /// <summary>
        /// Raised if a report should be saved.
        /// </summary>
        public event Action<int, int> SaveReport;

        #endregion

        #region Methods

        /// <summary>
        /// Updates the working days.
        /// </summary>
        /// <param name="workingDays">The working days.</param>
        public void Update(WorkingDay[] workingDays)
        {
            WorkingDays = workingDays.Select(item =>
            {
                var date = $"{item.Date:dd.MM.yyyy}";
                var weekdayName = $"{item.Date:dddd}";
                var totalTime = Formatting.FormatTime(item.TotalWorkTime);
                var workingPoints = item.WorkingPoints.Select(o =>
                {
                    var time = Formatting.FormatTime(o.WorkingTime);
                    return new WorkingPointViewModel(o.Id, o.Name, o.Category, time, 
                        id =>
                        {
                            var month = GetMonthNumber();
                            OpenWorkItemDetails?.Invoke(id, Year, month);
                        });
                }).ToArray();

                return new WorkingDayViewModel(date, weekdayName, totalTime, workingPoints);
            }).ToArray();
        }

        /// <summary>
        /// Updates the year and month.
        /// </summary>
        /// <param name="years">The years containing data.</param>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        public void Update(int[] years, int year, int month)
        {
            _IsUpdating = true;
            
            Years = years;
            Year = year;
            Month = Months[month - 1];

            _IsUpdating = false;
        }

        /// <summary>
        /// Gets the currently selected month number.
        /// </summary>
        /// <returns>The month number.</returns>
        int GetMonthNumber()
        {
            return Array.IndexOf(Months, Month) + 1;
        }

        /// <summary>
        /// Raises the date changed event.
        /// </summary>
        void RaiseDateChanged()
        {
            if (_IsUpdating)
                return;
            
            var month = GetMonthNumber();
            DateChanged?.Invoke(_Year, month);
        }
        
        #endregion
    }
}