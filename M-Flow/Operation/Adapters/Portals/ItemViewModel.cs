using System;
using System.Windows.Input;
using MFlow.API.MVVM;

namespace MFlow.Operation.Adapters.Portals
{
    class ItemViewModel
    {
        public ItemViewModel(Guid id, string name, Action onChangeName, Action onRemove)
        {
            Id = id;
            Name = name;
            
            EditCommand = new DelegateCommand(onChangeName);
            RemoveCommand = new DelegateCommand(onRemove);
        }

        public Guid Id { get; }
        
        public string Name { get; }
        
        public ICommand EditCommand { get; }
        
        public ICommand RemoveCommand { get; }
    }
}