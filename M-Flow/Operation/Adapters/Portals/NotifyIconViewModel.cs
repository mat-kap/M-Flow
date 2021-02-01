using System;
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
            ShowProtocolCommand = new DelegateCommand(() => ShowProtocol?.Invoke());
        }
        
        /// <summary>
        /// Get the exit application command.
        /// </summary>
        public ICommand ExitApplicationCommand { get; }
        
        /// <summary>
        /// Gets the show protocol command.
        /// </summary>
        public ICommand ShowProtocolCommand { get; }

        /// <summary>
        /// Raised when the protocol should be shown.
        /// </summary>
        public event Action ShowProtocol;
    }
}