using System;
using System.Collections.Generic;

namespace WpfToolset.Collections
{
    internal class WeakReferenceList<T> : IList<T>
    {
        private readonly List<WeakReference> _items = new List<WeakReference>();

        public int Count => _items.Count;

        public bool IsReadOnly => false;

        public T this[int index]
        {
            get
            {
                if (_items[index].IsAlive)
                {
                    return (T)_items[index].Target;
                }

                return default(T);
            }
            set
            {
                if (_items.Count > index)
                {
                    _items[index] = new WeakReference(value);
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index out of range!");
                }
            }
        }

        public int IndexOf(T item)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < _items.Count; i++)
            {
                if (comparer.Equals((T)_items[i].Target, item))
                {
                    return i;
                }
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            _items.Insert(index, new WeakReference(item));
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        public void Add(T item)
        {
            Insert(_items.Count, item);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(T item)
        {
            return IndexOf(item) > -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                array[arrayIndex + i] = item.IsAlive ? (T)item.Target : default(T);
            }
        }

        public bool Remove(T item)
        {
            int i = IndexOf(item);
            if (i < 0)
            {
                return false;
            }

            RemoveAt(i);
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in _items)
            {
                yield return item.IsAlive ? (T)item.Target : default(T);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}