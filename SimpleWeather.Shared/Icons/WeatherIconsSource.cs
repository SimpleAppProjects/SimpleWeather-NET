using SimpleWeather.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Icons
{
    public static class WeatherIconsSource
    {
        public const string WeatherIconsEF = "wi-erik-flowers";

        public readonly static IReadOnlyList<ComboBoxItem> WeatherIconSources = new List<ComboBoxItem>
        {
            new ComboBoxItem("Weather Icons", WeatherIconsEF)
        };
    }
}
