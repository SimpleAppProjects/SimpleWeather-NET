using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Utils
{
    public static class CustomExtensions
    {
        /* Number extensions */
        public static int RoundToInt(this float @num)
        {
            return (int)MathF.Round(num);
        }

        public static float Times(this float @num, float value)
        {
            return num * value;
        }

        public static float Div(this float @num, float value)
        {
            return num / value;
        }

        /* Kotlin-like extensions */
        public static TOutput Let<TInput, TOutput>(this TInput obj, Func<TInput, TOutput> func)
        {
            return func.Invoke(obj);
        }

        public static void Let<TInput>(this TInput obj, Action<TInput> action)
        {
            action.Invoke(obj);
        }
    }
}
