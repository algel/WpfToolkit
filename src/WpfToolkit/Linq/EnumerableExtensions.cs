using System;
using System.Collections.Generic;
using System.Linq;

namespace WpfToolset.Linq
{
    static class EnumerableExtensions
    {
        /// <summary>
        /// Execute the provided <paramref name="action" /> on every item in <paramref name="sequence" />.
        /// </summary>
        /// <typeparam name="TItem">Type of the items stored in <paramref name="sequence" /></typeparam>
        /// <param name="sequence">Sequence of items to process.</param>
        /// <param name="action">Code to run over each item.</param>
        public static void ForEach<TItem>(this IEnumerable<TItem> sequence, Action<TItem> action)
        {
            if (sequence == null)
                return;
            foreach (TItem obj in sequence)
                action(obj);
        }
    }

    static class CollectionExtensions
    {
        /// <summary>
        /// Adds the elements from the specified collection - <paramref name="items"/> to the end of the target <paramref name="collection"/>.
        /// </summary>
        /// <param name="collection">The collection that will be extended.</param>
        /// <param name="items">The items that will be added.</param>
        /// <exception cref="ArgumentNullException"><paramref name="items"/> is null</exception>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            items.ToList().ForEach(collection.Add);
        }
    }
}
