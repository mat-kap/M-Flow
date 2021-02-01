using System;

namespace MFlow.Data
{
    /// <summary>
    /// Holds the data of an entry in the work item details.
    /// </summary>
    public class WorkItemDetailsEntry
    {
        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public DateTime Date { get; set; }
        
        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        public TimeSpan Time { get; set; }
    }
}