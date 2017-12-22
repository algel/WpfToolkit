using System;
using JetBrains.Annotations;

namespace Algel.WpfTools.ComponentModel
{
    /// <summary>
    /// Base class for properties provide read and write access to values by two keys
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <typeparam name="TKey1">The type of key1</typeparam>
    /// <typeparam name="TKey2">The type of key2</typeparam>
    [PublicAPI]
    public class IndexProperty<T, TKey1, TKey2> : IndexPropertyBase<T, Tuple<TKey1, TKey2>>
    {
        /// <inheritdoc />
        public IndexProperty(Func<TKey1, TKey2, T> g, Action<TKey1, TKey2, T> s) : base(key => g(key.Item1, key.Item2), (key, value) => s(key.Item1, key.Item2, value))
        {
        }

        /// <inheritdoc />
        public IndexProperty(Func<TKey1, TKey2, T> g, Action<TKey1, TKey2, T, T> s) : base(key => g(key.Item1, key.Item2), (key, oldValue, value) => s(key.Item1, key.Item2, oldValue, value))
        {
        }

        /// <summary>
        /// Get or set value by keys
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        public T this[TKey1 key1, TKey2 key2]
        {
            get => base[new Tuple<TKey1, TKey2>(key1, key2)];
            set => base[new Tuple<TKey1, TKey2>(key1, key2)] = value;
        }
    }
}