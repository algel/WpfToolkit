using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Algel.WpfTools
{
    /// <inheritdoc />
    [PublicAPI]
    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName"></param>
        [NotifyPropertyChangedInvocator]
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Occurs when property value changed by <see cref="SetValue{T}"/> method
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected virtual void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {

        }

        /// <summary>
        /// Sets a new property value if it differs from the current value.
        /// Then calls The OnPropertyChanged and RaisePropertyChanged methods
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value))
                return false;

            var oldValue = field;
            field = value;
            OnPropertyChanged(propertyName, oldValue, value);
            RaisePropertyChanged(propertyName);
            return true;
        }
    }
}
