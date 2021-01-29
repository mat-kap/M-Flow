using System;
using MFlow.API.MVVM;
using MFlow.Data;

namespace MFlow.Operation.Adapters.Portals.Working
{
    class SelectWorkingPointViewModel : BindableBase
    {
        WorkItem[] _DayPoints;
        WorkItem _DayPoint;
        
        public SelectWorkingPointViewModel()
        {
            StartCommand = new DelegateCommand(
                () => StartConcentrationPhase?.Invoke(DayPoint.Id), 
                () => DayPoint != null);

            ManagePointsCommand = new DelegateCommand(() => ManageDayPoints?.Invoke());
        }
        
        public WorkItem[] DayPoints
        {
            get => _DayPoints;
            private set => SetProperty(ref _DayPoints, value);
        }

        public WorkItem DayPoint
        {
            get => _DayPoint;
            set
            {
                if (!SetProperty(ref _DayPoint, value))
                    return;

                StartCommand.Invalidate();
            }
        }

        public DelegateCommand StartCommand { get; }

        public DelegateCommand ManagePointsCommand { get; }

        public event Action<Guid> StartConcentrationPhase;

        public event Action ManageDayPoints;

        public void Update(WorkItem[] items)
        {
            DayPoints = items;
            DayPoint = items.Length > 0 ? items[0] : null;
        }

    }
}