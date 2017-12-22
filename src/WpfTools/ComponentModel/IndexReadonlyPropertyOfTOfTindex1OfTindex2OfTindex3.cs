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
    /// <typeparam name="TKey3">The type of key3</typeparam>
    [PublicAPI]
    public class IndexReadonlyProperty<T, TKey1, TKey2, TKey3> : IndexReadonlyProperty<T, Tuple<TKey1, TKey2, TKey3>>
    {
        /// <inheritdoc />
        public IndexReadonlyProperty(Func<TKey1, TKey2, TKey3, T> g) : base(key => g(key.Item1, key.Item2, key.Item3))
        {
        }

        /// <summary>
        /// Get value by keys
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="key3"></param>
        public T this[TKey1 key1, TKey2 key2, TKey3 key3] => base[new Tuple<TKey1, TKey2, TKey3>(key1, key2, key3)];

    }
}