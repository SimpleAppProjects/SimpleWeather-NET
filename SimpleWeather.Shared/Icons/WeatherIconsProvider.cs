using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Icons
{
    public sealed partial class WeatherIconsProvider : WeatherIconProvider
    {
        public const string KEY = "wi-erik-flowers";

        public override string Key => KEY;

        public override string DisplayName => "Weather Icons";

        public override string AuthorName => "Erik Flowers";

        public override Uri AttributionLink => new Uri("https://erikflowers.github.io/weather-icons/");

        public override bool IsFontIcon => true;
    }
}
