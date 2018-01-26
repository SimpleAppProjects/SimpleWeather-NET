using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SimpleWeather.Controls;
using SimpleWeather.Utils;

namespace SimpleWeather.WeatherData
{
    public partial interface IWeatherProviderImpl
    {
        bool KeyRequired { get; }
        bool SupportsWeatherLocale { get; }

        Task<LocationQueryViewModel> GetLocation(WeatherUtils.Coordinate coordinate);
        Task<ObservableCollection<LocationQueryViewModel>> GetLocations(string ac_query);
        Task<Weather> GetWeather(string location_query);
        Task<Weather> GetWeather(LocationData location);
        string GetWeatherIcon(string icon);
        string GetWeatherIcon(bool isNight, string icon);
        Task<bool> IsKeyValid(string key);
        bool IsNight(Weather weather);
        string LocaleToLangCode(string iso, string name);
        Task UpdateLocationData(LocationData location);
        Task<string> UpdateLocationQuery(Weather weather);
        Task<string> UpdateLocationQuery(LocationData location);
    }
}