using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace WpfToolset.ComponentModel
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

        public IEnumerable<T> AsEnumerable(IEnumerable<TIndex> indices)
        {
            return indices.Select(t => this[t]);
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
