using SimpleWeather.Controls;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SimpleWeather.WeatherYahoo
{
    public partial class YahooWeatherProvider : WeatherProviderImpl
    {
        public override string GetBackgroundURI(Weather weather)
        {
            String icon = weather.condition.icon;
            String imgURI = null;

            // Apply background based on weather condition
            if (int.TryParse(icon, out int code))
            {
                switch (code)
                {
                    // Night
                    case 31:
                    case 33:
                        imgURI = ("ms-appx:///Assets/Backgrounds/NightSky.jpg");
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
                        imgURI = ("ms-appx:///Assets/Backgrounds/RainySky.jpg");
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
                        imgURI = ("ms-appx:///Assets/Backgrounds/StormySky.jpg");
                        break;
                    // Dust
                    case 19:
                        imgURI = ("ms-appx:///Assets/Backgrounds/Dust.jpg");
                        break;
                    // Foggy / Haze
                    case 20:
                    case 21:
                    case 22:
                        imgURI = ("ms-appx:///Assets/Backgrounds/FoggySky.jpg");
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
                        imgURI = ("ms-appx:///Assets/Backgrounds/Snow.jpg");
                        break;
                    /* Ambigious weather conditions */
                    // (Mostly) Cloudy
                    case 28:
                    case 26:
                    case 27:
                        if (IsNight(weather))
                            imgURI = ("ms-appx:///Assets/Backgrounds/MostlyCloudy-Night.jpg");
                        else
                            imgURI = ("ms-appx:///Assets/Backgrounds/MostlyCloudy-Day.jpg");
                        break;
                    // Partly Cloudy
                    case 44:
                    case 29:
                    case 30:
                        if (IsNight(weather))
                            imgURI = ("ms-appx:///Assets/Backgrounds/PartlyCloudy-Night.jpg");
                        else
                            imgURI = ("ms-appx:///Assets/Backgrounds/PartlyCloudy-Day.jpg");
                        break;
                    case 3200:
                    default:
                        // Set background based using sunset/rise times
                        if (IsNight(weather))
                            imgURI = ("ms-appx:///Assets/Backgrounds/NightSky.jpg");
                        else
                            imgURI = ("ms-appx:///Assets/Backgrounds/DaySky.jpg");
                        break;
                }
            }

            // Just in case
            if (imgURI == null)
            {
                // Set background based using sunset/rise times
                if (IsNight(weather))
                    imgURI = ("ms-appx:///Assets/Backgrounds/NightSky.jpg");
                else
                    imgURI = ("ms-appx:///Assets/Backgrounds/DaySky.jpg");
            }

            return imgURI;
        }

        public override String GetWeatherIconURI(string icon)
        {
            string baseuri = "ms-appx:///Assets/WeatherIcons/png/";
            string fileIcon = string.Empty;

            if (int.TryParse(icon, out int code))
                fileIcon = string.Format("yahoo-{0}.png", code);

            if (String.IsNullOrWhiteSpace(fileIcon))
            {
                // Not Available
                fileIcon = "na.png";
            }

            return baseuri + fileIcon;
        }
    }
}
