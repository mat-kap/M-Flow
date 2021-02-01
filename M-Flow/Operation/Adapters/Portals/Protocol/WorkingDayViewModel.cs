namespace MFlow.Operation.Adapters.Portals.Protocol
{
    /// <summary>
    /// View model for a working day.
    /// </summary>
    class WorkingDayViewModel
    {
        /// <summary>
        /// Creates an instance of <see cref="WorkingDayViewModel" />
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="weekdayName">The weekday name.</param>
        /// <param name="totalTime">The total time.</param>
        /// <param name="workingPoints">The working points.</param>
        public WorkingDayViewModel(string date, string weekdayName, string totalTime, WorkingPointViewModel[] workingPoints)
        {
            Date = date;
            WeekdayName = weekdayName;
            TotalTime = totalTime;
            WorkingPoints = workingPoints;
        }
        
        /// <summary>
        /// Gets the date.
        /// </summary>
        public string Date { get; }
        
        /// <summary>
        /// Gets the weekday name.
        /// </summary>
        public string WeekdayName { get; }
        
        /// <summary>
        /// Gets the total time.
        /// </summary>
        public string TotalTime { get; }
        
        /// <summary>
        /// Gets the working points.
        /// </summary>
        public WorkingPointViewModel[] WorkingPoints { get; }
    }
}