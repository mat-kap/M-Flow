using System;
using System.Windows.Input;
using MFlow.API.MVVM;

namespace MFlow.Operation.Adapters.Portals.Protocol
{
    /// <summary>
    /// View model for a working point.
    /// </summary>
    class WorkingPointViewModel
    {
        /// <summary>
        /// Creates an instance of <see cref="WorkingPointViewModel" />
        /// </summary>
        /// <param name="workItemId">The identifier of the work item.</param>
        /// <param name="name">The name.</param>
        /// <param name="category">The category.</param>
        /// <param name="time">The time.</param>
        /// <param name="onOpenDetails">Called when the details of the work item should be opened.</param>
        public WorkingPointViewModel(Guid workItemId, string name, string category, string time, Action<Guid> onOpenDetails)
        {
            Name = name;
            Category = category;
            Time = time;
            OpenDetailsCommand = new DelegateCommand(() => onOpenDetails(workItemId));
        }
        
        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// Gets the category.
        /// </summary>
        public string Category { get; }
        
        /// <summary>
        /// Gets the time.
        /// </summary>
        public string Time { get; }
        
        /// <summary>
        /// Gets the command used to open the 
        /// </summary>
        public ICommand OpenDetailsCommand { get; }
    }
}