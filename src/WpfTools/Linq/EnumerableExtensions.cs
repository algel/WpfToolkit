using System;
using System.Collections.Generic;

namespace Algel.WpfTools.Linq
{
    internal static class EnumerableExtensions
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
            foreach (var obj in sequence)
                action(obj);
        }
    }
}
