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
    public abstract class IndexPropertyBase<T, TKey> : IndexPropertyReadonlyBase<T, TKey>, INotifyPropertyChanged
    {
        private readonly Action<TKey, T> _setter;
        private readonly Action<TKey, T, T> _setterWithOldValue;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        protected IndexPropertyBase(Func<TKey, T> g, Action<TKey, T> s) : base(g)
        {
            _setter = s ?? throw new ArgumentNullException(nameof(s));
            PropertyChanged += IndexPropertyBase_PropertyChanged;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        protected IndexPropertyBase(Func<TKey, T> g, Action<TKey, T, T> s) : base(g)
        {
            _setterWithOldValue = s ?? throw new ArgumentNullException(nameof(s));
            PropertyChanged += IndexPropertyBase_PropertyChanged;
        }


        private void IndexPropertyBase_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName"></param>
        [NotifyPropertyChangedInvocator]
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// <see cref="PropertyChanged"/> event handler.
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName)
        {

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