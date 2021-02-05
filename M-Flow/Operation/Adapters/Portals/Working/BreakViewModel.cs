using System;
using MFlow.API.MVVM;

namespace MFlow.Operation.Adapters.Portals.Working
{
    /// <summary>
    /// View model for the break phase.
    /// </summary>
    class BreakViewModel : BindableBase
    {
        #region Fields

        string _ElapsedTime;
        double _Progress;
        bool _IsTimeElapsed;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="BreakViewModel" />
        /// </summary>
        public BreakViewModel()
        {
            FinishPointCommand = new DelegateCommand(
                () => FinishPoint?.Invoke(),
                () => _IsTimeElapsed);

            ChangePointCommand = new DelegateCommand(
                () => ChangePoint?.Invoke(),
                () => _IsTimeElapsed);

            NextPhaseCommand = new DelegateCommand(
                () => NextPhase?.Invoke(),
                () => _IsTimeElapsed);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the elapsed time.
        /// </summary>
        public string ElapsedTime
        {
            get => _ElapsedTime;
            private set => SetProperty(ref _ElapsedTime, value);
        }

        /// <summary>
        /// Gets the progress.
        /// </summary>
        public double Progress
        {
            get => _Progress;
            private set => SetProperty(ref _Progress, value);
        }
        
        /// <summary>
        /// Gets the next finish point command. 
        /// </summary>
        public DelegateCommand FinishPointCommand { get; }

        /// <summary>
        /// Gets the next change point command. 
        /// </summary>
        public DelegateCommand ChangePointCommand { get; }

        /// <summary>
        /// Gets the next phase command. 
        /// </summary>
        public DelegateCommand NextPhaseCommand { get; }

        #endregion

        #region Events

        /// <summary>
        /// Raised when the point should be finished.
        /// </summary>
        public event Action FinishPoint;

        /// <summary>
        /// Raised when the point should be changed.
        /// </summary>
        public event Action ChangePoint;

        /// <summary>
        /// Raised when the next phase should be started.
        /// </summary>
        public event Action NextPhase;

        #endregion

        #region Methods

        /// <summary>
        /// Updates the view.
        /// </summary>
        /// <param name="elapsedTime">The elapsed time.</param>
        /// <param name="progress">The progress from 0 to 100.</param>
        public void Update(TimeSpan elapsedTime, double progress)
        {
            ElapsedTime = $"{elapsedTime.Minutes:00}:{elapsedTime.Seconds:00}";
            Progress = progress;
        }

        /// <summary>
        /// Sets the time elapsed state.
        /// </summary>
        public void SetTimeElapsed()
        {
            _IsTimeElapsed = true;
            
            FinishPointCommand.Invalidate();
            ChangePointCommand.Invalidate();
            NextPhaseCommand.Invalidate();
        }

        #endregion
    }
}