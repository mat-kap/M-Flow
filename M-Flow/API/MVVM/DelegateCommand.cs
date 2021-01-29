using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MFlow.API.MVVM
{
    /// <summary>
    /// Command for WPF command binding which can be setuped through delegates.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        #region Fields

        readonly Func<object, bool> _CanExecuteCallback;
        readonly Func<object, Task> _ExecuteCallback;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// </summary>
        /// <param name="executeDelegate">The execute delegate.</param>
        /// <param name="canExecuteDelegate">The can execute delegate.</param>
        public DelegateCommand(Action executeDelegate, Func<bool> canExecuteDelegate)
            : this(_ => executeDelegate(), _ => canExecuteDelegate())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// </summary>
        /// <param name="executeDelegate">The execute delegate.</param>
        public DelegateCommand(Action executeDelegate)
            : this(executeDelegate, () => true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// </summary>
        /// <param name="executeDelegate">The execute delegate.</param>
        /// <param name="canExecuteDelegate">The can execute delegate.</param>
        DelegateCommand(Func<Task> executeDelegate, Func<bool> canExecuteDelegate)
            : this(_ => executeDelegate(), _ => canExecuteDelegate())
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// </summary>
        /// <param name="executeCallback">The execute callback.</param>
        /// <param name="canExecuteCallback">The can execute callback.</param>
        DelegateCommand(Action<object> executeCallback, Func<object, bool> canExecuteCallback)
        {
            if (executeCallback == null)
                throw new ArgumentNullException(nameof(executeCallback));

            if (canExecuteCallback == null)
                throw new ArgumentNullException(nameof(canExecuteCallback));

            _ExecuteCallback = arg =>
            {
                executeCallback(arg);
                return Task.Delay(0);
            };

            _CanExecuteCallback = canExecuteCallback;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// </summary>
        /// <param name="executeCallback">The execute callback.</param>
        /// <param name="canExecuteCallback">The can execute callback.</param>
        DelegateCommand(Func<object, Task> executeCallback, Func<object, bool> canExecuteCallback)
        {
            if (executeCallback == null)
                throw new ArgumentNullException(nameof(executeCallback));

            if (canExecuteCallback == null)
                throw new ArgumentNullException(nameof(canExecuteCallback));

            _ExecuteCallback = executeCallback;
            _CanExecuteCallback = canExecuteCallback;
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the can execute changed state has been changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        #endregion

        #region Methods

        /// <summary>
        /// Signals a change of the <ref name="CanExecute" /> method.
        /// </summary>
        public void Invalidate()
        {
            RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Determines if the command could be executed with the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>True if it could be executed, false if not.</returns>
        public bool CanExecute(object parameter)
        {
            return _CanExecuteCallback(parameter);
        }

        /// <summary>
        /// Executes the command with the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public async void Execute(object parameter)
        {
            await OnExecute(parameter);
        }

        /// <summary>
        /// Determines if the command could be executed.
        /// </summary>
        /// <returns>True if it could be executed, false if not.</returns>
        public bool CanExecute()
        {
            return CanExecute(null);
        }

        /// <summary>
        /// Creates a delegate command from an asynchronous handler.
        /// </summary>
        /// <param name="executeDelegate">The execute delegate.</param>
        /// <returns>The created delegate command.</returns>
        public static DelegateCommand FromAsyncHandler(Func<Task> executeDelegate)
        {
            return new DelegateCommand(executeDelegate, () => true);
        }

        /// <summary>
        /// Creates a delegate command from an asynchronous handler.
        /// </summary>
        /// <param name="executeDelegate">The execute delegate.</param>
        /// <param name="canExecuteDelegate">The can execute delegate.</param>
        /// <returns>The created delegate command.</returns>
        public static DelegateCommand FromAsyncHandler(Func<Task> executeDelegate, Func<bool> canExecuteDelegate)
        {
            return new DelegateCommand(executeDelegate, canExecuteDelegate);
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <returns>The execution task.</returns>
        async Task OnExecute(object parameter)
        {
            await _ExecuteCallback(parameter);
        }

        /// <summary>
        /// Called when the can execute changed event should be raised.
        /// </summary>
        void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}