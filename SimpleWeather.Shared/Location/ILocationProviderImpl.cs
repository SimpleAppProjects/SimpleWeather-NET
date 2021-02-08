using SimpleWeather.Controls;
using SimpleWeather.Utils;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SimpleWeather.Location
{
    public partial interface ILocationProviderImpl
    {
        string LocationAPI { get; }
        bool KeyRequired { get; }
        bool SupportsLocale { get; }
        bool NeedsLocationFromID { get; }
        bool NeedsLocationFromName { get; }
        bool NeedsLocationFromGeocoder { get; }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        Task<ObservableCollection<LocationQueryViewModel>> GetLocations(string ac_query, string weatherAPI);
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        Task<LocationQueryViewModel> GetLocation(WeatherUtils.Coordinate coordinate, string weatherAPI);
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        Task<LocationQueryViewModel> GetLocationFromID(LocationQueryViewModel model);
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        Task<LocationQueryViewModel> GetLocationFromName(LocationQueryViewModel model);
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        Task<bool> IsKeyValid(string key);
        string GetAPIKey();
        string LocaleToLangCode(string iso, string name);
        Task UpdateLocationData(LocationData location, string weatherAPI);
    }
}
