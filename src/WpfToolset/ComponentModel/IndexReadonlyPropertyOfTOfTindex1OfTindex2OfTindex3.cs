using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace WpfToolset.ComponentModel
{
    public class IndexReadonlyProperty<T, TIndex1, TIndex2, TIndex3>
    {
        private readonly Func<TIndex1, TIndex2, TIndex3, T> _getter;

        public IndexReadonlyProperty(Func<TIndex1, TIndex2, TIndex3, T> g)
        {
            _getter = g ?? throw new ArgumentNullException(nameof(g));
        }

        [IndexerName("Item")]
        public T this[TIndex1 index1, TIndex2 index2, TIndex3 index3] => _getter(index1, index2, index3);

        [IndexerName("Item")]
        public T this[Tuple<TIndex1, TIndex2, TIndex3> key] => _getter(key.Item1, key.Item2, key.Item3);

        public IEnumerable<T> AsEnumerable(IEnumerable<Tuple<TIndex1, TIndex2, TIndex3>> indices)
        {
            return indices.Select(t => this[t]);
        }
    }
}