#if WINDOWS_UWP
using System;

namespace SimpleWeather.WeatherData
{
    public abstract partial class WeatherProviderImpl : IWeatherProviderImpl
    {
        // Utils Methods
        public virtual string GetBackgroundURI(Weather weather)
        {
            String icon = weather.condition.icon;
            String imgURI = null;

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
                    imgURI = ("ms-appx:///Assets/Backgrounds/RainyDay.jpg");
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
                    imgURI = ("ms-appx:///Assets/Backgrounds/RainyNight.jpg");
                    break;
                // Tornado / Hurricane / Thunderstorm / Tropical Storm
                case WeatherIcons.DAY_LIGHTNING:
                case WeatherIcons.DAY_THUNDERSTORM:
                    imgURI = ("ms-appx:///Assets/Backgrounds/Thunderstorm-Day.jpg");
                    break;

                case WeatherIcons.NIGHT_ALT_LIGHTNING:
                case WeatherIcons.NIGHT_ALT_THUNDERSTORM:
                case WeatherIcons.LIGHTNING:
                case WeatherIcons.THUNDERSTORM:
                    imgURI = ("ms-appx:///Assets/Backgrounds/Thunderstorm-Night.jpg");
                    break;

                case WeatherIcons.DAY_STORM_SHOWERS:
                case WeatherIcons.DAY_SLEET_STORM:
                case WeatherIcons.STORM_SHOWERS:
                case WeatherIcons.NIGHT_ALT_STORM_SHOWERS:
                case WeatherIcons.NIGHT_ALT_SLEET_STORM:
                case WeatherIcons.HAIL:
                case WeatherIcons.HURRICANE:
                case WeatherIcons.TORNADO:
                    imgURI = ("ms-appx:///Assets/Backgrounds/StormySky.jpg");
                    break;
                // Dust
                case WeatherIcons.DUST:
                case WeatherIcons.SANDSTORM:
                    imgURI = ("ms-appx:///Assets/Backgrounds/Dust.jpg");
                    break;
                // Foggy / Haze
                case WeatherIcons.DAY_FOG:
                case WeatherIcons.DAY_HAZE:
                case WeatherIcons.FOG:
                case WeatherIcons.NIGHT_FOG:
                case WeatherIcons.SMOG:
                case WeatherIcons.SMOKE:
                    imgURI = ("ms-appx:///Assets/Backgrounds/FoggySky.jpg");
                    break;
                // Snow / Snow Showers/Storm
                case WeatherIcons.DAY_SNOW:
                case WeatherIcons.DAY_SNOW_THUNDERSTORM:
                case WeatherIcons.NIGHT_ALT_SNOW:
                case WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM:
                case WeatherIcons.SNOW:
                    imgURI = ("ms-appx:///Assets/Backgrounds/Snow.jpg");
                    break;

                case WeatherIcons.SNOW_WIND:
                case WeatherIcons.DAY_SNOW_WIND:
                case WeatherIcons.NIGHT_ALT_SNOW_WIND:
                    imgURI = ("ms-appx:///Assets/Backgrounds/Snow-Windy.jpg");
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
                        imgURI = ("ms-appx:///Assets/Backgrounds/MostlyCloudy-Night.jpg");
                    else
                        imgURI = ("ms-appx:///Assets/Backgrounds/MostlyCloudy-Day.jpg");
                    break;
                // Partly Cloudy
                case WeatherIcons.DAY_SUNNY_OVERCAST:
                case WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY:
                    if (IsNight(weather))
                        imgURI = ("ms-appx:///Assets/Backgrounds/PartlyCloudy-Night.jpg");
                    else
                        imgURI = ("ms-appx:///Assets/Backgrounds/PartlyCloudy-Day.jpg");
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
                        imgURI = ("ms-appx:///Assets/Backgrounds/NightSky.jpg");
                    else
                        imgURI = ("ms-appx:///Assets/Backgrounds/DaySky.jpg");
                    break;
            }

            // Just in case
            if (String.IsNullOrWhiteSpace(imgURI))
            {
                // Set background based using sunset/rise times
                if (IsNight(weather))
                    imgURI = ("ms-appx:///Assets/Backgrounds/NightSky.jpg");
                else
                    imgURI = ("ms-appx:///Assets/Backgrounds/DaySky.jpg");
            }

            return imgURI;
        }
    }
}
#endif