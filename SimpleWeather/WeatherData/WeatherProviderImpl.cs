using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SimpleWeather.Controls;
using SimpleWeather.Utils;

namespace SimpleWeather.WeatherData
{
    public abstract partial class WeatherProviderImpl : IWeatherProviderImpl
    {
        // Variables
        public abstract bool SupportsWeatherLocale { get; }
        public abstract bool KeyRequired { get; }
        public abstract bool NeedsExternalLocationData { get; }

        // Methods
        // AutoCompleteQuery
        public abstract Task<ObservableCollection<LocationQueryViewModel>> GetLocations(String ac_query);
        // GeopositionQuery
        public abstract Task<LocationQueryViewModel> GetLocation(WeatherUtils.Coordinate coordinate);
        // Weather
        public abstract Task<Weather> GetWeather(String location_query);
        public virtual async Task<Weather> GetWeather(LocationData location)
        {
            var weather = await GetWeather(location.query);

            weather.location.name = location.name;
            weather.location.tz_offset = location.tz_offset;
            weather.location.tz_short = location.tz_short;

            return weather;
        }

        // KeyCheck
        public abstract Task<bool> IsKeyValid(String key);

        // Utils Methods
        public abstract Task<String> UpdateLocationQuery(Weather weather);
        public virtual String LocaleToLangCode(String iso, String name) { return "EN"; }
        public abstract String GetWeatherIcon(String icon);
        public virtual string GetWeatherIcon(bool isNight, String icon)
        {
            return GetWeatherIcon(icon);
        }

        public abstract bool IsNight(Weather weather);
    }
}