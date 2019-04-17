using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SimpleWeather.Controls;
using SimpleWeather.Location;
using SimpleWeather.Utils;

namespace SimpleWeather.WeatherData
{
    public partial interface IWeatherProviderImpl
    {
        String WeatherAPI { get; }
        LocationProviderImpl LocationProvider { get; }
        bool KeyRequired { get; }
        bool SupportsWeatherLocale { get; }
        bool SupportsAlerts { get; }
        bool NeedsExternalAlertData { get; }

        Task<ObservableCollection<LocationQueryViewModel>> GetLocations(string ac_query);
        Task<LocationQueryViewModel> GetLocation(WeatherUtils.Coordinate coordinate);
        Task<Weather> GetWeather(string location_query);
        Task<Weather> GetWeather(LocationData location);
        Task<List<WeatherAlert>> GetAlerts(LocationData location);
        string GetWeatherIcon(string icon);
        string GetWeatherIcon(bool isNight, string icon);
        Task<bool> IsKeyValid(string key);
        string GetAPIKey();
        bool IsNight(Weather weather);
        string LocaleToLangCode(string iso, string name);
        Task UpdateLocationData(LocationData location);
        Task<string> UpdateLocationQuery(Weather weather);
        Task<string> UpdateLocationQuery(LocationData location);
    }
}