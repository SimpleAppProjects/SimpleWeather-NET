using System;
using System.Globalization;

namespace SimpleWeather.Utils
{
    public static class DateTimeUtils
    {
        public const string DATETIMEOFFSET_FORMAT = "dd.MM.yyyy HH:mm:ss zzzz";
        public const string ISO8601_DATETIME_FORMAT = "yyyy'-'MM'-'dd'T'HH':'mm':'ssZ";
        public const string TIME_OFFSET_FORMAT = "hh':'mm':'ss";

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

        public static String ToDateTimeOffsetFormat(this DateTimeOffset date)
        {
            return date.ToString(DATETIMEOFFSET_FORMAT, CultureInfo.InvariantCulture);
        }

        public static String ToISO8601Format(this DateTime date)
        {
            return date.ToString(ISO8601_DATETIME_FORMAT, CultureInfo.InvariantCulture);
        }

        public static String ToZoneOffsetFormat(this TimeSpan time)
        {
            if (time.CompareTo(TimeSpan.Zero) < 0)
                return "-" + time.ToString(TIME_OFFSET_FORMAT, CultureInfo.InvariantCulture);
            else
                return time.ToString(TIME_OFFSET_FORMAT, CultureInfo.InvariantCulture);
        }
    }
}