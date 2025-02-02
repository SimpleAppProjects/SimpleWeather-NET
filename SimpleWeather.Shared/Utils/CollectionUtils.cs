using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if __IOS__
using Foundation;
#endif
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

        public static void RemoveRange<T>(this IList<T> list, int start, int count)
        {
            var end = start + count;

            if (start < 0 || count < 0 || end > list.Count)
            {
                throw new IndexOutOfRangeException();
            }

            for (int i = end - 1; i >= start; i--)
            {
                list.RemoveAt(i);
            }
        }

#if __IOS__
        public static bool ContainsKey(this NSDictionary dict, object key)
        {
            return dict.ContainsKey(NSObject.FromObject(key));
        }
#endif

        public static string ToString(this IEnumerable list)
        {
            return list.Cast<object>().ToString<object>();
        }

        public static string ToString(this IEnumerable list, Func<object, string> toStringConv)
        {
            return list.Cast<object>().ToString(toStringConv);
        }

        public static string ToString<T>(this IEnumerable<T> list)
        {
            return $"[{string.Join(',', list.Select(x => x.ToString()))}]";
        }

        public static string ToString<T>(this IEnumerable<T> list, Func<T, string> toStringConv)
        {
            return $"[{string.Join(',', list.Select(toStringConv))}]";
        }
    }
}
