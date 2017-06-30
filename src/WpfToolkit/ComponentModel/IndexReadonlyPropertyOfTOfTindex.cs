using System;
using System.Runtime.CompilerServices;

namespace WpfToolset.ComponentModel
{
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
}