using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Utils
{
    public static class CustomExtensions
    {
        /* Kotlin-like extensions */
        public static TOutput Let<TInput, TOutput>(this TInput obj, Func<TInput, TOutput> func)
        {
            return func.Invoke(obj);
        }

        public static void Let<TInput>(this TInput obj, Action<TInput> action)
        {
            action.Invoke(obj);
        }

        public static TInput Apply<TInput>(this TInput obj, Action<TInput> action)
        {
            action.Invoke(obj);
            return obj;
        }
    }
}
