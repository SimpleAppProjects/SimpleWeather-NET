using Android.Content.Res;
using SimpleWeather.Droid;
using SimpleWeather.WeatherData;
using System;

namespace SimpleWeather.Utils
{
    public static partial class WeatherUtils
    {
        public static String GetBackgroundURI(Weather weather)
        {
            String icon = weather.condition.icon;
            String file = null;

            // Apply background based on weather condition
            /* WeatherUnderground */
            if (icon.Contains("mostlysunny") || icon.Contains("partlysunny") ||
                icon.Contains("partlycloudy"))
            {
                if (IsNight(weather))
                    file = "file:///android_asset/backgrounds/PartlyCloudy-Night.jpg";
                else
                    file = "file:///android_asset/backgrounds/PartlyCloudy-Day.jpg";
            }
            if (icon.Contains("cloudy"))
            {
                if (IsNight(weather))
                    file = "file:///android_asset/backgrounds/MostlyCloudy-Night.jpg";
                else
                    file = "file:///android_asset/backgrounds/MostlyCloudy-Day.jpg";
            }
            else if (icon.Contains("rain") || icon.Contains("sleet"))
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

            /* Yahoo Weather */
            if (String.IsNullOrWhiteSpace(file) && int.TryParse(icon, out int code))
            {
                switch (code)
                {
                    // Night
                    case 31:
                    case 33:
                        file = "file:///android_asset/backgrounds/NightSky.jpg";
                        break;
                    // Rain 
                    case 9:
                    case 11:
                    case 12:
                    case 40:
                    // (Mixed) Rain/Snow/Sleet
                    case 5:
                    case 6:
                    case 7:
                    case 18:
                    // Hail / Freezing Rain
                    case 8:
                    case 10:
                    case 17:
                    case 35:
                        file = "file:///android_asset/backgrounds/RainySky.jpg";
                        break;
                    // Tornado / Hurricane / Thunderstorm / Tropical Storm
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 37:
                    case 38:
                    case 39:
                    case 45:
                    case 47:
                        file = "file:///android_asset/backgrounds/StormySky.jpg";
                        break;
                    // Dust
                    case 19:
                        file = "file:///android_asset/backgrounds/Dust.jpg";
                        break;
                    // Foggy / Haze
                    case 20:
                    case 21:
                    case 22:
                        file = "file:///android_asset/backgrounds/FoggySky.jpg";
                        break;
                    // Snow / Snow Showers/Storm
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 41:
                    case 42:
                    case 43:
                    case 46:
                        file = "file:///android_asset/backgrounds/Snow.jpg";
                        break;
                    /* Ambigious weather conditions */
                    // (Mostly) Cloudy
                    case 28:
                    case 26:
                    case 27:
                        if (IsNight(weather))
                            file = "file:///android_asset/backgrounds/MostlyCloudy-Night.jpg";
                        else
                            file = "file:///android_asset/backgrounds/MostlyCloudy-Day.jpg";
                        break;
                    // Partly Cloudy
                    case 44:
                    case 29:
                    case 30:
                        if (IsNight(weather))
                            file = "file:///android_asset/backgrounds/PartlyCloudy-Night.jpg";
                        else
                            file = "file:///android_asset/backgrounds/PartlyCloudy-Day.jpg";
                        break;
                    case 3200:
                    default:
                        // Set background based using sunset/rise times
                        if (IsNight(weather))
                            file = "file:///android_asset/backgrounds/NightSky.jpg";
                        else
                            file = "file:///android_asset/backgrounds/DaySky.jpg";
                        break;
                }
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
    }
}