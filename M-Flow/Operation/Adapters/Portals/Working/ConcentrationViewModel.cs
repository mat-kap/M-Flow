using System;
using MFlow.API.MVVM;

namespace MFlow.Operation.Adapters.Portals.Working
{
    class ConcentrationViewModel : BindableBase
    {
        string _ElapsedTime;
        string _ActiveName;
        string _ActivePhase;
        double _Progress;
        
        public string ElapsedTime
        {
            get => _ElapsedTime;
            private set => SetProperty(ref _ElapsedTime, value);
        }
        
        public string ActiveName
        {
            get => _ActiveName;
            private set => SetProperty(ref _ActiveName, value);
        }
        
        public string ActivePhase
        {
            get => _ActivePhase;
            private set => SetProperty(ref _ActivePhase, value);
        }
        
        public double Progress
        {
            get => _Progress;
            private set => SetProperty(ref _Progress, value);
        }

        public void Update(TimeSpan elapsedTime, double progress)
        {
            ElapsedTime = $"{elapsedTime.Minutes:00}:{elapsedTime.Seconds:00}";
            Progress = progress;
        }

        public void Update(string activeName, string activePhase)
        {
            ActiveName = activeName;
            ActivePhase = activePhase;
        }
    }
}