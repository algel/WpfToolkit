using System;
using System.Runtime.CompilerServices;

namespace WpfToolset.ComponentModel
{
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
}