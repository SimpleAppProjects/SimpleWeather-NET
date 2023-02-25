using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
