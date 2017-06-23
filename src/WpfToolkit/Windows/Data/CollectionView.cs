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

namespace WpfToolkit.Windows.Data
{
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

        #endregion

        #region .ctor

        public CollectionView([NotNull] ICollectionView generic)
        {
            _collectionView = generic ?? throw new ArgumentNullException(nameof(generic));
            CurrentChanged += (sender, e) => RaisePropertyChanged(nameof(CurrentItem));
        }

        /// <inheritdoc />
        public CollectionView([NotNull] IEnumerable source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var cvf = source as ICollectionViewFactory;
            if (cvf != null)
                _collectionView = cvf.CreateView();

            if (_collectionView == null)
            {
                var list = source as IBindingList;
                if (list != null)
                    _collectionView = new BindingListCollectionView(list);
            }

            if (_collectionView == null)
            {
                var list = source as IList;
                if (list != null)
                    _collectionView = new ListCollectionView(list);
            }

            if (_collectionView == null)
            {
                _enumerableWrapper = new EnumerableWrapper(source);
                _collectionView = new ListCollectionView(_enumerableWrapper);
            }
            CurrentChanged += (sender, e) => RaisePropertyChanged(nameof(CurrentItem));
        }

        #endregion

        #region Methods

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator<T>(_collectionView.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _collectionView.GetEnumerator();
        }

        public bool Contains(object item)
        {
            return _collectionView.Contains(item);
        }

        public void Refresh()
        {
            using (_collectionView.DeferRefresh())
            {
                _enumerableWrapper?.RefreshList();
            }
        }

        public IDisposable DeferRefresh()
        {
            return _collectionView.DeferRefresh();
        }

        public bool MoveCurrentToFirst()
        {
            return _collectionView.MoveCurrentToFirst();
        }

        public bool MoveCurrentToLast()
        {
            return _collectionView.MoveCurrentToLast();
        }

        public bool MoveCurrentToNext()
        {
            return _collectionView.MoveCurrentToNext();
        }

        public bool MoveCurrentToPrevious()
        {
            return _collectionView.MoveCurrentToPrevious();
        }

        public bool MoveCurrentTo(object item)
        {
            return _collectionView.MoveCurrentTo(item);
        }

        public bool MoveCurrentToPosition(int position)
        {
            return _collectionView.MoveCurrentToPosition(position);
        }

        #endregion

        #region Properties

        public CultureInfo Culture
        {
            get { return _collectionView.Culture; }
            set { _collectionView.Culture = value; }
        }

        public bool CanFilter => _collectionView.CanFilter;

        public SortDescriptionCollection SortDescriptions => _collectionView.SortDescriptions;

        public bool CanSort => _collectionView.CanSort;

        public bool CanGroup => _collectionView.CanGroup;

        public ObservableCollection<GroupDescription> GroupDescriptions => _collectionView.GroupDescriptions;

        public ReadOnlyObservableCollection<object> Groups => _collectionView.Groups;

        public bool IsEmpty => _collectionView.IsEmpty;

        public int CurrentPosition => _collectionView.CurrentPosition;

        public bool IsCurrentAfterLast => _collectionView.IsCurrentAfterLast;

        public bool IsCurrentBeforeFirst => _collectionView.IsCurrentBeforeFirst;

        Predicate<object> ICollectionView.Filter
        {
            get { return _collectionView.Filter; }
            set
            {
                _collectionView.Filter = value;
                _filter = null;
            }
        }

        [CanBeNull]
        public Predicate<T> Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                if (_filter != null)
                    _collectionView.Filter = e => _filter(e != null ? (T)e : default(T));
                else
                    _collectionView.Filter = null;
            }
        }

        object ICollectionView.CurrentItem => _collectionView.CurrentItem;

        public T CurrentItem => (T)_collectionView.CurrentItem;

        IEnumerable ICollectionView.SourceCollection => _collectionView.SourceCollection;

        [NotNull]
        public IEnumerable<T> SourceCollection => _collectionView.SourceCollection.Cast<T>();

        #endregion

        #region Events

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

        public class EnumerableWrapper : IList, INotifyCollectionChanged, IWeakEventListener
        {
            private readonly IEnumerable _source;
            private ArrayList _listImplementation;

            public EnumerableWrapper(IEnumerable source)
            {
                _source = source;
                RefreshList();
                var notifyCollection = _source as INotifyCollectionChanged;
                if (notifyCollection != null)
                    CollectionChangedEventManager.AddListener(notifyCollection, this);
            }

            public IEnumerator GetEnumerator()
            {
                return _listImplementation.GetEnumerator();
            }

            public void CopyTo(Array array, int index)
            {
                _listImplementation.CopyTo(array, index);
            }

            public int Count => _listImplementation.Count;

            public object SyncRoot => _listImplementation.SyncRoot;

            public bool IsSynchronized => _listImplementation.IsSynchronized;

            public int Add(object value)
            {
                return _listImplementation.Add(value);
            }

            public bool Contains(object value)
            {
                return _listImplementation.Contains(value);
            }

            public void Clear()
            {
                _listImplementation.Clear();
            }

            public int IndexOf(object value)
            {
                return _listImplementation.IndexOf(value);
            }

            public void Insert(int index, object value)
            {
                _listImplementation.Insert(index, value);
            }

            public void Remove(object value)
            {
                _listImplementation.Remove(value);
            }

            public void RemoveAt(int index)
            {
                _listImplementation.RemoveAt(index);
            }

            public object this[int index]
            {
                get { return _listImplementation[index]; }
                set { _listImplementation[index] = value; }
            }

            public bool IsReadOnly => _listImplementation.IsReadOnly;

            public bool IsFixedSize => _listImplementation.IsFixedSize;

            public event NotifyCollectionChangedEventHandler CollectionChanged;

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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
