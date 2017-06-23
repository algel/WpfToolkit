using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace WpfToolkit.ComponentModel
{
    public class IndexProperty<T, TIndex> : INotifyPropertyChanged
    {
        private readonly Func<TIndex, T> _getter;
        private readonly Action<TIndex, T> _setter;
        private readonly Action<TIndex, T, T> _setterWithOldValue;

        public IndexProperty(Func<TIndex, T> g, Action<TIndex, T> s)
        {
            _getter = g ?? throw new ArgumentNullException(nameof(g));
            _setter = s ?? throw new ArgumentNullException(nameof(s));
        }

        public IndexProperty(Func<TIndex, T> g, Action<TIndex, T, T> s)
        {
            _getter = g ?? throw new ArgumentNullException(nameof(g));
            _setterWithOldValue = s ?? throw new ArgumentNullException(nameof(s));
        }

        [IndexerName("Item")]
        public T this[TIndex index]
        {
            get
            {
                return _getter(index);
            }
            set
            {
                var oldValue = _getter(index);
                if (!Equals(oldValue, value))
                {
                    if (_setter != null)
                        _setter(index, value);
                    else
                        _setterWithOldValue(index, oldValue, value);

                    // ReSharper disable once NotResolvedInText
                    OnPropertyChanged("Item[]");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class IndexProperty<T, TIndex1, TIndex2> : INotifyPropertyChanged
    {
        private readonly Func<TIndex1, TIndex2, T> _getter;
        private readonly Action<TIndex1, TIndex2, T> _setter;
        private readonly Action<TIndex1, TIndex2, T, T> _setterWithOldValue;

        public IndexProperty(Func<TIndex1, TIndex2, T> g, Action<TIndex1, TIndex2, T> s)
        {
            _getter = g ?? throw new ArgumentNullException(nameof(g));
            _setter = s ?? throw new ArgumentNullException(nameof(s));
        }

        public IndexProperty(Func<TIndex1, TIndex2, T> g, Action<TIndex1, TIndex2, T, T> s)
        {
            _getter = g ?? throw new ArgumentNullException(nameof(g));
            _setterWithOldValue = s ?? throw new ArgumentNullException(nameof(s));
        }

        [IndexerName("Item")]
        public T this[TIndex1 index1, TIndex2 index2]
        {
            get
            {
                return _getter(index1, index2);
            }
            set
            {
                var oldValue = _getter(index1, index2);
                if (!Equals(oldValue, value))
                {
                    if (_setter != null)
                        _setter(index1, index2, value);
                    else
                        _setterWithOldValue(index1, index2, oldValue, value);

                    // ReSharper disable once NotResolvedInText
                    OnPropertyChanged("Item[]");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class IndexProperty<T, TIndex1, TIndex2, TIndex3> : INotifyPropertyChanged
    {
        private readonly Func<TIndex1, TIndex2, TIndex3, T> _getter;
        private readonly Action<TIndex1, TIndex2, TIndex3, T> _setter;
        private readonly Action<TIndex1, TIndex2, TIndex3, T, T> _setterWithOldValue;

        public IndexProperty(Func<TIndex1, TIndex2, TIndex3, T> g, Action<TIndex1, TIndex2, TIndex3, T> s)
        {
            _getter = g ?? throw new ArgumentNullException(nameof(g));
            _setter = s ?? throw new ArgumentNullException(nameof(s));
        }

        public IndexProperty(Func<TIndex1, TIndex2, TIndex3, T> g, Action<TIndex1, TIndex2, TIndex3, T, T> s)
        {
            _getter = g ?? throw new ArgumentNullException(nameof(g));
            _setterWithOldValue = s ?? throw new ArgumentNullException(nameof(s));
        }

        [IndexerName("Item")]
        public T this[TIndex1 index1, TIndex2 index2, TIndex3 index3]
        {
            get
            {
                return _getter(index1, index2, index3);
            }
            set
            {
                var oldValue = _getter(index1, index2, index3);
                if (!Equals(oldValue, value))
                {
                    if (_setter != null)
                        _setter(index1, index2, index3, value);
                    else
                        _setterWithOldValue(index1, index2, index3, oldValue, value);

                    // ReSharper disable once NotResolvedInText
                    OnPropertyChanged("Item[]");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class IndexReadonlyProperty<T, TIndex>
    {
        private readonly Func<TIndex, T> _getter;

        public IndexReadonlyProperty(Func<TIndex, T> g)
        {
            _getter = g ?? throw new ArgumentNullException(nameof(g));
        }

        [IndexerName("Item")]
        public T this[TIndex index] => _getter(index);
    }

    public class IndexReadonlyProperty<T, TIndex1, TIndex2>
    {
        private readonly Func<TIndex1, TIndex2, T> _getter;

        public IndexReadonlyProperty(Func<TIndex1, TIndex2, T> g)
        {
            _getter = g ?? throw new ArgumentNullException(nameof(g));
        }

        [IndexerName("Item")]
        public T this[TIndex1 index1, TIndex2 index2] => _getter(index1, index2);
    }

    public class IndexReadonlyProperty<T, TIndex1, TIndex2, TIndex3>
    {
        private readonly Func<TIndex1, TIndex2, TIndex3, T> _getter;

        public IndexReadonlyProperty(Func<TIndex1, TIndex2, TIndex3, T> g)
        {
            _getter = g ?? throw new ArgumentNullException(nameof(g));
        }

        [IndexerName("Item")]
        public T this[TIndex1 index1, TIndex2 index2, TIndex3 index3] => _getter(index1, index2, index3);
    }
}
