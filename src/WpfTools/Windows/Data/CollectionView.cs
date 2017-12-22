using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using JetBrains.Annotations;

namespace Algel.WpfTools.Windows.Data
{
    /// <inheritdoc cref="ICollectionView" />
    [PublicAPI]
    public class CollectionView<T> : IEnumerable<T>, ICollectionView, INotifyPropertyChanged
    {
        #region Fields

        [NotNull]
        private readonly ICollectionView _collectionView;
        [NotNull]
        private readonly object _objectLock = new object();
        [CanBeNull]
        private Predicate<T> _filter;

        private readonly EnumerableWrapper _enumerableWrapper;
        //private readonly IndexedEnumerable _enumerableWrapper;

        #endregion

        #region .ctor

        /// <inheritdoc />
        public CollectionView([NotNull] ICollectionView generic)
        {
            _collectionView = generic ?? throw new ArgumentNullException(nameof(generic));
            CurrentChanged += (sender, e) => RaisePropertyChanged(nameof(CurrentItem));
        }

        /// <inheritdoc />
        // ReSharper disable once NotNullMemberIsNotInitialized
        public CollectionView([NotNull] IEnumerable source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (source is ICollectionViewFactory cvf)
                _collectionView = cvf.CreateView();

            if (_collectionView == null)
            {
                if (source is IBindingList list)
                    _collectionView = new BindingListCollectionView(list);
            }

            if (_collectionView == null)
            {
                if (source is IList list)
                    _collectionView = new ListCollectionView(list);
            }

            if (_collectionView == null)
            {
                _enumerableWrapper = new EnumerableWrapper(source);
                _collectionView = new ListCollectionView(_enumerableWrapper);
                //_enumerableWrapper = new IndexedEnumerable(source);
                //_collectionView = _enumerableWrapper.CollectionView; 
            }
            CurrentChanged += (sender, e) => RaisePropertyChanged(nameof(CurrentItem));
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator<T>(_collectionView.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _collectionView.GetEnumerator();
        }

        /// <inheritdoc />
        public bool Contains(object item)
        {
            return _collectionView.Contains(item);
        }

        /// <inheritdoc />
        public void Refresh()
        {
            if (_enumerableWrapper != null)
            {
                using (_collectionView.DeferRefresh())
                {
                    _enumerableWrapper?.RefreshList();
                }
            }
            else
            {
                _collectionView.Refresh();
            }
        }

        /// <inheritdoc />
        public IDisposable DeferRefresh()
        {
            return _collectionView.DeferRefresh();
        }

        /// <inheritdoc />
        public bool MoveCurrentToFirst()
        {
            return _collectionView.MoveCurrentToFirst();
        }

        /// <inheritdoc />
        public bool MoveCurrentToLast()
        {
            return _collectionView.MoveCurrentToLast();
        }

        /// <inheritdoc />
        public bool MoveCurrentToNext()
        {
            return _collectionView.MoveCurrentToNext();
        }

        /// <inheritdoc />
        public bool MoveCurrentToPrevious()
        {
            return _collectionView.MoveCurrentToPrevious();
        }

        /// <inheritdoc />
        public bool MoveCurrentTo(object item)
        {
            return _collectionView.MoveCurrentTo(item);
        }

        /// <inheritdoc />
        public bool MoveCurrentToPosition(int position)
        {
            return _collectionView.MoveCurrentToPosition(position);
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public CultureInfo Culture
        {
            get => _collectionView.Culture;
            set => _collectionView.Culture = value;
        }

        /// <inheritdoc />
        public bool CanFilter => _collectionView.CanFilter;

        /// <inheritdoc />
        public SortDescriptionCollection SortDescriptions => _collectionView.SortDescriptions;

        /// <inheritdoc />
        public bool CanSort => _collectionView.CanSort;

        /// <inheritdoc />
        public bool CanGroup => _collectionView.CanGroup;

        /// <inheritdoc />
        public ObservableCollection<GroupDescription> GroupDescriptions => _collectionView.GroupDescriptions;

        /// <inheritdoc />
        public ReadOnlyObservableCollection<object> Groups => _collectionView.Groups;

        /// <inheritdoc />
        public bool IsEmpty => _collectionView.IsEmpty;

        /// <inheritdoc />
        public int CurrentPosition => _collectionView.CurrentPosition;

        /// <inheritdoc />
        public bool IsCurrentAfterLast => _collectionView.IsCurrentAfterLast;

        /// <inheritdoc />
        public bool IsCurrentBeforeFirst => _collectionView.IsCurrentBeforeFirst;

        Predicate<object> ICollectionView.Filter
        {
            get => _collectionView.Filter;
            set
            {
                _collectionView.Filter = value;
                _filter = null;
            }
        }

        /// <inheritdoc cref="ICollectionView.Filter" />
        [CanBeNull]
        public Predicate<T> Filter
        {
            get => _filter;
            set
            {
                _filter = value;
                if (_filter != null)
                    _collectionView.Filter = e => _filter((T) e);
                else
                    _collectionView.Filter = null;
            }
        }

        /// <inheritdoc />
        object ICollectionView.CurrentItem => _collectionView.CurrentItem;

        /// <inheritdoc cref="ICollectionView.CurrentItem" />
        public T CurrentItem => (T)_collectionView.CurrentItem;

        IEnumerable ICollectionView.SourceCollection => _collectionView.SourceCollection;

        /// <inheritdoc cref="ICollectionView.SourceCollection" />
        [NotNull]
        public IEnumerable<T> SourceCollection => _collectionView.SourceCollection.Cast<T>();

        #endregion

        #region Events

        /// <inheritdoc />
        public event CurrentChangingEventHandler CurrentChanging
        {
            add
            {
                lock (_objectLock)
                {
                    _collectionView.CurrentChanging += value;
                }
            }
            remove
            {
                lock (_objectLock)
                {
                    _collectionView.CurrentChanging -= value;
                }
            }
        }

        /// <inheritdoc />
        public event EventHandler CurrentChanged
        {
            add
            {
                lock (_objectLock)
                {
                    _collectionView.CurrentChanged += value;
                }
            }
            remove
            {
                lock (_objectLock)
                {
                    _collectionView.CurrentChanged -= value;
                }
            }
        }

        /// <inheritdoc />
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                lock (_objectLock)
                {
                    _collectionView.CollectionChanged += value;
                }
            }
            remove
            {
                lock (_objectLock)
                {
                    _collectionView.CollectionChanged -= value;
                }
            }
        }

        #endregion

        #region Nested types

        private sealed class Enumerator<TItem> : IEnumerator<TItem>
        {
            [NotNull]
            private readonly IEnumerator _enumerator;

            public Enumerator([NotNull] IEnumerator enumerator)
            {
                _enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                return _enumerator.MoveNext();
            }

            public void Reset()
            {
                _enumerator.Reset();
            }

            public TItem Current => (TItem)_enumerator.Current;

            object IEnumerator.Current => Current;
        }

        private class EnumerableWrapper : IList, INotifyCollectionChanged, IWeakEventListener
        {
            private readonly IEnumerable _source;
            private ArrayList _listImplementation;

            public EnumerableWrapper(IEnumerable source)
            {
                _source = source;
                RefreshList();
                if (_source is INotifyCollectionChanged notifyCollection)
                    CollectionChangedEventManager.AddListener(notifyCollection, this);
            }

            /// <inheritdoc />
            public IEnumerator GetEnumerator()
            {
                return _listImplementation.GetEnumerator();
            }

            /// <inheritdoc />
            public void CopyTo(Array array, int index)
            {
                _listImplementation.CopyTo(array, index);
            }

            /// <inheritdoc />
            public int Count => _listImplementation.Count;

            /// <inheritdoc />
            public object SyncRoot => _listImplementation.SyncRoot;

            /// <inheritdoc />
            public bool IsSynchronized => _listImplementation.IsSynchronized;

            /// <inheritdoc />
            public int Add(object value)
            {
                return _listImplementation.Add(value);
            }

            /// <inheritdoc />
            public bool Contains(object value)
            {
                return _listImplementation.Contains(value);
            }

            /// <inheritdoc />
            public void Clear()
            {
                _listImplementation.Clear();
            }

            /// <inheritdoc />
            public int IndexOf(object value)
            {
                return _listImplementation.IndexOf(value);
            }

            /// <inheritdoc />
            public void Insert(int index, object value)
            {
                _listImplementation.Insert(index, value);
            }

            /// <inheritdoc />
            public void Remove(object value)
            {
                _listImplementation.Remove(value);
            }

            /// <inheritdoc />
            public void RemoveAt(int index)
            {
                _listImplementation.RemoveAt(index);
            }

            /// <inheritdoc />
            public object this[int index]
            {
                get => _listImplementation[index];
                set => _listImplementation[index] = value;
            }

            /// <inheritdoc />
            public bool IsReadOnly => _listImplementation.IsReadOnly;

            /// <inheritdoc />
            public bool IsFixedSize => _listImplementation.IsFixedSize;

            /// <inheritdoc />
            public event NotifyCollectionChangedEventHandler CollectionChanged;

            /// <inheritdoc />
            public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
            {
                if (managerType == typeof(CollectionChangedEventManager))
                {
                    if (Equals(sender, _source))
                    {
                        RefreshList();
                    }
                }
                return true;
            }

            public void RefreshList()
            {
                _listImplementation = new ArrayList();
                foreach (var item in _source)
                {
                    _listImplementation.Add(item);
                }
                RaiseNotifyCollectionChanged();
            }

            private void RaiseNotifyCollectionChanged()
            {
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }


        #endregion

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raise PropertyChanged event
        /// </summary>
        /// <param name="propertyName"></param>
        [NotifyPropertyChangedInvocator]
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
