using System;
using MFlow.API.MVVM;

namespace MFlow.Operation.Adapters.Portals.Working
{
    class BreakViewModel : BindableBase
    {
        string _ElapsedTime;
        double _Progress;
        bool _IsTimeElapsed;

        public BreakViewModel()
        {
            FinishPointCommand = new DelegateCommand(
                () => FinishPoint?.Invoke(),
                () => _IsTimeElapsed);

            NextPhaseCommand = new DelegateCommand(
                () => NextPhase?.Invoke(),
                () => _IsTimeElapsed);
        }

        public string ElapsedTime
        {
            get => _ElapsedTime;
            private set => SetProperty(ref _ElapsedTime, value);
        }

        public double Progress
        {
            get => _Progress;
            private set => SetProperty(ref _Progress, value);
        }
        
        public DelegateCommand FinishPointCommand { get; }

        public DelegateCommand NextPhaseCommand { get; }

        public event Action FinishPoint;

        public event Action NextPhase;

        public void Update(TimeSpan elapsedTime, double progress)
        {
            ElapsedTime = $"{elapsedTime.Minutes:00}:{elapsedTime.Seconds:00}";
            Progress = progress;
        }

        public void SetTimeElapsed()
        {
            _IsTimeElapsed = true;
            
            FinishPointCommand.Invalidate();
            NextPhaseCommand.Invalidate();
        }
    }
}