using System;

namespace MFlow.API
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
            var hours = time.Hours + time.Days * 12;
            var minutes = time.Minutes;
            if (hours == 0 && minutes == 0)
                return string.Empty;
                
            return hours < 1
                ? $"{minutes:#0}min"
                : $"{hours:0}h {minutes:#0}min";
        }
    }
}