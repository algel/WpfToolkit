using System;
using System.ComponentModel;

namespace WpfToolkit.Collections
{
    /// <summary>
    /// Represents event data for CollectionChanging event.
    /// </summary>
    public class CollectionChangingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionChangingEventArgs"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        public CollectionChangingEventArgs(CollectionChangeAction action, int index, object item)
        {
            Action = action;
            Index = index;
            Item = item;
        }

        /// <summary>
        /// Set this to true to cancel the changes.
        /// </summary>
        public bool Cancel
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionChangingEventArgs"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        public CollectionChangingEventArgs(CollectionChangeAction action)
        {
            Action = action;
        }

        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        /// <value>The item.</value>
        public object Item
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>The index.</value>
        public int Index
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the collection change action.
        /// </summary>
        /// <value>The action.</value>
        public CollectionChangeAction Action
        {
            get;
            set;
        }
    }
}