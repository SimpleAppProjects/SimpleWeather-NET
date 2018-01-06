using Android.Content.Res;
using SimpleWeather.Droid;
using SimpleWeather.WeatherData;
using System;

namespace SimpleWeather.WeatherUnderground
{
    public partial class WeatherUndergroundProvider : WeatherProviderImpl
    {
        public override string GetBackgroundURI(Weather weather)
        {
            String icon = weather.condition.icon;
            String file = null;

            // Apply background based on weather condition
            if (icon.Contains("mostlysunny") || icon.Contains("partlysunny") ||
                icon.Contains("partlycloudy"))
            {
                if (IsNight(weather))
                    file = "file:///android_asset/backgrounds/PartlyCloudy-Night.jpg";
                else
                    file = "file:///android_asset/backgrounds/PartlyCloudy-Day.jpg";
            }
            else if (icon.Contains("cloudy"))
            {
                if (IsNight(weather))
                    file = "file:///android_asset/backgrounds/MostlyCloudy-Night.jpg";
                else
                    file = "file:///android_asset/backgrounds/MostlyCloudy-Day.jpg";
            }
            else if (icon.Contains("rain") || icon.Contains("sleet") || icon.Contains("sleat"))
            {
                file = "file:///android_asset/backgrounds/RainySky.jpg";
            }
            else if (icon.Contains("flurries") || icon.Contains("snow"))
            {
                file = "file:///android_asset/backgrounds/Snow.jpg";
            }
            else if (icon.Contains("tstorms"))
            {
                file = "file:///android_asset/backgrounds/StormySky.jpg";
            }
            else if (icon.Contains("hazy") || icon.Contains("fog"))
            {
                file = "file:///android_asset/backgrounds/FoggySky.jpg";
            }
            else if (icon.Contains("clear") || icon.Contains("sunny"))
            {
                // Set background based using sunset/rise times
                if (IsNight(weather))
                    file = "file:///android_asset/backgrounds/NightSky.jpg";
                else
                    file = "file:///android_asset/backgrounds/DaySky.jpg";
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

        public override int GetWeatherIconResource(string icon)
        {
            int weatherIcon = -1;

            if (icon.Contains("nt_mostlycloudy") || icon.Contains("nt_partlysunny") || icon.Contains("nt_cloudy"))
                weatherIcon = Resource.Drawable.yahoo_27;
            else if (icon.Contains("nt_partlycloudy") || icon.Contains("nt_mostlysunny"))
                weatherIcon = Resource.Drawable.yahoo_33;
            else if (icon.Contains("nt_clear") || icon.Contains("nt_sunny") || icon.Contains("nt_unknown"))
                weatherIcon = Resource.Drawable.yahoo_31;
            else if (icon.Contains("chancerain"))
                weatherIcon = Resource.Drawable.wu_chancerain;
            else if (icon.Contains("clear") || icon.Contains("sunny"))
                weatherIcon = Resource.Drawable.wu_clear;
            else if (icon.Contains("cloudy"))
                weatherIcon = Resource.Drawable.wu_cloudy;
            else if (icon.Contains("flurries"))
                weatherIcon = Resource.Drawable.wu_flurries;
            else if (icon.Contains("fog"))
                weatherIcon = Resource.Drawable.yahoo_20;
            else if (icon.Contains("hazy"))
                weatherIcon = Resource.Drawable.yahoo_21;
            else if (icon.Contains("sleet") || icon.Contains("sleat"))
                weatherIcon = Resource.Drawable.wu_sleat;
            else if (icon.Contains("rain"))
                weatherIcon = Resource.Drawable.wu_rain;
            else if (icon.Contains("snow"))
                weatherIcon = Resource.Drawable.wu_snow;
            else if (icon.Contains("tstorms"))
                weatherIcon = Resource.Drawable.wu_tstorms;
            else if (icon.Contains("unknown"))
                weatherIcon = Resource.Drawable.wu_unknown;
            else if (icon.Contains("nt_"))
                weatherIcon = Resource.Drawable.yahoo_31;

            if (weatherIcon == -1)
            {
                // Not Available
                weatherIcon = Resource.Drawable.na;
            }

            return weatherIcon;
        }
    }
}