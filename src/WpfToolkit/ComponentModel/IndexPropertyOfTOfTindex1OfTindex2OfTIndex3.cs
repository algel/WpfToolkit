using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace WpfToolset.ComponentModel
{
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
}