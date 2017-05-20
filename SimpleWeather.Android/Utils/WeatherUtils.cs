using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Media;
using Java.IO;
using SimpleWeather.Droid;
using SimpleWeather.WeatherData;
using System;

namespace SimpleWeather.Utils
{
    public static partial class WeatherUtils
    {
        private static Resources res = App.Context.Resources;
        private static AssetManager am = App.Context.Assets;

        public static System.IO.Stream GetBackgroundStream(Weather weather)
        {
            System.IO.Stream imgStream = null;

            // Apply background based on weather condition
            /* WeatherUnderground */
            switch (weather.condition.icon)
            {
                case "cloudy":
                case "mostlycloudy":
                    if (isNight(weather))
                        
                        imgStream = am.Open("backgrounds/MostlyCloudy-Night.jpg");
                    else
                        imgStream = am.Open("backgrounds/MostlyCloudy-Day.jpg");
                    break;
                case "mostlysunny":
                case "partlysunny":
                case "partlycloudy":
                    if (isNight(weather))
                        imgStream = am.Open("backgrounds/PartlyCloudy-Night.jpg");
                    else
                        imgStream = am.Open("backgrounds/PartlyCloudy-Day.jpg");
                    break;
                case "chancerain":
                case "chancesleat":
                case "rain":
                case "sleat":
                    imgStream = am.Open("backgrounds/RainySky.jpg");
                    break;
                case "chanceflurries":
                case "chancesnow":
                case "flurries":
                case "snow":
                    imgStream = am.Open("backgrounds/Snow.jpg");
                    break;
                case "chancetstorms":
                case "tstorms":
                    imgStream = am.Open("backgrounds/StormySky.jpg");
                    break;
                case "hazy":
                    imgStream = am.Open("backgrounds/FoggySky.jpg");
                    break;
                case "sunny":
                case "clear":
                case "unknown":
                    // Set background based using sunset/rise times
                    if (isNight(weather))
                        imgStream = am.Open("backgrounds/NightSky.jpg");
                    else
                        imgStream = am.Open("backgrounds/DaySky.jpg");
                    break;
            }

            /* Yahoo Weather */
            if (imgStream == null)
            {
                switch (int.Parse(weather.condition.icon))
                {
                    // Night
                    case 31:
                    case 33:
                        imgStream = am.Open("backgrounds/NightSky.jpg");
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
                        imgStream = am.Open("backgrounds/RainySky.jpg");
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
                        imgStream = am.Open("backgrounds/StormySky.jpg");
                        break;
                    // Dust
                    case 19:
                        imgStream = am.Open("backgrounds/Dust.jpg");
                        break;
                    // Foggy / Haze
                    case 20:
                    case 21:
                    case 22:
                        imgStream = am.Open("backgrounds/FoggySky.jpg");
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
                        imgStream = am.Open("backgrounds/Snow.jpg");
                        break;
                    /* Ambigious weather conditions */
                    // (Mostly) Cloudy
                    case 28:
                    case 26:
                    case 27:
                        if (isNight(weather))
                            imgStream = am.Open("backgrounds/MostlyCloudy-Night.jpg");
                        else
                            imgStream = am.Open("backgrounds/MostlyCloudy-Day.jpg");
                        break;
                    // Partly Cloudy
                    case 44:
                    case 29:
                    case 30:
                        if (isNight(weather))
                            imgStream = am.Open("backgrounds/PartlyCloudy-Night.jpg");
                        else
                            imgStream = am.Open("backgrounds/PartlyCloudy-Day.jpg");
                        break;
                    default:
                        // Set background based using sunset/rise times
                        if (isNight(weather))
                            imgStream = am.Open("backgrounds/NightSky.jpg");
                        else
                            imgStream = am.Open("backgrounds/DaySky.jpg");
                        break;
                }
            }

            return imgStream;//return new BitmapDrawable(res, ThumbnailUtils.ExtractThumbnail(BitmapFactory.DecodeStream(imgStream), width, height, ThumnailExtractOptions.RecycleInput));
        }
    }
}
