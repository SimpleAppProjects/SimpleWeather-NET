using SimpleWeather.LocationData;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SimpleWeather.WeatherData
{
    public partial interface IWeatherProvider
    {
        String WeatherAPI { get; }
        IWeatherLocationProvider LocationProvider { get; }
        bool KeyRequired { get; }
        bool SupportsWeatherLocale { get; }
        bool SupportsAlerts { get; }
        bool NeedsExternalAlertData { get; }

        bool IsRegionSupported(LocationData.LocationData location);
        bool IsRegionSupported(LocationData.LocationQuery location);

        int HourlyForecastInterval { get; }

        AuthType AuthType { get; }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        Task<ObservableCollection<LocationQuery>> GetLocations(string ac_query);
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        Task<LocationQuery> GetLocation(WeatherUtils.Coordinate coordinate);
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        Task<Weather> GetWeather(LocationData.LocationData location);
        Task<ICollection<WeatherAlert>> GetAlerts(LocationData.LocationData location);
        string GetWeatherIcon(string icon);
        string GetWeatherIcon(bool isNight, string icon);
        string GetWeatherCondition(string icon);
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        Task<bool> IsKeyValid(string key);
        string GetAPIKey();
        bool IsNight(Weather weather);
        string LocaleToLangCode(string iso, string name);
        Task UpdateLocationData(LocationData.LocationData location);
        string UpdateLocationQuery(Weather weather);
        string UpdateLocationQuery(LocationData.LocationData location);
    }
}