using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.TZDB;
using SimpleWeather.Weather_API.Utils;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SimpleWeather.LocationData
{
    public abstract partial class WeatherLocationProviderImpl : IWeatherLocationProvider, IRateLimitedRequest
    {
        protected readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();
        protected readonly ITZDBService TZDBService = Ioc.Default.GetService<ITZDBService>();

        // Variables
        public abstract string LocationAPI { get; }
        public abstract bool KeyRequired { get; }
        public abstract bool SupportsLocale { get; }
        public virtual bool NeedsLocationFromID => false;
        public virtual bool NeedsLocationFromName => false;
        public virtual bool NeedsLocationFromGeocoder => false;

        public virtual long GetRetryTime() => 5000;

        // Methods
        // AutoCompleteQuery
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public abstract Task<ObservableCollection<LocationQuery>> GetLocations(String ac_query, String weatherAPI);
        // GeopositionQuery
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public abstract Task<LocationQuery> GetLocation(WeatherUtils.Coordinate coordinate, String weatherAPI);

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public abstract Task<LocationQuery> GetLocationFromID(LocationQuery model);

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public abstract Task<LocationQuery> GetLocationFromName(LocationQuery model);

        // KeyCheck
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public abstract Task<bool> IsKeyValid(String key);
        public abstract String GetAPIKey();

        protected string GetProviderKey()
        {
            if (SettingsManager.UsePersonalKeys[LocationAPI])
            {
                return SettingsManager.APIKeys[LocationAPI];
            }
            else
            {
                return GetAPIKey();
            }
        }

        // Utils Methods
        public virtual async Task UpdateLocationData(LocationData location, String weatherAPI)
        {
            LocationQuery qview = null;
            try
            {
                qview = await GetLocation(new WeatherUtils.Coordinate(location), weatherAPI);
            }
            catch (WeatherException wEx)
            {
                Logger.WriteLine(LoggerLevel.Error, wEx, "LocationProviderImpl: UpdateLocationData: WeatherException!");
            }

            if (qview != null && !String.IsNullOrWhiteSpace(qview.Location_Query))
            {
                location.name = qview.LocationName;
                location.latitude = qview.LocationLat;
                location.longitude = qview.LocationLong;
                location.tz_long = qview.LocationTZLong;
                if (String.IsNullOrWhiteSpace(qview.LocationTZLong) && location.latitude != 0 && location.longitude != 0)
                {
                    String tzId = await TZDBService.GetTimeZone(location.latitude, location.longitude);
                    if (!Equals("unknown", tzId))
                        location.tz_long = tzId;
                }
                location.locationSource = qview.LocationSource;

                // Update DB here or somewhere else
                await SettingsManager.UpdateLocation(location);
            }
        }

        public virtual String LocaleToLangCode(String iso, String name) { return "EN"; }
    }
}