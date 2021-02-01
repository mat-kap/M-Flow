using System;

namespace MFlow.Data
{
    /// <summary>
    /// Holds the data of a working point.
    /// </summary>
    public class WorkingPoint
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
        /// Gets or sets the working time.
        /// </summary>
        public TimeSpan WorkingTime { get; set; }
    }
}