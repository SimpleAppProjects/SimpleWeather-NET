using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using SimpleWeather.Controls;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;

namespace SimpleWeather.Location
{
    public abstract partial class LocationProviderImpl : ILocationProviderImpl
    {
        // Variables
        public abstract bool KeyRequired { get; }
        public abstract bool SupportsWeatherLocale { get; }

        // Methods
        // AutoCompleteQuery
        public abstract Task<ObservableCollection<LocationQueryViewModel>> GetLocations(String ac_query, String weatherAPI);
        // GeopositionQuery
        public abstract Task<LocationQueryViewModel> GetLocation(WeatherUtils.Coordinate coordinate, String weatherAPI);
        public abstract Task<LocationQueryViewModel> GetLocation(string query, String weatherAPI);

        // KeyCheck
        public abstract Task<bool> IsKeyValid(String key);
        public abstract String GetAPIKey();

        // Utils Methods
        public virtual async Task UpdateLocationData(WeatherData.LocationData location, String weatherAPI)
        {
            var qview = await GetLocation(location.query, weatherAPI);

            if (qview != null && !String.IsNullOrWhiteSpace(qview.LocationQuery))
            {
                location.name = qview.LocationName;
                location.latitude = qview.LocationLat;
                location.longitude = qview.LocationLong;
                location.tz_long = qview.LocationTZ_Long;

                // Update DB here or somewhere else
                await Settings.UpdateLocation(location);
            }
        }

        public virtual String LocaleToLangCode(String iso, String name) { return "EN"; }
    }
}
