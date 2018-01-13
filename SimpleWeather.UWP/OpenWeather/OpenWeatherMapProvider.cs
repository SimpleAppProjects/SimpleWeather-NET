using System;
using SimpleWeather.WeatherData;

namespace SimpleWeather.OpenWeather
{
    public partial class OpenWeatherMapProvider : WeatherProviderImpl
    {
        public override string GetBackgroundURI(WeatherData.Weather weather)
        {
            String icon = weather.condition.icon;
            String imgURI = null;

            // Apply background based on weather condition
            switch (icon)
            {
                // drizzle
                case "300":
                case "301":
                case "321":
                case "500":
                // rain
                case "302":
                case "311":
                case "312":
                case "314":
                case "501":
                case "502":
                case "503":
                case "504":
                // rain-mix
                case "310":
                case "511":
                case "611":
                case "612":
                case "615":
                case "616":
                case "620":
                // rain showers
                case "313":
                case "520":
                case "521":
                case "522":
                case "701":
                // sleet
                case "602":
                // hail
                case "906":
                    imgURI = ("ms-appx:///Assets/Backgrounds/RainySky.jpg");
                    break;
                // thunderstorm
                case "200":
                case "201":
                case "202":
                case "230":
                case "231":
                case "232":
                // lightning
                case "210":
                case "211":
                case "212":
                case "221":
                // storm-showers
                case "531":
                case "901":
                // tornado
                case "781":
                case "900":
                // hurricane
                case "902":
                    imgURI = ("ms-appx:///Assets/Backgrounds/StormySky.jpg");
                    break;
                // dust
                case "731":
                case "761":
                case "762":
                    imgURI = ("ms-appx:///Assets/Backgrounds/Dust.jpg");
                    break;
                // smoke
                case "711":
                // haze
                case "721":
                // fog
                case "741":
                    imgURI = ("ms-appx:///Assets/Backgrounds/FoggySky.jpg");
                    break;
                // snow
                case "600":
                case "601":
                case "621":
                case "622":
                    imgURI = ("ms-appx:///Assets/Backgrounds/Snow.jpg");
                    break;
                // cloudy-gusts
                case "771":
                case "801":
                case "802":
                // cloudy-gusts
                case "803":
                // cloudy
                case "804":
                    if (IsNight(weather))
                        imgURI = ("ms-appx:///Assets/Backgrounds/MostlyCloudy-Night.jpg");
                    else
                        imgURI = ("ms-appx:///Assets/Backgrounds/MostlyCloudy-Day.jpg");
                    break;
                // day-sunny
                case "800":
                // cold
                case "903":
                // hot
                case "904":
                // windy
                case "905":
                // strong wind
                case "957":
                default:
                    // Set background based using sunset/rise times
                    if (IsNight(weather))
                        imgURI = ("ms-appx:///Assets/Backgrounds/NightSky.jpg");
                    else
                        imgURI = ("ms-appx:///Assets/Backgrounds/DaySky.jpg");
                    break;
            }

            return imgURI;
        }

        public override String GetWeatherIconURI(string icon)
        {
            string baseuri = "ms-appx:///Assets/WeatherIcons/png/";
            string fileIcon = string.Empty;

            if (int.TryParse(icon, out int code))
                fileIcon = string.Format("owm_{0}.png", code);

            if (String.IsNullOrWhiteSpace(fileIcon))
            {
                // Not Available
                fileIcon = "na.png";
            }

            return baseuri + fileIcon;
        }
    }
}