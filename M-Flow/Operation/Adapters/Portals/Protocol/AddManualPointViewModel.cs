using System;
using System.Windows.Input;
using MFlow.API.MVVM;
using MFlow.Data;

namespace MFlow.Operation.Adapters.Portals.Protocol
{
    /// <summary>
    /// View model for the manual point adding.
    /// </summary>
    class AddManualPointViewModel : BindableBase
    {
        #region Fields

        Category[] _Categories;
        Category _Category;
        DateTime _Date;
        string _Name;
        int _WorkingHours;

        #endregion
        
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="AddManualPointViewModel" />
        /// </summary>
        public AddManualPointViewModel()
        {
            CancelCommand = new DelegateCommand(() => CancelAdding?.Invoke());
            OkCommand = new DelegateCommand(
                () => AddManualPoint?.Invoke(Date, Name, Category, WorkingHours), 
                () => Category != null && !string.IsNullOrEmpty(Name) && WorkingHours > 0);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name
        {
            get => _Name;
            set
            {
                if (!SetProperty(ref _Name, value))
                    return;
                
                OkCommand.Invalidate();
            }
        }

        /// <summary>
        /// Gets the categories.
        /// </summary>
        public Category[] Categories
        {
            get => _Categories;
            private set => SetProperty(ref _Categories, value);
        }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        public Category Category
        {
            get => _Category;
            set
            {
                if (!SetProperty(ref _Category, value))
                    return;
                
                OkCommand.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public DateTime Date
        {
            get => _Date;
            set => SetProperty(ref _Date, value);
        }

        /// <summary>
        /// Gets or sets the working hours.
        /// </summary>
        public int WorkingHours
        {
            get => _WorkingHours;
            set
            {
                if (!SetProperty(ref _WorkingHours, value))
                    return;
                
                OkCommand.Invalidate();
            }
        }
        
        /// <summary>
        /// Gets the ok command.
        /// </summary>
        public DelegateCommand OkCommand { get; }

        /// <summary>
        /// Gets the cancel command.
        /// </summary>
        public ICommand CancelCommand { get; }

        #endregion

        #region Events

        /// <summary>
        /// Raised when a manual point should be added.
        /// </summary>
        public event Action<DateTime, string, Category, int> AddManualPoint;

        /// <summary>
        /// Raised when the adding should be canceled.
        /// </summary>
        public event Action CancelAdding;

        #endregion

        #region Methods

        /// <summary>
        /// Updates the categories.
        /// </summary>
        /// <param name="categories">The categories.</param>
        public void Update(Category[] categories)
        {
            Categories = categories;
            Category = categories.Length > 0 ? categories[0] : null;
        }

        /// <summary>
        /// Updates the year and month.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        public void Update(int year, int month)
        {
            Date = new DateTime(year, month, 1);
        }

        #endregion
    }
}