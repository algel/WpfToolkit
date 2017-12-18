using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using Algel.WpfTools.Windows.Data;

namespace Algel.WpfTools.Linq
{
    public static class CollectionViewExtensions
    {

        public static CollectionView<T> ToCollectionView<T>(this IEnumerable<T> sourceList)
        {
            return new CollectionView<T>(sourceList);
        }

        /// <summary>
        /// Adds SortDescriptor
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="member">Sort member name</param>
        /// <param name="direction">Sort direction (default value is <see cref="ListSortDirection.Ascending"/>)</param>
        /// <returns></returns>
        public static CollectionView<T> AddSort<T>(this CollectionView<T> collection, string member, ListSortDirection direction = ListSortDirection.Ascending)
        {
            collection.SortDescriptions.Add(new SortDescription(member, direction));
            return collection;
        }

        /// <summary>
        /// Adds GroupDescriptor
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="member">Group member name</param>
        /// <returns></returns>
        public static CollectionView<T> AddGroup<T>(this CollectionView<T> collection, string member)
        {
            collection.GroupDescriptions.Add(new PropertyGroupDescription(member));
            return collection;
        }

        /// <summary>
        /// Set filter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static CollectionView<T> SetFilter<T>(this CollectionView<T> collection, Predicate<T> predicate)
        {
            collection.Filter = predicate;
            return collection;
        }

    }
}