using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if WINDOWS
using Windows.Foundation.Collections;
#endif

namespace SimpleWeather.Utils
{
    public static class CollectionUtils
    {
        public static T Random<T>(this IEnumerable<T> e)
        {
            return e.ElementAt(new Random().Next(0, e.Count()));
        }

        public static T RandomOrDefault<T>(this IEnumerable<T> e)
        {
            return e.ElementAtOrDefault(new Random().Next(0, e.Count()));
        }

        public static IEnumerable<T> WhereNot<T>(this IEnumerable<T> e, Func<T, bool> predicate)
        {
            return e.Where(it => !predicate(it));
        }

        public static void ForEach<T>(this IEnumerable<T> e, Action<T> action)
        {
            foreach (T item in e)
            {
                action(item);
            }
        }

        public static void ForEach(this IEnumerable e, Action<object> action)
        {
            foreach (object item in e)
            {
                action(item);
            }
        }

        public static int IndexOf(this IEnumerable e, object value)
        {
            int index = 0;

            foreach (object item in e)
            {
                if (Equals(item, value))
                    return index;

                index++;
            }

            return -1;
        }

        public static int IndexOf<T>(this IEnumerable<T> e, T value)
        {
            int index = 0;

            var comparer = EqualityComparer<T>.Default;

            foreach (T item in e)
            {
                if (comparer.Equals(item, value))
                    return index;

                index++;
            }

            return -1;
        }

        public static void ForEachIndexed<T>(this IEnumerable<T> e, Action<int, T> action)
        {
            var i = 0;
            foreach (T item in e)
            {
                action(i, item);
                i++;
            }
        }

        public static void ForEachIndexed(this IEnumerable e, Action<int, object> action)
        {
            var i = 0;
            foreach (object item in e)
            {
                action(i, item);
                i++;
            }
        }

#if WINDOWS
#nullable enable
        public static object? GetValueOrDefault(this IPropertySet dictionary, string key) =>
            dictionary.GetValueOrDefault(key, default!);

        public static object GetValueOrDefault(this IPropertySet dictionary, string key, object defaultValue)
        {
            if (dictionary is null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            return dictionary.TryGetValue(key, out object? value) ? value : defaultValue;
        }
#nullable restore
#endif
    }
}
