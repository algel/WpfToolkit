using System;
using JetBrains.Annotations;

namespace Algel.WpfTools.ComponentModel
{
    /// <summary>
    /// Base class for properties provide read and write access to values by key
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <typeparam name="TKey">The type of key</typeparam>
    [PublicAPI]
    public class IndexProperty<T, TKey> : IndexPropertyBase<T, TKey>
    {
        /// <inheritdoc />
        public IndexProperty(Func<TKey, T> g, Action<TKey, T> s) : base(g, s)
        {
        }

        /// <inheritdoc />
        public IndexProperty(Func<TKey, T> g, Action<TKey, T, T> s) : base(g, s)
        {
        }
    }
}
