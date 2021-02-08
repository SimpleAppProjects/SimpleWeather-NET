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
        public String Title { get; set; }
        public DetailItemViewModel MoonPhase { get; set; }
        public MoonPhase.MoonPhaseType PhaseType { get; set; }

        public MoonPhaseViewModel(MoonPhase moonPhase)
        {
            PhaseType = moonPhase.phase;
            MoonPhase = new DetailItemViewModel(moonPhase.phase);
            Title = MoonPhase.Label;
        }
    }
}
