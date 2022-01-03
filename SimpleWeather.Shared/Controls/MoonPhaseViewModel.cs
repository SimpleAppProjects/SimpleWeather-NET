using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Windows.System.UserProfile;

namespace SimpleWeather.Controls
{
    public class MoonPhaseViewModel
    {
        public DetailItemViewModel MoonPhase { get; set; }
        public MoonPhase.MoonPhaseType PhaseType { get; set; }

        public DateTime MoonriseTime { get; }
        public DateTime MoonsetTime { get; }

        public string Moonrise { get; }
        public string Moonset { get; }

        public MoonPhaseViewModel(Astronomy astronomy)
        {
            var culture = CultureUtils.UserCulture;

            if (astronomy.moonrise != null && astronomy.moonrise != DateTime.MinValue)
            {
                MoonriseTime = astronomy.moonrise;
                Moonrise = astronomy.moonrise.ToString("t", culture);
            }

            if (astronomy.moonset != null && astronomy.moonset != DateTime.MinValue)
            {
                MoonsetTime = astronomy.moonset;
                Moonset = astronomy.moonset.ToString("t", culture);
            }

            if (astronomy.moonphase != null)
            {
                PhaseType = astronomy.moonphase.phase;
                MoonPhase = new DetailItemViewModel(astronomy.moonphase.phase);
            }
        }
    }
}
