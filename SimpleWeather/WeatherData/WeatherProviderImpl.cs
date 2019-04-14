using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SimpleWeather.Controls;
using SimpleWeather.Utils;
using System.Collections.Generic;
using Windows.UI;
using System.Globalization;
using SimpleWeather.Location;

namespace SimpleWeather.WeatherData
{
    public abstract partial class WeatherProviderImpl : IWeatherProviderImpl
    {
        protected LocationProviderImpl locProvider;

        // Variables
        public LocationProviderImpl LocationProvider { get { return locProvider; } }
        public abstract string WeatherAPI { get; }
        public abstract bool SupportsWeatherLocale { get; }
        public abstract bool KeyRequired { get; }
        public abstract bool SupportsAlerts { get; }
        public abstract bool NeedsExternalAlertData { get; }

        // Methods
        // AutoCompleteQuery
        public async Task<ObservableCollection<LocationQueryViewModel>> GetLocations(String ac_query)
        {
            return await locProvider.GetLocations(ac_query, WeatherAPI);
        }
        // GeopositionQuery
        public async Task<LocationQueryViewModel> GetLocation(WeatherUtils.Coordinate coordinate)
        {
            return await locProvider.GetLocation(coordinate, WeatherAPI);
        }
        public async Task<LocationQueryViewModel> GetLocation(string query)
        {
            return await locProvider.GetLocation(query, WeatherAPI);
        }
        // Weather
        public abstract Task<Weather> GetWeather(String location_query);
        public virtual async Task<Weather> GetWeather(LocationData location)
        {
            if (location == null || location.query == null)
                throw new WeatherException(WeatherUtils.ErrorStatus.Unknown);

            var weather = await GetWeather(location.query);

            if (SupportsAlerts && NeedsExternalAlertData)
                weather.weather_alerts = await GetAlerts(location);

            if (String.IsNullOrWhiteSpace(weather.location.name))
                weather.location.name = location.name;

            weather.location.latitude = location.latitude.ToString(CultureInfo.InvariantCulture);
            weather.location.longitude = location.longitude.ToString(CultureInfo.InvariantCulture);
            weather.location.tz_short = location.tz_short;
            weather.location.tz_offset = location.tz_offset;

            if (String.IsNullOrWhiteSpace(weather.location.tz_long))
                weather.location.tz_long = location.tz_long;

            return weather;
        }
        // Alerts
        public virtual async Task<List<WeatherAlert>> GetAlerts(LocationData location)
        {
            if ("US".Equals(location.country_code))
                return await new NWS.NWSAlertProvider().GetAlerts(location);
            else
                return null;
        }

        // KeyCheck
        public abstract Task<bool> IsKeyValid(String key);
        public abstract String GetAPIKey();

        // Utils Methods
        public async Task UpdateLocationData(LocationData location)
        {
            await locProvider.UpdateLocationData(location, WeatherAPI);
        }
        public abstract Task<String> UpdateLocationQuery(Weather weather);
        public abstract Task<String> UpdateLocationQuery(LocationData location);
        public virtual String LocaleToLangCode(String iso, String name) { return "EN"; }
        public abstract String GetWeatherIcon(String icon);
        // Used in some providers for hourly forecast
        public virtual string GetWeatherIcon(bool isNight, String icon)
        {
            return GetWeatherIcon(icon);
        }

        // Used for current condition
        public virtual bool IsNight(Weather weather)
        {
            bool isNight = false;

            String icon = weather.condition.icon;

            switch (icon)
            {
                case WeatherIcons.NIGHT_ALT_HAIL:
                case WeatherIcons.NIGHT_ALT_LIGHTNING:
                case WeatherIcons.NIGHT_ALT_RAIN:
                case WeatherIcons.NIGHT_ALT_RAIN_MIX:
                case WeatherIcons.NIGHT_ALT_RAIN_WIND:
                case WeatherIcons.NIGHT_ALT_SHOWERS:
                case WeatherIcons.NIGHT_ALT_SLEET:
                case WeatherIcons.NIGHT_ALT_SLEET_STORM:
                case WeatherIcons.NIGHT_ALT_SNOW:
                case WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM:
                case WeatherIcons.NIGHT_ALT_SNOW_WIND:
                case WeatherIcons.NIGHT_ALT_SPRINKLE:
                case WeatherIcons.NIGHT_ALT_STORM_SHOWERS:
                case WeatherIcons.NIGHT_ALT_THUNDERSTORM:
                case WeatherIcons.NIGHT_FOG:
                case WeatherIcons.NIGHT_CLEAR:
                case WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY:
                case WeatherIcons.NIGHT_ALT_CLOUDY:
                case WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS:
                case WeatherIcons.NIGHT_ALT_CLOUDY_WINDY:
                case WeatherIcons.NIGHT_ALT_CLOUDY_HIGH:
                    isNight = true;
                    break;
            }

            return isNight;
        }

        public virtual Color GetWeatherBackgroundColor(Weather weather)
        {
            byte[] rgb = null;
            String icon = weather.condition.icon;

            // Apply background based on weather condition
            switch (icon)
            {
                // Rain/Snow/Sleet/Hail/Storms
                case WeatherIcons.DAY_HAIL:
                case WeatherIcons.DAY_LIGHTNING:
                case WeatherIcons.DAY_RAIN:
                case WeatherIcons.DAY_RAIN_MIX:
                case WeatherIcons.DAY_RAIN_WIND:
                case WeatherIcons.DAY_SHOWERS:
                case WeatherIcons.DAY_SLEET:
                case WeatherIcons.DAY_SLEET_STORM:
                case WeatherIcons.DAY_SNOW:
                case WeatherIcons.DAY_SNOW_THUNDERSTORM:
                case WeatherIcons.DAY_SNOW_WIND:
                case WeatherIcons.DAY_SPRINKLE:
                case WeatherIcons.DAY_STORM_SHOWERS:
                case WeatherIcons.DAY_THUNDERSTORM:
                case WeatherIcons.NIGHT_ALT_HAIL:
                case WeatherIcons.NIGHT_ALT_LIGHTNING:
                case WeatherIcons.NIGHT_ALT_RAIN:
                case WeatherIcons.NIGHT_ALT_RAIN_MIX:
                case WeatherIcons.NIGHT_ALT_RAIN_WIND:
                case WeatherIcons.NIGHT_ALT_SHOWERS:
                case WeatherIcons.NIGHT_ALT_SLEET:
                case WeatherIcons.NIGHT_ALT_SLEET_STORM:
                case WeatherIcons.NIGHT_ALT_SNOW:
                case WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM:
                case WeatherIcons.NIGHT_ALT_SNOW_WIND:
                case WeatherIcons.NIGHT_ALT_SPRINKLE:
                case WeatherIcons.NIGHT_ALT_STORM_SHOWERS:
                case WeatherIcons.NIGHT_ALT_THUNDERSTORM:
                case WeatherIcons.HAIL:
                case WeatherIcons.RAIN:
                case WeatherIcons.RAIN_MIX:
                case WeatherIcons.RAIN_WIND:
                case WeatherIcons.SHOWERS:
                case WeatherIcons.SLEET:
                case WeatherIcons.SNOW:
                case WeatherIcons.SPRINKLE:
                case WeatherIcons.STORM_SHOWERS:
                case WeatherIcons.THUNDERSTORM:
                case WeatherIcons.SNOW_WIND:
                case WeatherIcons.LIGHTNING:
                    // lighter than night color + cloudiness
                    rgb = new byte[3] { 53, 67, 116 };
                    break;
                // Dust
                case WeatherIcons.DUST:
                // Foggy / Haze
                case WeatherIcons.DAY_FOG:
                case WeatherIcons.DAY_HAZE:
                case WeatherIcons.NIGHT_FOG:
                case WeatherIcons.FOG:
                    // add haziness
                    rgb = new byte[3] { 143, 163, 196 };
                    break;
                // Night
                case WeatherIcons.NIGHT_CLEAR:
                case WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY:
                    // Night background
                    rgb = new byte[3] { 26, 36, 74 };
                    break;
                // Mostly/Partly Cloudy
                case WeatherIcons.DAY_CLOUDY:
                case WeatherIcons.DAY_CLOUDY_GUSTS:
                case WeatherIcons.DAY_CLOUDY_WINDY:
                case WeatherIcons.DAY_CLOUDY_HIGH:
                case WeatherIcons.DAY_SUNNY_OVERCAST:
                case WeatherIcons.NIGHT_ALT_CLOUDY:
                case WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS:
                case WeatherIcons.NIGHT_ALT_CLOUDY_WINDY:
                case WeatherIcons.NIGHT_ALT_CLOUDY_HIGH:
                case WeatherIcons.CLOUD:
                case WeatherIcons.CLOUDY:
                case WeatherIcons.CLOUDY_GUSTS:
                case WeatherIcons.CLOUDY_WINDY:
                    if (IsNight(weather))
                    {
                        // Add night background plus cloudiness
                        rgb = new byte[3] { 16, 37, 67 };
                    }
                    else
                    {
                        // add day bg + cloudiness
                        rgb = new byte[3] { 119, 148, 196 };
                    }
                    break;
                case WeatherIcons.NA:
                default:
                    // Set background based using sunset/rise times
                    if (IsNight(weather))
                    {
                        // Night background
                        rgb = new byte[3] { 26, 36, 74 };
                    }
                    else
                    {
                        // set day bg
                        rgb = new byte[3] { 72, 116, 191 };
                    }
                    break;
            }

            // Just in case
            if (rgb == null)
            {
                // Set background based using sunset/rise times
                if (IsNight(weather))
                {
                    // Night background
                    rgb = new byte[3] { 26, 36, 74 };
                }
                else
                {
                    // set day bg
                    rgb = new byte[3] { 72, 116, 191 };
                }
            }

            return Color.FromArgb(255, rgb[0], rgb[1], rgb[2]);
        }
    }
}