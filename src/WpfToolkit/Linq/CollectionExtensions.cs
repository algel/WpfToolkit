using System;
using System.Collections.Generic;
using System.Linq;

namespace WpfToolset.Linq
{
    internal static class CollectionExtensions
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