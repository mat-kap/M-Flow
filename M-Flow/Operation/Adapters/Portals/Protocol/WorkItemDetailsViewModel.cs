using System;
using System.Linq;
using System.Windows.Input;
using MFlow.API.MVVM;
using MFlow.Data;

namespace MFlow.Operation.Adapters.Portals.Protocol
{
    /// <summary>
    /// View model for the work item details.
    /// </summary>
    class WorkItemDetailsViewModel : BindableBase
    {
        #region Fields

        Tuple<string, string>[] _WorkingDays;
        string _Title;
        
        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="WorkItemDetailsViewModel" />
        /// </summary>
        public WorkItemDetailsViewModel()
        {
            RemoveCommand = new DelegateCommand(() => Delete?.Invoke());
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the title.
        /// </summary>
        public string Title
        {
            get => _Title;
            private set => SetProperty(ref _Title, value);
        }
        
        /// <summary>
        /// Gets the working days.
        /// </summary>
        public Tuple<string, string>[] WorkingDays
        {
            get => _WorkingDays;
            private set => SetProperty(ref _WorkingDays, value);
        }
        
        /// <summary>
        /// Gets the remove command.
        /// </summary>
        public ICommand RemoveCommand { get; }
        
        #endregion

        #region Events

        /// <summary>
        /// Raised if the work item should be deleted.
        /// </summary>
        public event Action Delete;

        #endregion
        
        #region Methods

        /// <summary>
        /// Update the view model using the specified work item details.
        /// </summary>
        /// <param name="details">The work item details.</param>
        public void Update(WorkItemDetails details)
        {
            var totalTime = details.WorkingDays.Aggregate(TimeSpan.Zero, (t, i) => t + i.Time);
            Title = $"{details.Name} ({Formatting.FormatTime(totalTime)})";

            WorkingDays = details.WorkingDays.Select(o =>
            {
                var date = $"{o.Date:dd.MM.yyyy}";
                var time = Formatting.FormatTime(o.Time);
                return Tuple.Create(date, time);
            }).ToArray();
        }

        #endregion
    }
}