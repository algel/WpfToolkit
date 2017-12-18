using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Algel.WpfTools.ComponentModel
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

        public IEnumerable<T> AsEnumerable(IEnumerable<TIndex> indices)
        {
            return indices.Select(t => this[t]);
        }
    }
}