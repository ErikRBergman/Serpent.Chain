namespace Serpent.Common.BaseTypeExtensions.Collections
{
    using System;
    using System.Collections.Generic;

    public static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue @default = default(TValue))
        {
            return dictionary.TryGetValue(key, out TValue value) ? value : @default;
        }

        public static bool DoForKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Action<TKey, TValue> action)
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                action(key, value);
                return true;
            }

            return false;
        }

        public static bool DoForKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Action<TValue> action)
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                action(value);
                return true;
            }

            return false;
        }

        public static bool DoForKey<TKey, TValue, TContext>(this IDictionary<TKey, TValue> dictionary, TKey key, TContext context, Action<TKey, TValue, TContext> action)
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                action(key, value, context);
                return true;
            }

            return false;
        }

        public static bool DoForKey<TKey, TValue, TContext>(this IDictionary<TKey, TValue> dictionary, TKey key, TContext context, Action<TValue, TContext> action)
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                action(value, context);
                return true;
            }

            return false;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue, TType>(
            this IReadOnlyCollection<TType> items,
            Func<TType, TKey> keySelector,
            Func<TType, TValue> valueSelector)
        {
            var dictionary = new Dictionary<TKey, TValue>(items.Count);

            foreach (var item in items)
            {
                dictionary.Add(keySelector(item), valueSelector(item));
            }

            return dictionary;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue, TType>(
            this IReadOnlyCollection<TType> items,
            Func<TType, TKey> keySelector,
            Func<TType, TValue> valueSelector,
            IEqualityComparer<TKey> equalityComparer)
        {
            var dictionary = new Dictionary<TKey, TValue>(items.Count, equalityComparer);

            foreach (var item in items)
            {
                dictionary.Add(keySelector(item), valueSelector(item));
            }

            return dictionary;
        }
    }
}