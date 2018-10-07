using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleWeather.Utils
{
    public class DateTimeUtils
    {
        public static TimeSpan MaxTimeOfDay()
        {
            return new TimeSpan(0, 23, 59, 59, 999);
        }

        public static DateTime GetClosestWeekday(DayOfWeek day)
        {
            var today = new DateTime(DateTime.Today.Ticks, DateTimeKind.Utc);

            var nextWeekday = GetNextWeekday(today, day);
            var prevWeekday = GetPrevWeekday(today, day);

            if ((nextWeekday - today) < (today - prevWeekday))
                return nextWeekday;
            else
                return prevWeekday;
        }

        // Functionality thanks to: https://stackoverflow.com/questions/6346119/datetime-get-next-tuesday
        public static DateTime GetNextWeekday(DateTime start, DayOfWeek day)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }

        public static DateTime GetPrevWeekday(DateTime start, DayOfWeek day)
        {
            // The (... - 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToRemove = -(((int)start.DayOfWeek - (int)day + 7) % 7);
            return start.AddDays(daysToRemove);
        }
    }
}
