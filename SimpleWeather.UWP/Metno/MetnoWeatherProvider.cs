using System;
using SimpleWeather.WeatherData;

namespace SimpleWeather.Metno
{
    public partial class MetnoWeatherProvider : WeatherProviderImpl
    {
        public override string GetBackgroundURI(Weather weather)
        {
            String icon = weather.condition.icon;
            String imgURI = null;

            // Apply background based on weather condition
            switch (icon)
            {
                case "2": // LightCloud
                case "3": // PartlyCloud
                    if (IsNight(weather))
                        imgURI = ("ms-appx:///Assets/Backgrounds/PartlyCloudy-Night.jpg");
                    else
                        imgURI = ("ms-appx:///Assets/Backgrounds/PartlyCloudy-Day.jpg");
                    break;

                case "4": // Cloud
                    if (IsNight(weather))
                        imgURI = ("ms-appx:///Assets/Backgrounds/MostlyCloudy-Night.jpg");
                    else
                        imgURI = ("ms-appx:///Assets/Backgrounds/MostlyCloudy-Day.jpg");
                    break;

                case "5": // LightRainSun
                case "7": // SleetSun
                case "9": // LightRain
                case "10": // Rain
                case "12": // Sleet

                case "20": // SleetSunThunder
                case "23": // SleetThunder
                case "26": // LightSleetThunderSun
                case "27": // HeavySleetThunderSun

                case "31": // LightSleetThunder
                case "32": // HeavySleetThunder

                case "40": // DrizzleSun
                case "41": // RainSun
                case "42": // LightSleetSun
                case "43": // HeavySleetSun
                case "46": // Drizzle
                case "47": // LightSleet
                case "48": // HeavySleet
                    imgURI = ("ms-appx:///Assets/Backgrounds/RainySky.jpg");
                    break;

                case "6": // LightRainThunderSun
                case "11": // RainThunder
                case "22": // LightRainThunder
                case "24": // DrizzleThunderSun
                case "25": // RainThunderSun
                case "30": // DrizzleThunder
                    imgURI = ("ms-appx:///Assets/Backgrounds/StormySky.jpg");
                    break;

                case "15": // Fog
                    imgURI = ("ms-appx:///Assets/Backgrounds/FoggySky.jpg");
                    break;

                case "8": // SnowSun
                case "13": // Snow
                case "14": // SnowThunder

                case "21": // SnowSunThunder
                case "28": // LightSnowThunderSun
                case "29": // HeavySnowThunderSun

                case "33": // LightSnowThunder
                case "34": // HeavySnowThunder

                case "44": // LightSnowSun
                case "45": // HeavySnowSun
                case "49": // LightSnow
                case "50": // HeavySnow
                    imgURI = ("ms-appx:///Assets/Backgrounds/Snow.jpg");
                    break;

                case "1": // Sun
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
            string baseuri = "ms-appx:///Assets/WeatherIcon/png/";
            string fileIcon = string.Empty;

            switch (icon)
            {
                case "1": // Sun
                    fileIcon = "yahoo-32.png";
                    break;

                case "2": // LightCloud
                case "3": // PartlyCloud
                    fileIcon = "yahoo-30.png";
                    break;

                case "4": // Cloud
                    fileIcon = "yahoo-26.png";
                    break;

                case "5": // LightRainSun
                    fileIcon = "owm_321.png";
                    break;

                case "6": // LightRainThunderSun
                    fileIcon = "owm_521.png";
                    break;

                case "7": // SleetSun
                    fileIcon = "owm_602.png";
                    break;

                case "8": // SnowSun
                    fileIcon = "owm_601.png";
                    break;

                case "9": // LightRain
                    fileIcon = "owm_321.png";
                    break;

                case "10": // Rain
                    fileIcon = "owm_501.png";
                    break;

                case "11": // RainThunder
                    fileIcon = "owm_901.png";
                    break;

                case "12": // Sleet
                    fileIcon = "owm_602.png";
                    break;

                case "13": // Snow
                    fileIcon = "owm_601.png";
                    break;

                case "14": // SnowThunder
                    fileIcon = "owm_601.png";
                    break;

                case "15": // Fog
                    fileIcon = "owm_741.png";
                    break;

                case "20": // SleetSunThunder
                    fileIcon = "owm_602.png";
                    break;

                case "21": // SnowSunThunder
                    fileIcon = "owm_601.png";
                    break;

                case "22": // LightRainThunder
                case "30": // DrizzleThunder
                    fileIcon = "owm_531.png";
                    break;

                case "23": // SleetThunder
                    fileIcon = "owm_602.png";
                    break;

                case "24": // DrizzleThunderSun
                case "25": // RainThunderSun
                    fileIcon = "yahoo-1.png";
                    break;

                case "26": // LightSleetThunderSun
                case "27": // HeavySleetThunderSun
                    fileIcon = "owm_602.png";
                    break;

                case "28": // LightSnowThunderSun
                case "29": // HeavySnowThunderSun
                    fileIcon = "owm_601.png";
                    break;

                case "31": // LightSleetThunder
                case "32": // HeavySleetThunder
                    fileIcon = "owm_602.png";
                    break;

                case "33": // LightSnowThunder
                case "34": // HeavySnowThunder
                    fileIcon = "owm_601.png";
                    break;

                case "40": // DrizzleSun
                case "41": // RainSun
                    fileIcon = "owm_301.png";
                    break;

                case "42": // LightSleetSun
                case "43": // HeavySleetSun
                    fileIcon = "owm_602.png";
                    break;

                case "44": // LightSnowSun
                case "45": // HeavySnowSun
                    fileIcon = "owm_601.png";
                    break;

                case "46": // Drizzle
                    fileIcon = "owm_301.png";
                    break;

                case "47": // LightSleet
                case "48": // HeavySleet
                    fileIcon = "owm_602.png";
                    break;

                case "49": // LightSnow
                case "50": // HeavySnow
                    fileIcon = "owm_601.png";
                    break;
            }

            if (String.IsNullOrWhiteSpace(fileIcon))
            {
                // Not Available
                fileIcon = "na.png";
            }

            return baseuri + fileIcon;
        }
    }
}