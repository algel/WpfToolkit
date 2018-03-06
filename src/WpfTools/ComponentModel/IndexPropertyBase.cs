using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using JetBrains.Annotations;

namespace Algel.WpfTools.ComponentModel
{
    /// <summary>
    /// Base class for properties provide read and write access to values by key
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <typeparam name="TKey">The type of key</typeparam>
    [PublicAPI]
    public abstract class IndexPropertyBase<T, TKey> : IndexPropertyReadonlyBase<T, TKey>
    {
        private readonly Action<TKey, T> _setter;
        private readonly Action<TKey, T, T> _setterWithOldValue;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        protected IndexPropertyBase(Func<TKey, T> g, Action<TKey, T> s) : base(g)
        {
            _setter = s ?? throw new ArgumentNullException(nameof(s));
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        protected IndexPropertyBase(Func<TKey, T> g, Action<TKey, T, T> s) : base(g)
        {
            _setterWithOldValue = s ?? throw new ArgumentNullException(nameof(s));
        }

        /// <summary>
        /// Get or set value by index
        /// </summary>
        /// <param name="index"></param>
        public new T this[TKey index]
        {
            get => base[index];
            set
            {
                var oldValue = GetOldValueOrDefault(index);

                if (!Equals(oldValue, value))
                {
                    if (_setter != null)
                        _setter(index, value);
                    else
                        _setterWithOldValue(index, oldValue, value);

                    RaisePropertyChanged(Binding.IndexerName);
                }
            }
        }

    }
}