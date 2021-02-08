using SimpleWeather.Controls;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        Task<ObservableCollection<LocationQueryViewModel>> GetLocations(string ac_query);
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        Task<LocationQueryViewModel> GetLocation(WeatherUtils.Coordinate coordinate);
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        Task<Weather> GetWeather(string location_query, string country_code);
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        Task<Weather> GetWeather(LocationData location);
        Task<ICollection<WeatherAlert>> GetAlerts(LocationData location);
        string GetWeatherIcon(string icon);
        string GetWeatherIcon(bool isNight, string icon);
        string GetWeatherCondition(string icon);
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        Task<bool> IsKeyValid(string key);
        string GetAPIKey();
        bool IsNight(Weather weather);
        string LocaleToLangCode(string iso, string name);
        Task UpdateLocationData(LocationData location);
        string UpdateLocationQuery(Weather weather);
        string UpdateLocationQuery(LocationData location);
    }
}