using System;

namespace MFlow.Data
{
    /// <summary>
    /// Holds the details of a work item.
    /// </summary>
    public class WorkItemDetails
    {
        /// <summary>
        /// Gets or sets the work item identifier.
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Gets or sets the work item name.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the work item category.
        /// </summary>
        public string Category { get; set; }
        
        /// <summary>
        /// Gets or sets the working days.
        /// </summary>
        public WorkItemDetailsEntry[] WorkingDays { get; set; }
    }
}