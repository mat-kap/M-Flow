using System;
using System.Collections.Generic;

namespace MFlow.Data
{
    /// <summary>
    /// Holds the data of a day.
    /// </summary>
    public class WorkingDay
    {
        /// <summary>
        /// Gets or sets the date of the day.
        /// </summary>
        public DateTime Date { get; set; }
        
        /// <summary>
        /// Gets or sets the total work time at this day.
        /// </summary>
        public TimeSpan TotalWorkTime { get; set; }

        /// <summary>
        /// Gets the working points at this day.
        /// </summary>
        public List<WorkingPoint> WorkingPoints { get; } = new List<WorkingPoint>();
    }
}