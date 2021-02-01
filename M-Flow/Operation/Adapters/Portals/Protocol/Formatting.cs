using System;

namespace MFlow.Operation.Adapters.Portals.Protocol
{
    /// <summary>
    /// Can format values into string.
    /// </summary>
    public static class Formatting
    {
        /// <summary>
        /// Formats the time span.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>The formatted time.</returns>
        public static string FormatTime(TimeSpan time)
        {
            return time.Hours < 1
                ? $"{time.Minutes:00}min"
                : $"{time.Hours:00}h {time.Minutes:00}min";
        }
    }
}