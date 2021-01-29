using System;
using MFlow.Data;

namespace MFlow.Operation.Adapters.Providers
{
    /// <summary>
    /// Definition of a time server which works on the time.
    /// </summary>
    public interface ITimeServer
    {
        /// <summary>
        /// Gets the current day.
        /// </summary>
        /// <returns>The date of te current day.</returns>
        DateTime GetDay();

        /// <summary>
        /// Starts a new timer.
        /// </summary>
        /// <param name="onTick">The callback which is called on a tick.</param>
        /// <param name="cancelToken">The cancellation token.</param>
        void StartTimer(Action<TimeSpan> onTick, CancelToken cancelToken);
    }
}