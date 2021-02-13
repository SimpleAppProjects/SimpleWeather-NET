using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Icons
{
    public abstract partial class WeatherIconProvider : IWeatherIconsProvider
    {
        public abstract string Key { get; }
        public abstract string DisplayName { get; }
        public abstract string AuthorName { get; }
        public abstract Uri AttributionLink { get; }
        public abstract bool IsFontIcon { get; }
    }
}
