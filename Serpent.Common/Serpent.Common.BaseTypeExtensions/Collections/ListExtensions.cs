namespace Serpent.Common.BaseTypeExtensions.Collections
{
    using System.Collections.Generic;

    public static class ListExtensions
    {
        public static T LastItem<T>(this IList<T> list)
        {
            return list[list.Count - 1];
        }

        public static T LastItemOrDefault<T>(this IList<T> list, T @default)
        {
            if (list.Count > 0)
            {
                return list[list.Count - 1];
            }

            return @default;
        }


        public static T LastItem<T>(this IList<T> list, int indexFromTheEnd)
        {
            return list[list.Count - 1 - indexFromTheEnd];
        }

        public static void RemoveLast<T>(this IList<T> list)
        {
            list.RemoveAt(list.Count - 1);
        }

    }
}