using System;
using JetBrains.Annotations;

namespace Algel.WpfTools.ComponentModel
{
    /// <summary>
    /// Provide read access to value by two keys
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <typeparam name="TKey1">The type of key1</typeparam>
    /// <typeparam name="TKey2">The type of key2</typeparam>
    [PublicAPI]
    public class IndexReadonlyProperty<T, TKey1, TKey2> : IndexReadonlyProperty<T, Tuple<TKey1, TKey2>>
    {
        /// <inheritdoc />
        public IndexReadonlyProperty(Func<TKey1, TKey2, T> g) : base(key=>g(key.Item1, key.Item2))
        {
        }

        /// <summary>
        /// Get value by keys
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        public T this[TKey1 key1, TKey2 key2] => base[new Tuple<TKey1, TKey2>(key1, key2)];

    }
}