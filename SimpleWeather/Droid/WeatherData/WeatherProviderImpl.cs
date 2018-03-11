using Android.Content.Res;
using Android.Graphics;
using SimpleWeather.Droid;
using SimpleWeather.WeatherData;
using System;

namespace SimpleWeather.WeatherData
{
    public abstract partial class WeatherProviderImpl : IWeatherProviderImpl
    {
        // Utils Methods
        public virtual string GetBackgroundURI(Weather weather)
        {
            String icon = weather.condition.icon;
            String file = null;

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
                    file = "file:///android_asset/backgrounds/RainySky.jpg";
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
                    file = "file:///android_asset/backgrounds/StormySky.jpg";
                    break;
                // Dust
                case WeatherIcons.DUST:
                case WeatherIcons.SANDSTORM:
                    file = "file:///android_asset/backgrounds/Dust.jpg";
                    break;
                // Foggy / Haze
                case WeatherIcons.DAY_FOG:
                case WeatherIcons.DAY_HAZE:
                case WeatherIcons.FOG:
                case WeatherIcons.NIGHT_FOG:
                case WeatherIcons.SMOG:
                case WeatherIcons.SMOKE:
                    file = "file:///android_asset/backgrounds/FoggySky.jpg";
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
                    file = "file:///android_asset/backgrounds/Snow.jpg";
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
                        file = "file:///android_asset/backgrounds/MostlyCloudy-Night.jpg";
                    else
                        file = "file:///android_asset/backgrounds/MostlyCloudy-Day.jpg";
                    break;
                // Partly Cloudy
                case WeatherIcons.DAY_SUNNY_OVERCAST:
                case WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY:
                    if (IsNight(weather))
                        file = "file:///android_asset/backgrounds/PartlyCloudy-Night.jpg";
                    else
                        file = "file:///android_asset/backgrounds/PartlyCloudy-Day.jpg";
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
                        file = "file:///android_asset/backgrounds/NightSky.jpg";
                    else
                        file = "file:///android_asset/backgrounds/DaySky.jpg";
                    break;
            }

            // Just in case
            if (String.IsNullOrWhiteSpace(file))
            {
                // Set background based using sunset/rise times
                if (IsNight(weather))
                    file = "file:///android_asset/backgrounds/NightSky.jpg";
                else
                    file = "file:///android_asset/backgrounds/DaySky.jpg";
            }

            return file;
        }

        public virtual int GetWeatherIconResource(string icon)
        {
            int weatherIcon = -1;

            switch (icon)
            {
                case WeatherIcons.DAY_SUNNY:
                    weatherIcon = Resource.Drawable.day_sunny;
                    break;
                case WeatherIcons.DAY_CLOUDY:
                    weatherIcon = Resource.Drawable.day_cloudy;
                    break;
                case WeatherIcons.DAY_CLOUDY_GUSTS:
                    weatherIcon = Resource.Drawable.day_cloudy_gusts;
                    break;
                case WeatherIcons.DAY_CLOUDY_WINDY:
                    weatherIcon = Resource.Drawable.day_cloudy_windy;
                    break;
                case WeatherIcons.DAY_FOG:
                    weatherIcon = Resource.Drawable.day_fog;
                    break;
                case WeatherIcons.DAY_HAIL:
                    weatherIcon = Resource.Drawable.day_hail;
                    break;
                case WeatherIcons.DAY_HAZE:
                    weatherIcon = Resource.Drawable.day_haze;
                    break;
                case WeatherIcons.DAY_LIGHTNING:
                    weatherIcon = Resource.Drawable.day_lightning;
                    break;
                case WeatherIcons.DAY_RAIN:
                    weatherIcon = Resource.Drawable.day_rain;
                    break;
                case WeatherIcons.DAY_RAIN_MIX:
                    weatherIcon = Resource.Drawable.day_rain_mix;
                    break;
                case WeatherIcons.DAY_RAIN_WIND:
                    weatherIcon = Resource.Drawable.day_rain_wind;
                    break;
                case WeatherIcons.DAY_SHOWERS:
                    weatherIcon = Resource.Drawable.day_showers;
                    break;
                case WeatherIcons.DAY_SLEET:
                    weatherIcon = Resource.Drawable.day_sleet;
                    break;
                case WeatherIcons.DAY_SLEET_STORM:
                    weatherIcon = Resource.Drawable.day_sleet_storm;
                    break;
                case WeatherIcons.DAY_SNOW:
                    weatherIcon = Resource.Drawable.day_snow;
                    break;
                case WeatherIcons.DAY_SNOW_THUNDERSTORM:
                    weatherIcon = Resource.Drawable.day_snow_thunderstorm;
                    break;
                case WeatherIcons.DAY_SNOW_WIND:
                    weatherIcon = Resource.Drawable.day_snow_wind;
                    break;
                case WeatherIcons.DAY_SPRINKLE:
                    weatherIcon = Resource.Drawable.day_sprinkle;
                    break;
                case WeatherIcons.DAY_STORM_SHOWERS:
                    weatherIcon = Resource.Drawable.day_storm_showers;
                    break;
                case WeatherIcons.DAY_SUNNY_OVERCAST:
                    weatherIcon = Resource.Drawable.day_sunny_overcast;
                    break;
                case WeatherIcons.DAY_THUNDERSTORM:
                    weatherIcon = Resource.Drawable.day_thunderstorm;
                    break;
                case WeatherIcons.DAY_WINDY:
                    weatherIcon = Resource.Drawable.day_windy;
                    break;
                case WeatherIcons.DAY_HOT:
                    weatherIcon = Resource.Drawable.hot;
                    break;
                case WeatherIcons.DAY_CLOUDY_HIGH:
                    weatherIcon = Resource.Drawable.day_cloudy_high;
                    break;
                case WeatherIcons.DAY_LIGHT_WIND:
                    weatherIcon = Resource.Drawable.day_light_wind;
                    break;
                case WeatherIcons.NIGHT_CLEAR:
                    weatherIcon = Resource.Drawable.night_clear;
                    break;
                case WeatherIcons.NIGHT_ALT_CLOUDY:
                    weatherIcon = Resource.Drawable.night_alt_cloudy;
                    break;
                case WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS:
                    weatherIcon = Resource.Drawable.night_alt_cloudy_gusts;
                    break;
                case WeatherIcons.NIGHT_ALT_CLOUDY_WINDY:
                    weatherIcon = Resource.Drawable.night_alt_cloudy_windy;
                    break;
                case WeatherIcons.NIGHT_ALT_HAIL:
                    weatherIcon = Resource.Drawable.night_alt_hail;
                    break;
                case WeatherIcons.NIGHT_ALT_LIGHTNING:
                    weatherIcon = Resource.Drawable.night_alt_lightning;
                    break;
                case WeatherIcons.NIGHT_ALT_RAIN:
                    weatherIcon = Resource.Drawable.night_alt_rain;
                    break;
                case WeatherIcons.NIGHT_ALT_RAIN_MIX:
                    weatherIcon = Resource.Drawable.night_alt_rain_mix;
                    break;
                case WeatherIcons.NIGHT_ALT_RAIN_WIND:
                    weatherIcon = Resource.Drawable.night_alt_rain_wind;
                    break;
                case WeatherIcons.NIGHT_ALT_SHOWERS:
                    weatherIcon = Resource.Drawable.night_alt_showers;
                    break;
                case WeatherIcons.NIGHT_ALT_SLEET:
                    weatherIcon = Resource.Drawable.night_alt_sleet;
                    break;
                case WeatherIcons.NIGHT_ALT_SLEET_STORM:
                    weatherIcon = Resource.Drawable.night_alt_sleet_storm;
                    break;
                case WeatherIcons.NIGHT_ALT_SNOW:
                    weatherIcon = Resource.Drawable.night_alt_snow;
                    break;
                case WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM:
                    weatherIcon = Resource.Drawable.night_alt_snow_thunderstorm;
                    break;
                case WeatherIcons.NIGHT_ALT_SNOW_WIND:
                    weatherIcon = Resource.Drawable.night_alt_snow_wind;
                    break;
                case WeatherIcons.NIGHT_ALT_SPRINKLE:
                    weatherIcon = Resource.Drawable.night_alt_sprinkle;
                    break;
                case WeatherIcons.NIGHT_ALT_STORM_SHOWERS:
                    weatherIcon = Resource.Drawable.night_alt_storm_showers;
                    break;
                case WeatherIcons.NIGHT_ALT_THUNDERSTORM:
                    weatherIcon = Resource.Drawable.night_alt_thunderstorm;
                    break;
                case WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY:
                    weatherIcon = Resource.Drawable.night_alt_partly_cloudy;
                    break;
                case WeatherIcons.NIGHT_ALT_CLOUDY_HIGH:
                    weatherIcon = Resource.Drawable.night_alt_cloudy_high;
                    break;
                case WeatherIcons.NIGHT_FOG:
                    weatherIcon = Resource.Drawable.night_fog;
                    break;
                case WeatherIcons.CLOUD:
                    weatherIcon = Resource.Drawable.cloud;
                    break;
                case WeatherIcons.CLOUDY:
                    weatherIcon = Resource.Drawable.cloudy;
                    break;
                case WeatherIcons.CLOUDY_GUSTS:
                    weatherIcon = Resource.Drawable.cloudy_gusts;
                    break;
                case WeatherIcons.CLOUDY_WINDY:
                    weatherIcon = Resource.Drawable.cloudy_windy;
                    break;
                case WeatherIcons.FOG:
                    weatherIcon = Resource.Drawable.fog;
                    break;
                case WeatherIcons.HAIL:
                    weatherIcon = Resource.Drawable.hail;
                    break;
                case WeatherIcons.RAIN:
                    weatherIcon = Resource.Drawable.rain;
                    break;
                case WeatherIcons.RAIN_MIX:
                    weatherIcon = Resource.Drawable.rain_mix;
                    break;
                case WeatherIcons.RAIN_WIND:
                    weatherIcon = Resource.Drawable.rain_wind;
                    break;
                case WeatherIcons.SHOWERS:
                    weatherIcon = Resource.Drawable.showers;
                    break;
                case WeatherIcons.SLEET:
                    weatherIcon = Resource.Drawable.sleet;
                    break;
                case WeatherIcons.SNOW:
                    weatherIcon = Resource.Drawable.snow;
                    break;
                case WeatherIcons.SPRINKLE:
                    weatherIcon = Resource.Drawable.sprinkle;
                    break;
                case WeatherIcons.STORM_SHOWERS:
                    weatherIcon = Resource.Drawable.storm_showers;
                    break;
                case WeatherIcons.THUNDERSTORM:
                    weatherIcon = Resource.Drawable.thunderstorm;
                    break;
                case WeatherIcons.SNOW_WIND:
                    weatherIcon = Resource.Drawable.snow_wind;
                    break;
                case WeatherIcons.SMOG:
                    weatherIcon = Resource.Drawable.smog;
                    break;
                case WeatherIcons.SMOKE:
                    weatherIcon = Resource.Drawable.smoke;
                    break;
                case WeatherIcons.LIGHTNING:
                    weatherIcon = Resource.Drawable.lightning;
                    break;
                case WeatherIcons.DUST:
                    weatherIcon = Resource.Drawable.dust;
                    break;
                case WeatherIcons.SNOWFLAKE_COLD:
                    weatherIcon = Resource.Drawable.snowflake_cold;
                    break;
                case WeatherIcons.WINDY:
                    weatherIcon = Resource.Drawable.windy;
                    break;
                case WeatherIcons.STRONG_WIND:
                    weatherIcon = Resource.Drawable.strong_wind;
                    break;
                case WeatherIcons.SANDSTORM:
                    weatherIcon = Resource.Drawable.sandstorm;
                    break;
                case WeatherIcons.HURRICANE:
                    weatherIcon = Resource.Drawable.hurricane;
                    break;
                case WeatherIcons.TORNADO:
                    weatherIcon = Resource.Drawable.tornado;
                    break;
            }

            if (weatherIcon == -1)
            {
                // Not Available
                weatherIcon = Resource.Drawable.na;
            }

            return weatherIcon;
        }
    }
}