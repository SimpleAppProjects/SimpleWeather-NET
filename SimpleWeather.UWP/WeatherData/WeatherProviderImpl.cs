using System;
using System.Globalization;
using System.Runtime.Serialization;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

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
                case WeatherIcons.DAY_SLEET_STORM:
                case WeatherIcons.DAY_SPRINKLE:
                case WeatherIcons.HAIL:
                case WeatherIcons.NIGHT_ALT_HAIL:
                case WeatherIcons.NIGHT_ALT_RAIN:
                case WeatherIcons.NIGHT_ALT_RAIN_MIX:
                case WeatherIcons.NIGHT_ALT_RAIN_WIND:
                case WeatherIcons.NIGHT_ALT_SHOWERS:
                case WeatherIcons.NIGHT_ALT_SLEET:
                case WeatherIcons.NIGHT_ALT_SLEET_STORM:
                case WeatherIcons.NIGHT_ALT_SPRINKLE:
                case WeatherIcons.RAIN:
                case WeatherIcons.RAIN_MIX:
                case WeatherIcons.RAIN_WIND:
                case WeatherIcons.SHOWERS:
                case WeatherIcons.SLEET:
                case WeatherIcons.SPRINKLE:
                    imgURI = ("ms-appx:///Assets/Backgrounds/RainySky.jpg");
                    break;
                // Tornado / Hurricane / Thunderstorm / Tropical Storm
                case WeatherIcons.DAY_LIGHTNING:
                case WeatherIcons.DAY_STORM_SHOWERS:
                case WeatherIcons.DAY_THUNDERSTORM:
                case WeatherIcons.NIGHT_ALT_LIGHTNING:
                case WeatherIcons.NIGHT_ALT_STORM_SHOWERS:
                case WeatherIcons.NIGHT_ALT_THUNDERSTORM:
                case WeatherIcons.HURRICANE:
                case WeatherIcons.LIGHTNING:
                case WeatherIcons.STORM_SHOWERS:
                case WeatherIcons.THUNDERSTORM:
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
                case WeatherIcons.DAY_SNOW_WIND:
                case WeatherIcons.NIGHT_ALT_SNOW:
                case WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM:
                case WeatherIcons.NIGHT_ALT_SNOW_WIND:
                case WeatherIcons.SNOW:
                case WeatherIcons.SNOW_WIND:
                    imgURI = ("ms-appx:///Assets/Backgrounds/Snow.jpg");
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

        public virtual String GetWeatherIconURI(string icon)
        {
            string baseuri = "ms-appx:///Assets/WeatherIcons/png/";
            string fileIcon = string.Empty;

            switch(icon)
            {
                case WeatherIcons.DAY_SUNNY:
                    fileIcon = "day_sunny.png";
                    break;
                case WeatherIcons.DAY_CLOUDY:
                    fileIcon = "day_cloudy.png";
                    break;
                case WeatherIcons.DAY_CLOUDY_GUSTS:
                    fileIcon = "day_cloudy_gusts.png";
                    break;
                case WeatherIcons.DAY_CLOUDY_WINDY:
                    fileIcon = "day_cloudy_windy.png";
                    break;
                case WeatherIcons.DAY_FOG:
                    fileIcon = "day_fog.png";
                    break;
                case WeatherIcons.DAY_HAIL:
                    fileIcon = "day_hail.png";
                    break;
                case WeatherIcons.DAY_HAZE:
                    fileIcon = "day_haze.png";
                    break;
                case WeatherIcons.DAY_LIGHTNING:
                    fileIcon = "day_lightning.png";
                    break;
                case WeatherIcons.DAY_RAIN:
                    fileIcon = "day_rain.png";
                    break;
                case WeatherIcons.DAY_RAIN_MIX:
                    fileIcon = "day_rain_mix.png";
                    break;
                case WeatherIcons.DAY_RAIN_WIND:
                    fileIcon = "day_rain_wind.png";
                    break;
                case WeatherIcons.DAY_SHOWERS:
                    fileIcon = "day_showers.png";
                    break;
                case WeatherIcons.DAY_SLEET:
                    fileIcon = "day_sleet.png";
                    break;
                case WeatherIcons.DAY_SLEET_STORM:
                    fileIcon = "day_sleet_storm.png";
                    break;
                case WeatherIcons.DAY_SNOW:
                    fileIcon = "day_snow.png";
                    break;
                case WeatherIcons.DAY_SNOW_THUNDERSTORM:
                    fileIcon = "day_snow_thunderstorm.png";
                    break;
                case WeatherIcons.DAY_SNOW_WIND:
                    fileIcon = "day_snow_wind.png";
                    break;
                case WeatherIcons.DAY_SPRINKLE:
                    fileIcon = "day_sprinkle.png";
                    break;
                case WeatherIcons.DAY_STORM_SHOWERS:
                    fileIcon = "day_storm_showers.png";
                    break;
                case WeatherIcons.DAY_SUNNY_OVERCAST:
                    fileIcon = "day_sunny_overcast.png";
                    break;
                case WeatherIcons.DAY_THUNDERSTORM:
                    fileIcon = "day_thunderstorm.png";
                    break;
                case WeatherIcons.DAY_WINDY:
                    fileIcon = "day_windy.png";
                    break;
                case WeatherIcons.DAY_HOT:
                    fileIcon = "hot.png";
                    break;
                case WeatherIcons.DAY_CLOUDY_HIGH:
                    fileIcon = "day_cloudy_high.png";
                    break;
                case WeatherIcons.DAY_LIGHT_WIND:
                    fileIcon = "day_light_wind.png";
                    break;
                case WeatherIcons.NIGHT_CLEAR:
                    fileIcon = "night_clear.png";
                    break;
                case WeatherIcons.NIGHT_ALT_CLOUDY:
                    fileIcon = "night_alt_cloudy.png";
                    break;
                case WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS:
                    fileIcon = "night_alt_cloudy_gusts.png";
                    break;
                case WeatherIcons.NIGHT_ALT_CLOUDY_WINDY:
                    fileIcon = "night_alt_cloudy_windy.png";
                    break;
                case WeatherIcons.NIGHT_ALT_HAIL:
                    fileIcon = "night_alt_hail.png";
                    break;
                case WeatherIcons.NIGHT_ALT_LIGHTNING:
                    fileIcon = "night_alt_lightning.png";
                    break;
                case WeatherIcons.NIGHT_ALT_RAIN:
                    fileIcon = "night_alt_rain.png";
                    break;
                case WeatherIcons.NIGHT_ALT_RAIN_MIX:
                    fileIcon = "night_alt_rain_mix.png";
                    break;
                case WeatherIcons.NIGHT_ALT_RAIN_WIND:
                    fileIcon = "night_alt_rain_wind.png";
                    break;
                case WeatherIcons.NIGHT_ALT_SHOWERS:
                    fileIcon = "night_alt_showers.png";
                    break;
                case WeatherIcons.NIGHT_ALT_SLEET:
                    fileIcon = "night_alt_sleet.png";
                    break;
                case WeatherIcons.NIGHT_ALT_SLEET_STORM:
                    fileIcon = "night_alt_sleet_storm.png";
                    break;
                case WeatherIcons.NIGHT_ALT_SNOW:
                    fileIcon = "night_alt_snow.png";
                    break;
                case WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM:
                    fileIcon = "night_alt_snow_thunderstorm.png";
                    break;
                case WeatherIcons.NIGHT_ALT_SNOW_WIND:
                    fileIcon = "night_alt_snow_wind.png";
                    break;
                case WeatherIcons.NIGHT_ALT_SPRINKLE:
                    fileIcon = "night_alt_sprinkle.png";
                    break;
                case WeatherIcons.NIGHT_ALT_STORM_SHOWERS:
                    fileIcon = "night_alt_storm_showers.png";
                    break;
                case WeatherIcons.NIGHT_ALT_THUNDERSTORM:
                    fileIcon = "night_alt_thunderstorm.png";
                    break;
                case WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY:
                    fileIcon = "night_alt_partly_cloudy.png";
                    break;
                case WeatherIcons.NIGHT_ALT_CLOUDY_HIGH:
                    fileIcon = "night_alt_cloudy_high.png";
                    break;
                case WeatherIcons.NIGHT_FOG:
                    fileIcon = "night_fog.png";
                    break;
                case WeatherIcons.CLOUD:
                    fileIcon = "cloud.png";
                    break;
                case WeatherIcons.CLOUDY:
                    fileIcon = "cloudy.png";
                    break;
                case WeatherIcons.CLOUDY_GUSTS:
                    fileIcon = "cloudy_gusts.png";
                    break;
                case WeatherIcons.CLOUDY_WINDY:
                    fileIcon = "cloudy_windy.png";
                    break;
                case WeatherIcons.FOG:
                    fileIcon = "fog.png";
                    break;
                case WeatherIcons.HAIL:
                    fileIcon = "hail.png";
                    break;
                case WeatherIcons.RAIN:
                    fileIcon = "rain.png";
                    break;
                case WeatherIcons.RAIN_MIX:
                    fileIcon = "rain_mix.png";
                    break;
                case WeatherIcons.RAIN_WIND:
                    fileIcon = "rain_wind.png";
                    break;
                case WeatherIcons.SHOWERS:
                    fileIcon = "showers.png";
                    break;
                case WeatherIcons.SLEET:
                    fileIcon = "sleet.png";
                    break;
                case WeatherIcons.SNOW:
                    fileIcon = "snow.png";
                    break;
                case WeatherIcons.SPRINKLE:
                    fileIcon = "sprinkle.png";
                    break;
                case WeatherIcons.STORM_SHOWERS:
                    fileIcon = "storm_showers.png";
                    break;
                case WeatherIcons.THUNDERSTORM:
                    fileIcon = "thunderstorm.png";
                    break;
                case WeatherIcons.SNOW_WIND:
                    fileIcon = "snow_wind.png";
                    break;
                case WeatherIcons.SMOG:
                    fileIcon = "smog.png";
                    break;
                case WeatherIcons.SMOKE:
                    fileIcon = "smoke.png";
                    break;
                case WeatherIcons.LIGHTNING:
                    fileIcon = "lightning.png";
                    break;
                case WeatherIcons.DUST:
                    fileIcon = "dust.png";
                    break;
                case WeatherIcons.SNOWFLAKE_COLD:
                    fileIcon = "snowflake_cold.png";
                    break;
                case WeatherIcons.WINDY:
                    fileIcon = "windy.png";
                    break;
                case WeatherIcons.STRONG_WIND:
                    fileIcon = "strong_wind.png";
                    break;
                case WeatherIcons.SANDSTORM:
                    fileIcon = "sandstorm.png";
                    break;
                case WeatherIcons.HURRICANE:
                    fileIcon = "hurricane.png";
                    break;
                case WeatherIcons.TORNADO:
                    fileIcon = "tornado.png";
                    break;
            }

            if (String.IsNullOrWhiteSpace(fileIcon))
            {
                // Not Available
                fileIcon = "na.png";
            }

            return baseuri + fileIcon;
        }

        public virtual void SetBackground(ImageBrush bg, Weather weather)
        {
            Uri imgURI = new Uri(GetBackgroundURI(weather));

            if (bg != null && bg.ImageSource != null)
            {
                // Skip re-settting bg
                if (bg.ImageSource is BitmapImage bmp && bmp.UriSource == imgURI)
                    return;
            }

            BitmapImage img = new BitmapImage(imgURI)
            {
                CreateOptions = BitmapCreateOptions.None,
                DecodePixelWidth = 960
            };
            bg.ImageSource = img;
        }
    }
}