namespace MFlow.Data
{
    /// <summary>
    /// Manages the cancellation of a process.
    /// </summary>
    public class CancelToken
    {
        /// <summary>
        /// Gets a flag if the process should be cancelled or not.
        /// </summary>
        public bool IsBreakPending { get; private set; }

        /// <summary>
        /// Set the cancellation state and breaks the process.
        /// </summary>
        public void Break()
        {
            IsBreakPending = true;
        }

        /// <summary>
        /// Resets the cancellation state.
        /// </summary>
        public void Reset()
        {
            IsBreakPending = false;
        }
    }
}