using SimpleWeather.WeatherData;
using System;
using System.Globalization;

namespace SimpleWeather.Common.Controls
{
    public class SunPhaseViewModel
    {
        public DateTime SunriseTime { get; }
        public DateTime SunsetTime { get; }

        public string Sunrise { get; }
        public string Sunset { get; }

        public TimeSpan TZOffset { get; }

        public SunPhaseViewModel(Astronomy astronomy, TimeSpan offset)
        {
            SunriseTime = astronomy.sunrise;
            SunsetTime = astronomy.sunset;

            Sunrise = astronomy.sunrise.ToString("t", CultureInfo.InvariantCulture);
            Sunset = astronomy.sunset.ToString("t", CultureInfo.InvariantCulture);

            TZOffset = offset;
        }

        public override bool Equals(object obj)
        {
            return obj is SunPhaseViewModel model &&
                   Sunrise == model.Sunrise &&
                   Sunset == model.Sunset &&
                   TZOffset.Equals(model.TZOffset);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Sunrise, Sunset, TZOffset);
        }
    }
}
