namespace Serpent.Common.BaseTypeExtensions.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumerableExtensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> sourceItems, Func<TSource, TKey> keySelector)
        {
            var keysSeen = new HashSet<TKey>();

            foreach (var item in sourceItems)
            {
                if (keysSeen.Add(keySelector(item)))
                {
                    yield return item;
                }
            }
        }

        public static IReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> sourceItems)
        {
            return sourceItems.ToArray();
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> sourceItems)
        {
            return new HashSet<T>(sourceItems);
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> sourceItems, IEqualityComparer<T> equalityComparer)
        {
            return new HashSet<T>(sourceItems, equalityComparer);
        }

        public static Queue<T> ToQueue<T>(this IEnumerable<T> items)
        {
            return new Queue<T>(items);
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }
    }
}