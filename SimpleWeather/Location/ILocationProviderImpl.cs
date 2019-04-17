using SimpleWeather.Controls;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Location
{
    public partial interface ILocationProviderImpl
    {
        string LocationAPI { get; }
        bool KeyRequired { get; }
        bool SupportsWeatherLocale { get; }

        Task<ObservableCollection<LocationQueryViewModel>> GetLocations(string ac_query, string weatherAPI);
        Task<LocationQueryViewModel> GetLocation(WeatherUtils.Coordinate coordinate, string weatherAPI);
        Task<bool> IsKeyValid(string key);
        string GetAPIKey();
        string LocaleToLangCode(string iso, string name);
        Task UpdateLocationData(LocationData location, string weatherAPI);
    }
}
