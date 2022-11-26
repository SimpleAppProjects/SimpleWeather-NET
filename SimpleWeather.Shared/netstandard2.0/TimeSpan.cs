#if NETSTANDARD2_0
using System;

namespace SimpleWeather.Utils
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan Divide(this TimeSpan timeSpan, double divisor)
        {
            if (double.IsNaN(divisor))
            {
                throw new ArgumentException("NaN", nameof(divisor));
            }

            double ticks = Math.Round(timeSpan.Ticks / divisor);
            return IntervalFromDoubleTicks(ticks);
        }

        private static TimeSpan IntervalFromDoubleTicks(double ticks)
        {
            if ((ticks > long.MaxValue) || (ticks < long.MinValue) || double.IsNaN(ticks))
                throw new OverflowException("TimeSpan too long");
            if (ticks == long.MaxValue)
                return TimeSpan.MaxValue;
            return new TimeSpan((long)ticks);
        }
    }
}
#endif