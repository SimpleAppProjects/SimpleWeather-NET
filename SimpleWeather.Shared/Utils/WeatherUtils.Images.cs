using SimpleWeather.Controls;
using SimpleWeather.Icons;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using SimpleWeather.WeatherData.Images;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Utils
{
    public static partial class WeatherUtils
    {
        public static Task<ImageDataViewModel> GetImageData(Weather weather)
        {
            return Task.Run(async () =>
            {
                String icon = weather.condition.icon;
                String backgroundCode = null;
                var wm = WeatherManager.GetInstance();

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
                        backgroundCode = WeatherBackground.RAIN;
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
                        backgroundCode = WeatherBackground.RAIN_NIGHT;
                        break;
                    // Tornado / Hurricane / Thunderstorm / Tropical Storm
                    case WeatherIcons.DAY_LIGHTNING:
                    case WeatherIcons.DAY_THUNDERSTORM:
                    case WeatherIcons.NIGHT_ALT_LIGHTNING:
                    case WeatherIcons.NIGHT_ALT_THUNDERSTORM:
                    case WeatherIcons.LIGHTNING:
                    case WeatherIcons.THUNDERSTORM:
                        backgroundCode = WeatherBackground.TSTORMS_NIGHT;
                        break;

                    case WeatherIcons.DAY_STORM_SHOWERS:
                    case WeatherIcons.DAY_SLEET_STORM:
                    case WeatherIcons.STORM_SHOWERS:
                    case WeatherIcons.NIGHT_ALT_STORM_SHOWERS:
                    case WeatherIcons.NIGHT_ALT_SLEET_STORM:
                    case WeatherIcons.HAIL:
                    case WeatherIcons.HURRICANE:
                    case WeatherIcons.TORNADO:
                        backgroundCode = WeatherBackground.STORMS;
                        break;

                    // Dust
                    case WeatherIcons.DUST:
                    case WeatherIcons.SANDSTORM:
                        backgroundCode = WeatherBackground.DUST;
                        break;

                    // Foggy / Haze
                    case WeatherIcons.DAY_FOG:
                    case WeatherIcons.DAY_HAZE:
                    case WeatherIcons.FOG:
                    case WeatherIcons.NIGHT_FOG:
                    case WeatherIcons.SMOG:
                    case WeatherIcons.SMOKE:
                        backgroundCode = WeatherBackground.FOG;
                        break;

                    // Snow / Snow Showers/Storm
                    case WeatherIcons.DAY_SNOW:
                    case WeatherIcons.DAY_SNOW_THUNDERSTORM:
                    case WeatherIcons.NIGHT_ALT_SNOW:
                    case WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM:
                    case WeatherIcons.SNOW:
                    case WeatherIcons.SNOW_WIND:
                    case WeatherIcons.DAY_SNOW_WIND:
                    case WeatherIcons.NIGHT_ALT_SNOW_WIND:
                        backgroundCode = WeatherBackground.SNOW;
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
                        if (wm.IsNight(weather))
                            backgroundCode = WeatherBackground.MOSTLYCLOUDY_NIGHT;
                        else
                            backgroundCode = WeatherBackground.MOSTLYCLOUDY_DAY;
                        break;

                    // Partly Cloudy
                    case WeatherIcons.DAY_SUNNY_OVERCAST:
                    case WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY:
                        if (wm.IsNight(weather))
                            backgroundCode = WeatherBackground.PARTLYCLOUDY_NIGHT;
                        else
                            backgroundCode = WeatherBackground.PARTLYCLOUDY_DAY;
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
                        if (wm.IsNight(weather))
                            backgroundCode = WeatherBackground.NIGHT;
                        else
                            backgroundCode = WeatherBackground.DAY;
                        break;
                }

                // Check cache for image data
                var imageHelper = ImageDataHelper.ImageDataHelperImpl;
                var imageData = await imageHelper.GetCachedImageData(backgroundCode);
                // Check if cache is available and valid
                if (imageData?.IsValid() == true)
                    return new ImageDataViewModel(imageData);
                else
                {
#if WINDOWS_UWP && !UNIT_TEST
                    if (!FeatureSettings.IsUpdateAvailable)
                    {
#endif
                        imageData = await imageHelper.GetRemoteImageData(backgroundCode);
                        if (imageData?.IsValid() == true)
                            return new ImageDataViewModel(imageData);
                        else
                        {
                            imageData = imageHelper.GetDefaultImageData(backgroundCode, weather);
                            if (imageData?.IsValid() == true)
                                return new ImageDataViewModel(imageData);
                        }
#if WINDOWS_UWP && !UNIT_TEST
                    }
                    else
                    {
                        imageData = imageHelper.GetDefaultImageData(backgroundCode, weather);
                        if (imageData?.IsValid() == true)
                            return new ImageDataViewModel(imageData);
                    }
#endif
                }

                return null;
            });
        }
    }
}