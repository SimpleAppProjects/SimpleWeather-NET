using SimpleWeather.WeatherData;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace SimpleWeather.Utils
{
    public static partial class WeatherUtils
    {
        public static void SetBackground(ImageBrush bg, Weather weather)
        {
            Uri imgURI = null;

            // Apply background based on weather condition
            /* WeatherUnderground */
            // TODO: use if statements
            switch (weather.condition.icon)
            {
                case "cloudy":
                case "mostlycloudy":
                    if (isNight(weather))
                        imgURI = new Uri("ms-appx:///Assets/Backgrounds/MostlyCloudy-Night.jpg");
                    else
                        imgURI = new Uri("ms-appx:///Assets/Backgrounds/MostlyCloudy-Day.jpg");
                    break;
                case "mostlysunny":
                case "partlysunny":
                case "partlycloudy":
                    if (isNight(weather))
                        imgURI = new Uri("ms-appx:///Assets/Backgrounds/PartlyCloudy-Night.jpg");
                    else
                        imgURI = new Uri("ms-appx:///Assets/Backgrounds/PartlyCloudy-Day.jpg");
                    break;
                case "chancerain":
                case "chancesleat":
                case "rain":
                case "sleat":
                    imgURI = new Uri("ms-appx:///Assets/Backgrounds/RainySky.jpg");
                    break;
                case "chanceflurries":
                case "chancesnow":
                case "flurries":
                case "snow":
                    imgURI = new Uri("ms-appx:///Assets/Backgrounds/Snow.jpg");
                    break;
                case "chancetstorms":
                case "tstorms":
                    imgURI = new Uri("ms-appx:///Assets/Backgrounds/StormySky.jpg");
                    break;
                case "hazy":
                    imgURI = new Uri("ms-appx:///Assets/Backgrounds/FoggySky.jpg");
                    break;
                case "sunny":
                case "clear":
                case "unknown":
                    // Set background based using sunset/rise times
                    if (isNight(weather))
                        imgURI = new Uri("ms-appx:///Assets/Backgrounds/NightSky.jpg");
                    else
                        imgURI = new Uri("ms-appx:///Assets/Backgrounds/DaySky.jpg");
                    break;
            }

            /* Yahoo Weather */
            int code = 0;
            if (imgURI == null && int.TryParse(weather.condition.icon, out code))
            {
                switch (int.Parse(weather.condition.icon))
                {
                    // Night
                    case 31:
                    case 33:
                        imgURI = new Uri("ms-appx:///Assets/Backgrounds/NightSky.jpg");
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
                        imgURI = new Uri("ms-appx:///Assets/Backgrounds/RainySky.jpg");
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
                        imgURI = new Uri("ms-appx:///Assets/Backgrounds/StormySky.jpg");
                        break;
                    // Dust
                    case 19:
                        imgURI = new Uri("ms-appx:///Assets/Backgrounds/Dust.jpg");
                        break;
                    // Foggy / Haze
                    case 20:
                    case 21:
                    case 22:
                        imgURI = new Uri("ms-appx:///Assets/Backgrounds/FoggySky.jpg");
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
                        imgURI = new Uri("ms-appx:///Assets/Backgrounds/Snow.jpg");
                        break;
                    /* Ambigious weather conditions */
                    // (Mostly) Cloudy
                    case 28:
                    case 26:
                    case 27:
                        if (isNight(weather))
                            imgURI = new Uri("ms-appx:///Assets/Backgrounds/MostlyCloudy-Night.jpg");
                        else
                            imgURI = new Uri("ms-appx:///Assets/Backgrounds/MostlyCloudy-Day.jpg");
                        break;
                    // Partly Cloudy
                    case 44:
                    case 29:
                    case 30:
                        if (isNight(weather))
                            imgURI = new Uri("ms-appx:///Assets/Backgrounds/PartlyCloudy-Night.jpg");
                        else
                            imgURI = new Uri("ms-appx:///Assets/Backgrounds/PartlyCloudy-Day.jpg");
                        break;
                    default:
                        // Set background based using sunset/rise times
                        if (isNight(weather))
                            imgURI = new Uri("ms-appx:///Assets/Backgrounds/NightSky.jpg");
                        else
                            imgURI = new Uri("ms-appx:///Assets/Backgrounds/DaySky.jpg");
                        break;
                }
            }

            if (bg != null && bg.ImageSource != null)
            {
                BitmapImage bmp = bg.ImageSource as BitmapImage;

                if (bmp != null && bmp.UriSource == imgURI)
                {
                    System.Diagnostics.Debug.WriteLine("Skipping...");
                    return;
                }
            }

            // Just in case
            if (imgURI == null)
            {
                // Set background based using sunset/rise times
                if (isNight(weather))
                    imgURI = new Uri("ms-appx:///Assets/Backgrounds/NightSky.jpg");
                else
                    imgURI = new Uri("ms-appx:///Assets/Backgrounds/DaySky.jpg");
            }

            BitmapImage img = new BitmapImage(imgURI);
            img.CreateOptions = BitmapCreateOptions.None;
            img.DecodePixelWidth = 960;
            bg.ImageSource = img;
        }
    }
}
