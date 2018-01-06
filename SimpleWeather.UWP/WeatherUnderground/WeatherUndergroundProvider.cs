using System;
using SimpleWeather.WeatherData;

namespace SimpleWeather.WeatherUnderground
{
    public partial class WeatherUndergroundProvider : WeatherProviderImpl
    {
        public override string GetBackgroundURI(Weather weather)
        {
            String icon = weather.condition.icon;
            String imgURI = null;

            // Apply background based on weather condition
            if (icon.Contains("mostlysunny") || icon.Contains("partlysunny") ||
                icon.Contains("partlycloudy"))
            {
                if (IsNight(weather))
                    imgURI = ("ms-appx:///Assets/Backgrounds/PartlyCloudy-Night.jpg");
                else
                    imgURI = ("ms-appx:///Assets/Backgrounds/PartlyCloudy-Day.jpg");
            }
            else if (icon.Contains("cloudy"))
            {
                if (IsNight(weather))
                    imgURI = ("ms-appx:///Assets/Backgrounds/MostlyCloudy-Night.jpg");
                else
                    imgURI = ("ms-appx:///Assets/Backgrounds/MostlyCloudy-Day.jpg");
            }
            else if (icon.Contains("rain") || icon.Contains("sleet") || icon.Contains("sleat"))
            {
                imgURI = ("ms-appx:///Assets/Backgrounds/RainySky.jpg");
            }
            else if (icon.Contains("flurries") || icon.Contains("snow"))
            {
                imgURI = ("ms-appx:///Assets/Backgrounds/Snow.jpg");
            }
            else if (icon.Contains("tstorms"))
            {
                imgURI = ("ms-appx:///Assets/Backgrounds/StormySky.jpg");
            }
            else if (icon.Contains("hazy") || icon.Contains("fog"))
            {
                imgURI = ("ms-appx:///Assets/Backgrounds/FoggySky.jpg");
            }
            else if (icon.Contains("clear") || icon.Contains("sunny"))
            {
                // Set background based using sunset/rise times
                if (IsNight(weather))
                    imgURI = ("ms-appx:///Assets/Backgrounds/NightSky.jpg");
                else
                    imgURI = ("ms-appx:///Assets/Backgrounds/DaySky.jpg");
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

            if (icon.Contains("nt_mostlycloudy") || icon.Contains("nt_partlysunny") || icon.Contains("nt_cloudy"))
                fileIcon = "yahoo-27.png";
            else if (icon.Contains("nt_partlycloudy") || icon.Contains("nt_mostlysunny"))
                fileIcon = "yahoo-33.png";
            else if (icon.Contains("nt_clear") || icon.Contains("nt_sunny") || icon.Contains("nt_unknown"))
                fileIcon = "yahoo-31.png";
            else if (icon.Contains("chancerain"))
                fileIcon = "wu-chancerain.png";
            else if (icon.Contains("clear") || icon.Contains("sunny"))
                fileIcon = "wu-clear.png";
            else if (icon.Contains("cloudy"))
                fileIcon = "wu-cloudy.png";
            else if (icon.Contains("flurries"))
                fileIcon = "wu-flurries.png";
            else if (icon.Contains("fog"))
                fileIcon = "yahoo-20.png";
            else if (icon.Contains("hazy"))
                fileIcon = "yahoo-21.png";
            else if (icon.Contains("sleet") || icon.Contains("sleat"))
                fileIcon = "wu-sleat.png";
            else if (icon.Contains("rain"))
                fileIcon = "wu-rain.png";
            else if (icon.Contains("snow"))
                fileIcon = "wu-snow.png";
            else if (icon.Contains("tstorms"))
                fileIcon = "wu-tstorms.png";
            else if (icon.Contains("unknown"))
                fileIcon = "wu-unknown.png";
            else if (icon.Contains("nt_"))
                fileIcon = "yahoo-31.png";

            if (String.IsNullOrWhiteSpace(fileIcon))
            {
                // Not Available
                fileIcon = "na.png";
            }

            return baseuri + fileIcon;
        }
    }
}