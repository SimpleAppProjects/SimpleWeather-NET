using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SimpleWeather.Controls;
using SimpleWeather.Utils;
#if WINDOWS_UWP
using Windows.UI;
#elif __ANDROID__
using Android.Graphics;
#endif

namespace SimpleWeather.WeatherData
{
    public abstract partial class WeatherProviderImpl : IWeatherProviderImpl
    {
        // Variables
        public abstract bool SupportsWeatherLocale { get; }
        public abstract bool KeyRequired { get; }
        public abstract bool NeedsExternalLocationData { get; }

        // Methods
        // AutoCompleteQuery
        public abstract Task<ObservableCollection<LocationQueryViewModel>> GetLocations(String ac_query);
        // GeopositionQuery
        public abstract Task<LocationQueryViewModel> GetLocation(WeatherUtils.Coordinate coordinate);
        // Weather
        public abstract Task<Weather> GetWeather(String location_query);
        public virtual async Task<Weather> GetWeather(LocationData location)
        {
            var weather = await GetWeather(location.query);

            weather.location.name = location.name;
            weather.location.tz_offset = location.tz_offset;
            weather.location.tz_short = location.tz_short;

            return weather;
        }

        // KeyCheck
        public abstract Task<bool> IsKeyValid(String key);

        // Utils Methods
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

#if WINDOWS_UWP
        public virtual Color GetWeatherBackgroundColor(Weather weather)
#elif __ANDROID__
        public virtual Color GetWeatherBackgroundColor(Weather weather)
#endif
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

#if WINDOWS_UWP
            return Color.FromArgb(255, rgb[0], rgb[1], rgb[2]);
#elif __ANDROID__
            return new Color(rgb[0], rgb[1], rgb[2]);
#endif
        }
    }
}