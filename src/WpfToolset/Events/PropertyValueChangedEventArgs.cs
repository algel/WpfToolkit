using System.ComponentModel;

namespace WpfToolset.Events
{
    public class PropertyValueChangedEventArgs : PropertyChangedEventArgs
    {
        /// <summary>
        /// Old value of the property that changed.
        /// </summary>
        public object OldValue { get; }

        /// <summary>
        /// New value of the property that changed.
        /// </summary>
        public object NewValue { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.PropertyChangedEventArgs" /> class.</summary>
        /// <param name="propertyName">The name of the property that changed. </param>
        /// <param name="oldValue">Old value of the property that changed.</param>
        /// <param name="newValue">New value of the property that changed.</param>
        public PropertyValueChangedEventArgs(string propertyName, object oldValue, object newValue) : base(propertyName)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
