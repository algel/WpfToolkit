using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Algel.WpfTools.ComponentModel
{
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

        [IndexerName("Item")]
        public T this[Tuple<TIndex1, TIndex2> key]
        {
            get { return this[key.Item1, key.Item2]; }
            set { this[key.Item1, key.Item2] = value; }
        }

        public IEnumerable<T> AsEnumerable(IEnumerable<Tuple<TIndex1, TIndex2>> indices)
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