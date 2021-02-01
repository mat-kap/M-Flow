using System;
using System.Threading;
using MFlow.Data;

namespace MFlow.Operation.Adapters.Providers
{
    /// <summary>
    /// Implementation of a time server.
    /// </summary>
    public class TimeServer : ITimeServer
    {
        #region Internal Types

        /// <summary>
        /// Definition of a timer instance.
        /// </summary>
        class TimerDefinition
        {
            #region Fields

            readonly DateTime _TimerStart;
            Action<TimeSpan> _OnTick;
            CancelToken _CancelToken;
            Timer _Timer;

            #endregion

            #region Constructors

            /// <summary>
            /// Creates an instance of <see cref="TimerDefinition" />
            /// </summary>
            /// <param name="onTick">The callback which is called on a tick.</param>
            /// <param name="cancelToken">The cancellation token.</param>
            public TimerDefinition(Action<TimeSpan> onTick, CancelToken cancelToken)
            {
                _OnTick = onTick;
                _CancelToken = cancelToken;
                _TimerStart = DateTime.Now;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Binds this definition to a specific timer instance.
            /// </summary>
            /// <param name="timer">The timer instance.</param>
            public void BindToTimer(Timer timer)
            {
                _Timer = timer;
            }
            
            /// <summary>
            /// Executes a tick.
            /// </summary>
            public void Tick()
            {
                if (_OnTick == null || _CancelToken == null)
                    return;
                
                if (!_CancelToken.IsBreakPending)
                {
                    var elapsed = DateTime.Now - _TimerStart;
                    _OnTick(elapsed);
                }
                else if (_Timer != null)
                {
                    _OnTick = null;
                    _CancelToken = null;
                    
                    _Timer.Change(Timeout.Infinite, Timeout.Infinite);
                    _Timer.Dispose();
                    _Timer = null;
                }
            }

            #endregion
        }

        #endregion
        
        #region Implementation of ITimeServer

        /// <summary>
        /// Gets the current day.
        /// </summary>
        /// <returns>The date of te current day.</returns>
        public DateTime GetDay()
        {
            return DateTime.Today;
        }

        /// <summary>
        /// Gets the current year.
        /// </summary>
        /// <returns>The current year.</returns>
        public int GetCurrentYear()
        {
            return DateTime.Today.Year;
        }

        /// <summary>
        /// Gets the current month.
        /// </summary>
        /// <returns>The current month.</returns>
        public int GetCurrentMonth()
        {
            return DateTime.Today.Month;
        }

        /// <summary>
        /// Starts a new timer.
        /// </summary>
        /// <param name="onTick">The callback which is called on a tick.</param>
        /// <param name="cancelToken">The cancellation token.</param>
        public void StartTimer(Action<TimeSpan> onTick, CancelToken cancelToken)
        {
            var definition = new TimerDefinition(onTick, cancelToken);
            var timer = new Timer(_ => definition.Tick(), null, 0, 200);
            definition.BindToTimer(timer);
        }

        #endregion
    }
}