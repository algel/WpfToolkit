using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using WpfToolset.Linq;

namespace WpfToolset.Collections
{
    /// <summary>
    /// Represents an <see cref="ObservableCollection{T}"/> that has ability to suspend
    /// change notification events.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    [Serializable]
    public class ObservableCollectionEx<T> : ObservableCollection<T>
    {
        #region Fields

        private bool _notificationsSuspended;
        private int _lastStartingIndex;
        private IList<T> _addedItems;
        private IList<T> _removedItems;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether change to the collection is made when
        /// its notifications are suspended.
        /// </summary>
        /// <value><c>true</c> if this instance is has been changed while notifications are
        /// suspended; otherwise, <c>false</c>.</value>
        protected bool IsDirty { get; set; }

        /// <summary>
        /// Get a value that indicates whether ObservableCollectionEx 
        /// would raise CollectionChanged event with Reset action, when a bulk add/remove operation takes place.
        /// </summary>
        public bool ShouldResetOnResumeNotifications { get; }

        /// <summary>
        /// Gets a value indicating whether change notifications are suspended.
        /// </summary>
        /// <value>
        /// 	<c>True</c> if notifications are suspended, otherwise, <c>false</c>.
        /// </value>
        public bool NotificationsSuspended => _notificationsSuspended;

        /// <summary>
        /// Gets the added items between suspend and resume.
        /// </summary>
        /// <value>The added items.</value>
        protected virtual IList<T> AddedItems => _addedItems ?? (_addedItems = new WeakReferenceList<T>());

        /// <summary>
        /// Gets the removed items between suspend and resume.
        /// </summary>
        /// <value>The removed items.</value>
        protected virtual IList<T> RemovedItems => _removedItems ?? (_removedItems = new WeakReferenceList<T>());

        #endregion

        #region .ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableCollectionEx{T}"/> class.
        /// </summary>
        public ObservableCollectionEx()
        {
            ShouldResetOnResumeNotifications = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableCollectionEx{T}" /> class.
        /// </summary>
        /// <param name="shouldResetOnResumeNotifications">Indicates whether ObservableCollectionEx will raise CollectionChanged 
        /// event with Reset action, when notifications are resumed.</param>
        internal ObservableCollectionEx(bool shouldResetOnResumeNotifications)
        {
            ShouldResetOnResumeNotifications = shouldResetOnResumeNotifications;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableCollectionEx{T}"/> class.
        /// </summary>
        /// <param name="collection">The collection from which the elements are copied.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="collection"/> parameter cannot be null.
        /// </exception>
        public ObservableCollectionEx(IEnumerable<T> collection)
        {
            ShouldResetOnResumeNotifications = true;
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            CopyFrom(collection);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableCollectionEx{T}" /> class.
        /// </summary>
        /// <param name="shouldResetOnResumeNotifications">Indicates whether ObservableCollectionEx will raise CollectionChanged 
        /// event with Reset action, when notifications are resumed.</param>
        ///  /// <param name="collection">The collection from which the elements are copied.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="collection"/> parameter cannot be null.
        /// </exception>
        internal ObservableCollectionEx(IEnumerable<T> collection, bool shouldResetOnResumeNotifications)
            : this(collection)
        {
            ShouldResetOnResumeNotifications = shouldResetOnResumeNotifications;
        }


        #endregion

        /// <summary>
        /// Occurs when collection is changing.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<CollectionChangingEventArgs> CollectionChanging;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public new event PropertyChangedEventHandler PropertyChanged
        {
            add { base.PropertyChanged += value; }
            remove { base.PropertyChanged -= value; }
        }

        #region Methods

        private void CopyFrom(IEnumerable<T> collection)
        {
            if (collection == null || Items == null)
                return;

            using (IEnumerator<T> enumerator = collection.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Items.Add(enumerator.Current);
                }
            }
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="ObservableCollection{T}"/>.
        /// </summary>
        /// <param name="items">The items that will be added.</param>
        /// <exception cref="ArgumentNullException"><paramref name="items"/> is null.</exception>
        public virtual void AddRange(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            SuspendNotifications();

            foreach (T item in items)
            {
                Add(item);
            }

            ResumeNotifications();
        }

        /// <summary>
        /// Inserts the elements of the specified collection at the specified index.
        /// </summary>
        /// <param name="items">The items that will be added.</param>
        /// <param name="index">The start index.</param>
        /// <exception cref="ArgumentNullException"><paramref name="items"/> is null.</exception>
        public virtual void InsertRange(IEnumerable<T> items, int index)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (index < 0 || index > Count)
            {
                throw new ArgumentException("Invalid start index!");
            }

            SuspendNotifications();

            var list = items.Cast<object>().ToList();
            var startIndex = index;

            foreach (T item in list)
            {
                Insert(startIndex, item);
                startIndex++;
            }

            ResumeNotifications();
        }

        /// <summary>
        /// Removes the elements from the specified collection.
        /// </summary>
        /// <param name="items">The items that will be removed.</param>
        /// <exception cref="ArgumentNullException"><paramref name="items"/> is null.</exception>
        public virtual void RemoveRange(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var list = items.Cast<object>().ToList();
            if (list.Count > 0)
            {
                SuspendNotifications();

                foreach (T item in list)
                {
                    Remove(item);
                }

                ResumeNotifications();
            }
        }

        /// <summary>
        /// Raises <see cref="INotifyCollectionChanged.CollectionChanged"/> with 
        /// <see cref="NotifyCollectionChangedAction.Reset"/> changed action.
        /// </summary>
        public void Reset()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <inheritdoc />
        /// <remarks>
        /// Raises the <see cref="INotifyCollectionChanged.CollectionChanged"/> event when
        /// notifications are not suspended.
        /// </remarks>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!NotificationsSuspended)
            {
                base.OnCollectionChanged(e);
            }
            else
            {
                if (!ShouldResetOnResumeNotifications)
                {
                    CacheModifiedItems(e);
                }
                IsDirty = true;
            }
        }

        private void CacheModifiedItems(NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add && e.Action != NotifyCollectionChangedAction.Remove)
            {
                return;
            }

            if (e.OldItems != null)
            {
                RemovedItems.AddRange(e.OldItems.OfType<T>());
                if (_lastStartingIndex != -1)
                {
                    _lastStartingIndex = e.OldStartingIndex;
                }
            }

            if (e.NewItems != null)
            {
                AddedItems.AddRange(e.NewItems.OfType<T>());
                if (_lastStartingIndex != -1)
                {
                    _lastStartingIndex = e.NewStartingIndex;
                }
            }
        }

        /// <inheritdoc />
        /// <remarks>
        /// Raises the <see cref="INotifyPropertyChanged.PropertyChanged"/> event when
        /// notifications are not suspended.
        /// </remarks>
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (!NotificationsSuspended)
            {
                base.OnPropertyChanged(e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:CollectionChanging"/> event.
        /// </summary>
        /// <param name="e">The <see cref="CollectionChangingEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCollectionChanging(CollectionChangingEventArgs e)
        {
            CollectionChanging?.Invoke(this, e);
        }

        /// <inheritdoc />
        protected override void InsertItem(int index, T item)
        {
            var e = new CollectionChangingEventArgs(CollectionChangeAction.Add, index, item);
            OnCollectionChanging(e);

            if (e.Cancel)
                return;

            LogInsert(item);
            base.InsertItem(index, item);
        }

        internal virtual void LogInsert(T item)
        {
        }

        internal virtual void LogRemove(T item)
        {
        }

        /// <inheritdoc />
        protected override void RemoveItem(int index)
        {
            var e = new CollectionChangingEventArgs(CollectionChangeAction.Remove, index, Items[index]);
            OnCollectionChanging(e);

            if (e.Cancel)
                return;

            LogRemove(this[index]);
            base.RemoveItem(index);
        }

        /// <inheritdoc />
        protected override void ClearItems()
        {
            var e = new CollectionChangingEventArgs(CollectionChangeAction.Refresh);
            OnCollectionChanging(e);

            if (e.Cancel)
                return;

            foreach (var item in this)
            {
                LogRemove(item);
            }

            base.ClearItems();
        }

        /// <summary>
        /// Suspends the notifications.
        /// </summary>
        public virtual void SuspendNotifications()
        {
            _notificationsSuspended = true;
        }

        /// <summary>
        /// Resumes the notifications.
        /// </summary>
        public virtual void ResumeNotifications()
        {
            _notificationsSuspended = false;

            if (IsDirty)
            {
                IsDirty = false;
                OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                RaiseCollectionChangedOnResume();
            }
        }

        /// <summary>
        /// Raises the CollectionChanged in accordance to the value of ShouldResetOnResumeNotifications and the presence of modified items.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        protected virtual void RaiseCollectionChangedOnResume()
        {
            if (ShouldResetOnResumeNotifications)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
            else
            {
                var shouldReset = true;
                if (AddedItems.Count > 0)
                {
                    shouldReset = false;

                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, AddedItems.ToList(), _lastStartingIndex));
                    _lastStartingIndex = -1;
                    AddedItems.Clear();
                }

                if (RemovedItems.Count > 0)
                {
                    shouldReset = false;
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, RemovedItems.ToList(), _lastStartingIndex));
                    _lastStartingIndex = -1;
                    RemovedItems.Clear();
                }

                if (shouldReset)
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }
        }


        #endregion
    }
}
