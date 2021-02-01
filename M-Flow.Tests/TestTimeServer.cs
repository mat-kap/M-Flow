using System;
using MFlow.Data;
using MFlow.Operation.Adapters.Providers;

namespace M_Flow.Tests
{
    /// <summary>
    /// Implementation of a time server which is used for testing.
    /// </summary>
    public class TestTimeServer : ITimeServer
    {
        #region Fields

        readonly DateTime _Today;
        Action<TimeSpan> _OnTick;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="TestTimeServer" />
        /// </summary>
        /// <param name="today">The date of today.</param>
        public TestTimeServer(DateTime today)
        {
            _Today = today;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the time of the active timers.
        /// </summary>
        /// <param name="elapsed"></param>
        public void SetTime(TimeSpan elapsed)
        {
            _OnTick.Invoke(elapsed);
        }

        #endregion
        
        #region Implementation of ITimeServer

        /// <summary>
        /// Gets the current day.
        /// </summary>
        /// <returns>The date of te current day.</returns>
        public DateTime GetDay()
        {
            return _Today;
        }

        /// <summary>
        /// Gets the current year.
        /// </summary>
        /// <returns>The current year.</returns>
        public int GetCurrentYear()
        {
            return _Today.Year;
        }

        /// <summary>
        /// Gets the current month.
        /// </summary>
        /// <returns>The current month.</returns>
        public int GetCurrentMonth()
        {
            return _Today.Month;
        }

        /// <summary>
        /// Starts a new timer.
        /// </summary>
        /// <param name="onTick">The callback which is called on a tick.</param>
        /// <param name="cancelToken">The cancellation token.</param>
        public void StartTimer(Action<TimeSpan> onTick, CancelToken cancelToken)
        {
            _OnTick = onTick;
            _OnTick.Invoke(TimeSpan.Zero);
        }

        #endregion
    }
}