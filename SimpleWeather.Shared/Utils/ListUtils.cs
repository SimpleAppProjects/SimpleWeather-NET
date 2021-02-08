using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleWeather.Utils
{
    public static class ListUtils
    {
        public static void EnsureCapacity<T>(this List<T> @list, int minimumCapacity)
        {
            if (list.Capacity < minimumCapacity && minimumCapacity >= list.Count)
            {
                list.Capacity = minimumCapacity;
            }
        }
    }
}
