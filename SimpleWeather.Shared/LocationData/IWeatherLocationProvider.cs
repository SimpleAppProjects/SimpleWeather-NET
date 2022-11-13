using SimpleWeather.Utils;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SimpleWeather.LocationData
{
    public partial interface IWeatherLocationProvider
    {
        string LocationAPI { get; }
        bool KeyRequired { get; }
        bool SupportsLocale { get; }
        bool NeedsLocationFromID { get; }
        bool NeedsLocationFromName { get; }
        bool NeedsLocationFromGeocoder { get; }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        Task<ObservableCollection<LocationQuery>> GetLocations(string ac_query, string weatherAPI);
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        Task<LocationQuery> GetLocation(WeatherUtils.Coordinate coordinate, string weatherAPI);
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        Task<LocationQuery> GetLocationFromID(LocationQuery model);
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        Task<LocationQuery> GetLocationFromName(LocationQuery model);
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        Task<bool> IsKeyValid(string key);
        string GetAPIKey();
        string LocaleToLangCode(string iso, string name);
        Task UpdateLocationData(LocationData location, string weatherAPI);
    }
}
