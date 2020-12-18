using SimpleWeather.Controls;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using Windows.UI;

namespace SimpleWeather.WeatherData
{
    public abstract partial class WeatherProviderImpl : IWeatherProviderImpl
    {
        // Variables
        public LocationProviderImpl LocationProvider { get; protected set; }
        public abstract string WeatherAPI { get; }
        public abstract bool SupportsWeatherLocale { get; }
        public abstract bool KeyRequired { get; }
        public abstract bool SupportsAlerts { get; }
        public abstract bool NeedsExternalAlertData { get; }

        // Methods
        // AutoCompleteQuery
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public Task<ObservableCollection<LocationQueryViewModel>> GetLocations(String ac_query)
        {
            return LocationProvider.GetLocations(ac_query, WeatherAPI);
        }
        // GeopositionQuery
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public Task<LocationQueryViewModel> GetLocation(WeatherUtils.Coordinate coordinate)
        {
            return LocationProvider.GetLocation(coordinate, WeatherAPI);
        }
        // Weather
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public abstract Task<Weather> GetWeather(String location_query, String country_code);
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public virtual Task<Weather> GetWeather(LocationData location)
        {
            return Task.Run(async () =>
            {
                if (location == null || location.query == null)
                    throw new WeatherException(WeatherUtils.ErrorStatus.Unknown);

                var weather = await AsyncTask.RunAsync(GetWeather(location.query, location.country_code));

                if (String.IsNullOrWhiteSpace(location.tz_long))
                {
                    if (!String.IsNullOrWhiteSpace(weather.location.tz_long))
                    {
                        location.tz_long = weather.location.tz_long;
                    }
                    else if (location.longitude != 0 && location.latitude != 0)
                    {
                        String tzId = await AsyncTask.RunAsync(TZDB.TZDBCache.GetTimeZone(location.latitude, location.longitude));
                        if (!String.IsNullOrWhiteSpace(tzId))
                            location.tz_long = tzId;
                    }

#if !UNIT_TEST
                    // Update DB here or somewhere else
                    await Settings.UpdateLocation(location);
#endif
                }

                if (String.IsNullOrWhiteSpace(weather.location.tz_long))
                    weather.location.tz_long = location.tz_long;

                if (String.IsNullOrWhiteSpace(weather.location.name))
                    weather.location.name = location.name;

                weather.location.latitude = (float)location.latitude;
                weather.location.longitude = (float)location.longitude;

                // Additional external data
                weather.condition.airQuality = await new AQICN.AQICNProvider().GetAirQualityData(location);

                return weather;
            });
        }
        // Alerts
        public virtual Task<ICollection<WeatherAlert>> GetAlerts(LocationData location)
        {
            if (LocationUtils.IsUS(location.country_code))
                return new NWS.NWSAlertProvider().GetAlerts(location);
            else
                return Task.FromResult<ICollection<WeatherAlert>>(null);
        }

        // KeyCheck
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public abstract Task<bool> IsKeyValid(String key);
        public abstract String GetAPIKey();

        // Utils Methods
        public Task UpdateLocationData(LocationData location)
        {
            return LocationProvider.UpdateLocationData(location, WeatherAPI);
        }
        public abstract String UpdateLocationQuery(Weather weather);
        public abstract String UpdateLocationQuery(LocationData location);
        public virtual String LocaleToLangCode(String iso, String name) { return "EN"; }
        public abstract String GetWeatherIcon(String icon);
        // Used in some providers for hourly forecast
        public virtual string GetWeatherIcon(bool isNight, String icon)
        {
            return GetWeatherIcon(icon);
        }

        /// <summary>
        /// Map the passed icon string to a localized weather condition string (if available)
        /// </summary>
        /// <param name="icon">The WeatherIcons to map</param>
        /// <returns>A localized weather condition string (if available); returns NULL if provider already supports localized data</returns>
        public virtual string GetWeatherCondition(String icon)
        {
            switch (icon)
            {
                case WeatherIcons.DAY_SUNNY:
                    return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_sunny");
                case WeatherIcons.NIGHT_CLEAR:
                    return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_clear");
                case WeatherIcons.DAY_SUNNY_OVERCAST:
                    return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_overcast");
                case WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY:
                    return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_partlycloudy");
                case WeatherIcons.DAY_CLOUDY:
                case WeatherIcons.NIGHT_ALT_CLOUDY:
                case WeatherIcons.CLOUDY:
                case WeatherIcons.NIGHT_ALT_CLOUDY_HIGH:
                case WeatherIcons.DAY_CLOUDY_HIGH:
                    return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_cloudy");
                case WeatherIcons.DAY_SPRINKLE:
                case WeatherIcons.NIGHT_ALT_SPRINKLE:
                case WeatherIcons.SPRINKLE:
                case WeatherIcons.DAY_SHOWERS:
                case WeatherIcons.NIGHT_ALT_SHOWERS:
                case WeatherIcons.SHOWERS:
                    return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_rainshowers");
                case WeatherIcons.DAY_THUNDERSTORM:
                case WeatherIcons.NIGHT_ALT_THUNDERSTORM:
                case WeatherIcons.THUNDERSTORM:
                case WeatherIcons.DAY_STORM_SHOWERS:
                case WeatherIcons.NIGHT_ALT_STORM_SHOWERS:
                    return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_tstorms");
                case WeatherIcons.DAY_SLEET:
                case WeatherIcons.NIGHT_ALT_SLEET:
                case WeatherIcons.SLEET:
                    return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_sleet");
                case WeatherIcons.DAY_SNOW:
                case WeatherIcons.NIGHT_ALT_SNOW:
                case WeatherIcons.SNOW:
                    return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_snow");
                case WeatherIcons.DAY_SNOW_WIND:
                case WeatherIcons.NIGHT_ALT_SNOW_WIND:
                case WeatherIcons.SNOW_WIND:
                    return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_heavysnow");
                case WeatherIcons.DAY_SNOW_THUNDERSTORM:
                case WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM:
                    return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_snow_tstorms");
                case WeatherIcons.HAIL:
                case WeatherIcons.DAY_HAIL:
                case WeatherIcons.NIGHT_ALT_HAIL:
                    return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_hail");
                case WeatherIcons.DAY_RAIN:
                case WeatherIcons.NIGHT_ALT_RAIN:
                case WeatherIcons.RAIN:
                    return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_rain");
                case WeatherIcons.DAY_FOG:
                case WeatherIcons.NIGHT_FOG:
                case WeatherIcons.FOG:
                    return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_fog");
                case WeatherIcons.DAY_SLEET_STORM:
                case WeatherIcons.NIGHT_ALT_SLEET_STORM:
                    return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_sleet_tstorms");
                case WeatherIcons.SNOWFLAKE_COLD:
                    return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_cold");
                case WeatherIcons.DAY_HOT:
                    return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_hot");
                case WeatherIcons.DAY_HAZE:
                    return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_haze");
                case WeatherIcons.SMOKE:
                    return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_smoky");
                case WeatherIcons.SANDSTORM:
                case WeatherIcons.DUST:
                    return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_dust");
                case WeatherIcons.TORNADO:
                    return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_tornado");
                case WeatherIcons.DAY_RAIN_MIX:
                case WeatherIcons.NIGHT_ALT_RAIN_MIX:
                case WeatherIcons.RAIN_MIX:
                    return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_rainandsnow");
                case WeatherIcons.DAY_WINDY:
                case WeatherIcons.WINDY:
                case WeatherIcons.DAY_CLOUDY_WINDY:
                case WeatherIcons.NIGHT_ALT_CLOUDY_WINDY:
                case WeatherIcons.DAY_CLOUDY_GUSTS:
                case WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS:
                case WeatherIcons.STRONG_WIND:
                    return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_windy");
                case WeatherIcons.HURRICANE:
                    return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_tropicalstorm");
                default:
                    return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_notavailable");
            }
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
            byte[] argb = null;
            String icon = weather.condition.icon;

            // Apply background based on weather condition
            switch (icon)
            {
                // Rain
                case WeatherIcons.DAY_RAIN:
                case WeatherIcons.DAY_RAIN_MIX:
                case WeatherIcons.DAY_RAIN_WIND:
                case WeatherIcons.DAY_SHOWERS:
                case WeatherIcons.DAY_SLEET:
                case WeatherIcons.DAY_SPRINKLE:
                    argb = new byte[4] { 0xff, 0x10, 0x20, 0x30 };
                    break;

                case WeatherIcons.NIGHT_ALT_HAIL:
                case WeatherIcons.NIGHT_ALT_RAIN:
                case WeatherIcons.NIGHT_ALT_RAIN_MIX:
                case WeatherIcons.NIGHT_ALT_RAIN_WIND:
                case WeatherIcons.NIGHT_ALT_SHOWERS:
                case WeatherIcons.NIGHT_ALT_SLEET:
                case WeatherIcons.NIGHT_ALT_SPRINKLE:
                case WeatherIcons.RAIN:
                case WeatherIcons.RAIN_MIX:
                case WeatherIcons.RAIN_WIND:
                case WeatherIcons.SHOWERS:
                case WeatherIcons.SLEET:
                case WeatherIcons.SPRINKLE:
                    argb = new byte[4] { 0xff, 0x18, 0x10, 0x10 };
                    break;
                // Tornado / Hurricane / Thunderstorm / Tropical Storm
                case WeatherIcons.DAY_LIGHTNING:
                case WeatherIcons.DAY_THUNDERSTORM:
                    argb = new byte[4] { 0xff, 0x28, 0x38, 0x48 };
                    break;

                case WeatherIcons.NIGHT_ALT_LIGHTNING:
                case WeatherIcons.NIGHT_ALT_THUNDERSTORM:
                case WeatherIcons.LIGHTNING:
                case WeatherIcons.THUNDERSTORM:
                    argb = new byte[4] { 0xff, 0x18, 0x18, 0x30 };
                    break;

                case WeatherIcons.DAY_STORM_SHOWERS:
                case WeatherIcons.DAY_SLEET_STORM:
                case WeatherIcons.STORM_SHOWERS:
                case WeatherIcons.NIGHT_ALT_STORM_SHOWERS:
                case WeatherIcons.NIGHT_ALT_SLEET_STORM:
                case WeatherIcons.HAIL:
                case WeatherIcons.HURRICANE:
                case WeatherIcons.TORNADO:
                    argb = new byte[4] { 0xff, 0x18, 0x28, 0x30 };
                    break;
                // Dust
                case WeatherIcons.DUST:
                case WeatherIcons.SANDSTORM:
                    argb = new byte[4] { 0xff, 0xb0, 0x68, 0x10 };
                    break;
                // Foggy / Haze
                case WeatherIcons.DAY_FOG:
                case WeatherIcons.DAY_HAZE:
                case WeatherIcons.FOG:
                case WeatherIcons.NIGHT_FOG:
                case WeatherIcons.SMOG:
                case WeatherIcons.SMOKE:
                    argb = new byte[4] { 0xff, 0x20, 0x20, 0x20 };
                    break;
                // Snow / Snow Showers/Storm
                case WeatherIcons.DAY_SNOW:
                case WeatherIcons.DAY_SNOW_THUNDERSTORM:
                case WeatherIcons.NIGHT_ALT_SNOW:
                case WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM:
                case WeatherIcons.SNOW:
                    argb = new byte[4] { 0xff, 0xb8, 0xd0, 0xf0 };
                    break;

                case WeatherIcons.SNOW_WIND:
                case WeatherIcons.DAY_SNOW_WIND:
                case WeatherIcons.NIGHT_ALT_SNOW_WIND:
                    argb = new byte[4] { 0xff, 0xe0, 0xe0, 0xe0 };
                    break;
                /* Ambigious weather conditions */
                // (Mostly) Cloudy
                case WeatherIcons.CLOUD:
                case WeatherIcons.CLOUDY:
                case WeatherIcons.CLOUDY_GUSTS:
                case WeatherIcons.CLOUDY_WINDY:
                case WeatherIcons.DAY_CLOUDY:
                case WeatherIcons.DAY_CLOUDY_GUSTS:
                case WeatherIcons.DAY_CLOUDY_HIGH:
                case WeatherIcons.DAY_CLOUDY_WINDY:
                case WeatherIcons.NIGHT_ALT_CLOUDY:
                case WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS:
                case WeatherIcons.NIGHT_ALT_CLOUDY_HIGH:
                case WeatherIcons.NIGHT_ALT_CLOUDY_WINDY:
                    if (IsNight(weather))
                        argb = new byte[4] { 0xff, 0x18, 0x20, 0x20 };
                    else
                        argb = new byte[4] { 0xff, 0x50, 0x80, 0xa8 };
                    break;
                // Partly Cloudy
                case WeatherIcons.DAY_SUNNY_OVERCAST:
                case WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY:
                    if (IsNight(weather))
                        argb = new byte[4] { 0xff, 0x18, 0x18, 0x20 };
                    else
                        argb = new byte[4] { 0xff, 0x88, 0xb0, 0xc8 };
                    break;

                case WeatherIcons.DAY_SUNNY:
                case WeatherIcons.NA:
                case WeatherIcons.NIGHT_CLEAR:
                case WeatherIcons.SNOWFLAKE_COLD:
                case WeatherIcons.DAY_HOT:
                case WeatherIcons.WINDY:
                case WeatherIcons.STRONG_WIND:
                default:
                    // Set background based using sunset/rise times
                    if (IsNight(weather))
                        argb = new byte[4] { 0xff, 0x18, 0x10, 0x18 };
                    else
                        argb = new byte[4] { 0xff, 0x20, 0xa8, 0xd8 };
                    break;
            }

            // Just in case
            if (argb == null)
            {
                // Set background based using sunset/rise times
                if (IsNight(weather))
                {
                    // Night background
                    argb = new byte[4] { 0xff, 0x18, 0x10, 0x18 };
                }
                else
                {
                    // set day bg
                    argb = new byte[4] { 0xff, 0x20, 0xa8, 0xd8 };
                }
            }

            return Color.FromArgb(argb[0], argb[1], argb[2], argb[3]);
        }
    }
}