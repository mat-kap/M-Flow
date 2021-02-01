using System;
using System.Windows.Input;
using MFlow.API.MVVM;

namespace MFlow.Operation.Adapters.Portals.DayPlanning
{
    class WorkItemViewModel
    {
        public WorkItemViewModel(Guid id, string name, string category, Action onChangeName, Action onRemove)
        {
            Id = id;
            Name = name;
            Category = category;
            
            EditCommand = new DelegateCommand(onChangeName);
            RemoveCommand = new DelegateCommand(onRemove);
        }

        public Guid Id { get; }
        
        public string Name { get; }
        
        public string Category { get; }
        
        public ICommand EditCommand { get; }
        
        public ICommand RemoveCommand { get; }
    }
}