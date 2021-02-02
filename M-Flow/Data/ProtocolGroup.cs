using System;
using System.Collections.Generic;

namespace MFlow.Data
{
    /// <summary>
    /// Group of a protocol which represents all the working on a category. 
    /// </summary>
    public class ProtocolGroup
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the total time.
        /// </summary>
        public TimeSpan TotalTime { get; set; }

        /// <summary>
        /// Gets the working points.
        /// </summary>
        public List<ProtocolEntry> WorkingPoints { get; } = new List<ProtocolEntry>();
    }
}