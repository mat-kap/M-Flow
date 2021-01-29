using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MFlow.API.MVVM
{
    /// <summary>
    /// Base of all view bound classes. 
    /// </summary>
    abstract class BindableBase : INotifyPropertyChanged
    {
        #region Events

        /// <summary>
        /// Occurs when a property has been changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

        /// <summary>
        /// Signals the change of the specified property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        internal void SignalChange(string propertyName)
        {
            RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Sets the property with an equality check and raises the property changed event.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="storage">The storage.</param>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>True if the value was stored, false if they value equals the current value.</returns>
        protected virtual bool SetProperty<TValue>(ref TValue storage, TValue value, [CallerMemberName] string propertyName = "")
        {
            if (Equals(storage, value))
                return false;

            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}