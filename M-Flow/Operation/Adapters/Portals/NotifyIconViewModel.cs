using System.Windows;
using System.Windows.Input;
using MFlow.API.MVVM;

namespace MFlow.Operation.Adapters.Portals
{
    class NotifyIconViewModel
    {
        public NotifyIconViewModel()
        {
            ExitApplicationCommand = new DelegateCommand(() => Application.Current.Shutdown());
        }
        
        /// <summary>
        /// Get the exit application command.
        /// </summary>
        public ICommand ExitApplicationCommand { get; }
    }
}