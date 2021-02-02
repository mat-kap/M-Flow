using System;

namespace MFlow.Data
{
    /// <summary>
    /// Holds the work done on a work item.
    /// </summary>
    public class ProtocolEntry
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the time worked on this entry.
        /// </summary>
        public TimeSpan Time { get; set; }
    }
}