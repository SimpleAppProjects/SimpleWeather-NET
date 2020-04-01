using GeoTimeZone;
using SimpleWeather.Controls;
using SimpleWeather.Utils;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SimpleWeather.Location
{
    public abstract partial class LocationProviderImpl : ILocationProviderImpl
    {
        // Variables
        public abstract string LocationAPI { get; }
        public abstract bool KeyRequired { get; }
        public abstract bool SupportsWeatherLocale { get; }

        // Methods
        // AutoCompleteQuery
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public abstract Task<ObservableCollection<LocationQueryViewModel>> GetLocations(String ac_query, String weatherAPI);
        // GeopositionQuery
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public abstract Task<LocationQueryViewModel> GetLocation(WeatherUtils.Coordinate coordinate, String weatherAPI);

        // KeyCheck
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public abstract Task<bool> IsKeyValid(String key);
        public abstract String GetAPIKey();

        // Utils Methods
        public virtual Task UpdateLocationData(LocationData location, String weatherAPI)
        {
            return Task.Run(async () =>
            {
                LocationQueryViewModel qview = null;
                try
                {
                    qview = await GetLocation(new WeatherUtils.Coordinate(location), weatherAPI);
                }
                catch (WeatherException wEx)
                {
                    Logger.WriteLine(LoggerLevel.Error, wEx, "LocationProviderImpl: UpdateLocationData: WeatherException!");
                }

                if (qview != null && !String.IsNullOrWhiteSpace(qview.LocationQuery))
                {
                    location.name = qview.LocationName;
                    location.latitude = qview.LocationLat;
                    location.longitude = qview.LocationLong;
                    location.tz_long = qview.LocationTZLong;
                    if (String.IsNullOrEmpty(qview.LocationTZLong) && location.longitude != 0 && location.latitude != 0)
                    {
                        String tzId = await TZDB.TZDBCache.GetTimeZone(location.latitude, location.longitude);
                        if (!String.IsNullOrWhiteSpace(tzId))
                            location.tz_long = tzId;
                    }
                    location.locationSource = qview.LocationSource;

                    // Update DB here or somewhere else
                    await Settings.UpdateLocation(location);
                }
            });
        }

        public virtual String LocaleToLangCode(String iso, String name) { return "EN"; }
    }
}