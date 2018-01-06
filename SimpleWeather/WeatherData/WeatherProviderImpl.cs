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

        // Methods
        // AutoCompleteQuery
        public abstract Task<ObservableCollection<LocationQueryViewModel>> GetLocations(String ac_query);
        // GeopositionQuery
        public abstract Task<LocationQueryViewModel> GetLocation(WeatherUtils.Coordinate coordinate);
        // Weather
        public abstract Task<Weather> GetWeather(String location_query);
        // KeyCheck
        public abstract Task<bool> IsKeyValid(String key);

        // Utils Methods
        public abstract Task<String> UpdateLocationQuery(Weather weather);
        public virtual String LocaleToLangCode(String iso, String name) { return "EN"; }
        public abstract String GetWeatherIcon(String icon);
        public abstract bool IsNight(Weather weather);
    }
}