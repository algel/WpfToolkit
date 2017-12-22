using System;
using JetBrains.Annotations;

namespace Algel.WpfTools.ComponentModel
{
    /// <inheritdoc />
    [PublicAPI]
    public class IndexReadonlyProperty<T, TKey> : IndexPropertyReadonlyBase<T, TKey>
    {
        /// <inheritdoc />
        public IndexReadonlyProperty(Func<TKey, T> g) : base(g)
        {
        }
    }
}