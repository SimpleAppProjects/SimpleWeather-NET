using SimpleWeather.ComponentModel;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Controls
{
    public class SunPhaseViewModel
    {
        public DateTime SunriseTime { get; }
        public DateTime SunsetTime { get; }

        public string Sunrise { get; }
        public string Sunset { get; }

        public SunPhaseViewModel(Astronomy astronomy)
        {
            SunriseTime = astronomy.sunrise;
            SunsetTime = astronomy.sunset;

            Sunrise = astronomy.sunrise.ToString("t", CultureInfo.InvariantCulture);
            Sunset = astronomy.sunset.ToString("t", CultureInfo.InvariantCulture);
        }

        public override bool Equals(object obj)
        {
            return obj is SunPhaseViewModel model &&
                   SunriseTime == model.SunriseTime &&
                   SunsetTime == model.SunsetTime &&
                   Sunrise == model.Sunrise &&
                   Sunset == model.Sunset;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SunriseTime, SunsetTime, Sunrise, Sunset);
        }
    }
}
