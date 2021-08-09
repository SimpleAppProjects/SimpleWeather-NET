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
        public TimeSpan SunriseTime { get; }
        public TimeSpan SunsetTime { get; }

        public string Sunrise { get; }
        public string Sunset { get; }

        public SunPhaseViewModel(Astronomy astronomy)
        {
            SunriseTime = astronomy.sunrise.TimeOfDay;
            SunsetTime = astronomy.sunset.TimeOfDay;

            Sunrise = astronomy.sunrise.ToString("t", CultureInfo.InvariantCulture);
            Sunset = astronomy.sunset.ToString("t", CultureInfo.InvariantCulture);
        }
    }
}
