using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Algel.WpfTools.ComponentModel
{
    /// <summary>
    /// Base class for properties provide read access to values by key
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <typeparam name="TKey">The type of key</typeparam>
    [PublicAPI]
    public abstract class IndexPropertyReadonlyBase<T, TKey>
    {
        #region Fields

        private readonly Func<TKey, T> _getter;

        #endregion

        #region .ctor

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        protected IndexPropertyReadonlyBase(Func<TKey, T> getter)
        {
            _getter = getter ?? throw new ArgumentNullException(nameof(getter));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets value by key
        /// </summary>
        /// <param name="key"></param>
        public T this[TKey key] => _getter(key);

        #endregion

        #region Methods

        /// <summary>
        /// Try get value by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected T GetOldValueOrDefault(TKey key)
        {
            try
            {
                return _getter(key);
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// Get values by <paramref name="indices"/>
        /// </summary>
        /// <param name="indices"></param>
        /// <returns></returns>
        public IEnumerable<T> AsEnumerable(IEnumerable<TKey> indices)
        {
            return indices.Select(t => this[t]);
        }

        #endregion

    }
}