using System;

namespace WpfToolset.Events
{
    /// <summary>
    /// Generic arguments class to pass to event handlers that need to receive data.
    /// </summary>
    /// <typeparam name="T">The type of data to pass.</typeparam>
    public class DataEventArgs<T> : EventArgs
    {
        /// <summary>Gets the information related to the event.</summary>
        /// <value>Information related to the event.</value>
        public T Value { get; }

        /// <summary>Initializes the DataEventArgs class.</summary>
        /// <param name="value">Information related to the event.</param>
        public DataEventArgs(T value)
        {
            Value = value;
        }
    }

    public class DataEventArgs : DataEventArgs<object>
    {
        /// <inheritdoc />
        public DataEventArgs(object value) : base(value)
        {
        }
    }
}
