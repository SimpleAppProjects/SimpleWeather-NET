using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace SimpleWeather
{
    public static class WeatherUtils
    {
        public static String GetWeatherIcon(string weather_code)
        {
            int weatherCode = int.Parse(weather_code);
            string WeatherIcon;

            switch (weatherCode)
            {
                case 0: // Tornado
                    WeatherIcon = "\uf056";
                    break;
                case 1: // Tropical Storm
                case 37:
                case 38: // Scattered Thunderstorms/showers
                case 39:
                case 45:
                case 47:
                    WeatherIcon = "\uf00e";
                    break;
                case 2: // Hurricane
                    WeatherIcon = "\uf073";
                    break;
                case 3:
                case 4: // Scattered Thunderstorms
                    WeatherIcon = "\uf01e";
                    break;
                case 5: // Mixed Rain/Snow
                case 6: // Mixed Rain/Sleet
                case 7: // Mixed Snow/Sleet
                case 18: // Sleet
                case 35: // Mixed Rain/Hail
                    WeatherIcon = "\uf017";
                    break;
                case 8: // Freezing Drizzle
                case 10: // Freezing Rain
                case 17: // Hail
                    WeatherIcon = "\uf015";
                    break;
                case 9: // Drizzle
                case 11: // Showers
                case 12:
                case 40: // Scattered Showers
                    WeatherIcon = "\uf01a";
                    break;
                case 13: // Snow Flurries
                case 16: // Snow
                case 42: // Scattered Snow Showers
                case 46: // Snow Showers
                    WeatherIcon = "\uf01b";
                    break;
                case 15: // Blowing Snow
                case 41: // Heavy Snow
                case 43:
                    WeatherIcon = "\uf064";
                    break;
                case 19: // Dust
                    WeatherIcon = "\uf063";
                    break;
                case 20: // Foggy
                    WeatherIcon = "\uf014";
                    break;
                case 21: // Haze
                    WeatherIcon = "\uf021";
                    break;
                case 22: // Smoky
                    WeatherIcon = "\uf062";
                    break;
                case 23: // Blustery
                case 24: // Windy
                    WeatherIcon = "\uf050";
                    break;
                case 25: // Cold
                    WeatherIcon = "\uf076";
                    break;
                case 26: // Cloudy
                    WeatherIcon = "\uf013";
                    break;
                case 27: // Mostly Cloudy (Night)
                case 29: // Partly Cloudy (Night)
                    WeatherIcon = "\uf031";
                    break;
                case 28: // Mostly Cloudy (Day)
                case 30: // Partly Cloudy (Day)
                    WeatherIcon = "\uf002";
                    break;
                case 31: // Clear (Night)
                    WeatherIcon = "\uf02e";
                    break;
                case 32: // Sunny
                    WeatherIcon = "\uf00d";
                    break;
                case 33: // Fair (Night)
                    WeatherIcon = "\uf083";
                    break;
                case 34: // Fair (Day)
                case 44: // Partly Cloudy
                    WeatherIcon = "\uf00c";
                    break;
                case 36: // HOT
                    WeatherIcon = "\uf072";
                    break;
                case 3200: // Not Available
                default:
                    WeatherIcon = "\uf077";
                    break;
            }

            return WeatherIcon;
        }

        public static ImageBrush GetBackground(Weather weather)
        {
            ImageBrush bg = new ImageBrush();
            bg.Stretch = Stretch.UniformToFill;
            bg.AlignmentX = AlignmentX.Right;
            Windows.UI.Xaml.Media.Imaging.BitmapImage img;

            // Apply background based on weather condition
            switch (int.Parse(weather.condition.code))
            {
                // Night
                case 31:
                case 33:
                    App.backgroundImages.TryGetValue("NightSky", out img);
                    bg.ImageSource = img;
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
                    App.backgroundImages.TryGetValue("RainySky", out img);
                    bg.ImageSource = img;
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
                    App.backgroundImages.TryGetValue("StormySky", out img);
                    bg.ImageSource = img;
                    break;
                // Dust
                case 19:
                    App.backgroundImages.TryGetValue("Dust", out img);
                    bg.ImageSource = img;
                    break;
                // Foggy / Haze
                case 20:
                case 21:
                case 22:
                    App.backgroundImages.TryGetValue("FoggySky", out img);
                    bg.ImageSource = img;
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
                    App.backgroundImages.TryGetValue("Snow", out img);
                    bg.ImageSource = img;
                    break;
                /* Ambigious weather conditions */
                // (Mostly) Cloudy
                case 28:
                case 26:
                case 27:
                    if (isNight(weather))
                    {
                        App.backgroundImages.TryGetValue("MostlyCloudy-Night", out img);
                        bg.ImageSource = img;
                    }
                    else
                    {
                        App.backgroundImages.TryGetValue("MostlyCloudy-Day", out img);
                        bg.ImageSource = img;
                    }
                    break;
                // Partly Cloudy
                case 44:
                case 29:
                case 30:
                    if (isNight(weather))
                    {
                        App.backgroundImages.TryGetValue("PartlyCloudy-Night", out img);
                        bg.ImageSource = img;
                    }
                    else
                    {
                        App.backgroundImages.TryGetValue("PartlyCloudy-Day", out img);
                        bg.ImageSource = img;
                    }
                    break;
            }

            if (bg.ImageSource == null)
            {
                // Set background based using sunset/rise times
                if (isNight(weather))
                {
                    App.backgroundImages.TryGetValue("NightSky", out img);
                    bg.ImageSource = img;
                }
                else
                {
                    App.backgroundImages.TryGetValue("DaySky", out img);
                    bg.ImageSource = img;
                }
            }

            return bg;
        }

        private static bool isNight(Weather weather)
        {
            // Determine whether its night using sunset/rise times
            if (DateTime.Now < DateTime.Parse(weather.astronomy.sunrise)
                    || DateTime.Now > DateTime.Parse(weather.astronomy.sunset))
                return true;
            else
                return false;
        }

        public static String GetLastBuildDate(Weather weather)
        {
            String date;

            // ex. "2016-08-22T04:53:07Z"
            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime updateTime = DateTime.ParseExact(weather.lastBuildDate,
                "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", provider).ToLocalTime();

            if (updateTime.DayOfWeek == DateTime.Today.DayOfWeek)
            {
                date = "Updated at " + updateTime.ToString("t");
            }
            else
                date = "Updated on " + updateTime.ToString("ddd") + " " + updateTime.ToString("t");

            return date;
        }
    }
}
