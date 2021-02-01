using System;
using System.Collections.Generic;

namespace MFlow.Data
{
    /// <summary>
    /// Event which can be stored.
    /// </summary>
    public class Event
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// Gets or sets the entity identifier.
        /// </summary>
        public Guid EntityId { get; set; }
        
        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        public DateTime TimeStamp { get; set; }
        
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        public Dictionary<string, string> Data { get; set; }
    }
}