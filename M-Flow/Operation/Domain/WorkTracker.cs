using System;
using System.Threading;
using System.Threading.Tasks;
using MFlow.Data;
using MFlow.Operation.Adapters.Providers;

namespace MFlow.Operation.Domain
{
    /// <summary>
    /// Tracks the work on work items.
    /// </summary>
    public static class WorkTracker
    {
        /// <summary>
        /// Gets the work item description.
        /// </summary>
        /// <param name="item">The work item.</param>
        /// <returns>The description (name and phase).</returns>
        public static Tuple<string, string> GetWorkItemDescription(WorkItem item)
        {
            return Tuple.Create(item.Name, $"Phase {item.WorkingPhases.Count + 1}");
        }
        
        /// <summary>
        /// Starts a phase.
        /// </summary>
        /// <param name="time">The total time of the phase.</param>
        /// <param name="onProgress">Callback which is called on a progress.</param>
        /// <param name="onFinished">Callback which is called when the total time is reached.</param>
        /// <param name="timeServer">The time server to use.</param>
        /// <returns>The cancellation token.</returns>
        public static CancelToken StartPhase(TimeSpan time, Action<TimeSpan, double> onProgress, Action onFinished, ITimeServer timeServer)
        {
            var cancelToken = new CancelToken();

            Task.Run(() => 
            { 
                timeServer.StartTimer(elapsed =>
                {
                    var progress = CalculateProgress(time, elapsed);
                    onProgress(elapsed, progress);

                    if (elapsed < time) 
                        return;
                    
                    onFinished();
                    cancelToken.Break();
                    
                }, cancelToken); 
            });
            
            Thread.Sleep(100);

            return cancelToken;
        }

        /// <summary>
        /// Calculates the progress.
        /// </summary>
        /// <param name="total">The total needed time.</param>
        /// <param name="elapsed">The elapsed time.</param>
        /// <returns>The progress.</returns>
        static double CalculateProgress(TimeSpan total, TimeSpan elapsed)
        {
            return 100.0 * elapsed.TotalSeconds / total.TotalSeconds;
        }
    }
}