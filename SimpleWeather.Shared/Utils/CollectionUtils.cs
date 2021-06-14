using System;
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
    }
}
