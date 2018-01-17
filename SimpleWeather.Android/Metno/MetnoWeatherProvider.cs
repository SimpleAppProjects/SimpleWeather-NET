using SimpleWeather.Droid;
using SimpleWeather.WeatherData;
using System;

namespace SimpleWeather.Metno
{
    public partial class MetnoWeatherProvider : WeatherProviderImpl
    {
        public override string GetBackgroundURI(Weather weather)
        {
            String icon = weather.condition.icon;
            String file = null;

            // Apply background based on weather condition
            switch (icon)
            {
                case "2": // LightCloud
                case "3": // PartlyCloud
                    if (IsNight(weather))
                        file = "file:///android_asset/backgrounds/PartlyCloudy-Night.jpg";
                    else
                        file = "file:///android_asset/backgrounds/PartlyCloudy-Day.jpg";
                    break;

                case "4": // Cloud
                    if (IsNight(weather))
                        file = "file:///android_asset/backgrounds/MostlyCloudy-Night.jpg";
                    else
                        file = "file:///android_asset/backgrounds/MostlyCloudy-Day.jpg";
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
                    file = "file:///android_asset/backgrounds/RainySky.jpg";
                    break;

                case "6": // LightRainThunderSun
                case "11": // RainThunder
                case "22": // LightRainThunder
                case "24": // DrizzleThunderSun
                case "25": // RainThunderSun
                case "30": // DrizzleThunder
                    file = "file:///android_asset/backgrounds/StormySky.jpg";
                    break;

                case "15": // Fog
                    file = "file:///android_asset/backgrounds/FoggySky.jpg";
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
                    file = "file:///android_asset/backgrounds/Snow.jpg";
                    break;

                case "1": // Sun
                default:
                    // Set background based using sunset/rise times
                    if (IsNight(weather))
                        file = "file:///android_asset/backgrounds/NightSky.jpg";
                    else
                        file = "file:///android_asset/backgrounds/DaySky.jpg";
                    break;
            }

            return file;
        }

        public override int GetWeatherIconResource(string icon)
        {
            int drawable = -1;
            string WeatherIcon = String.Empty;

            switch (icon)
            {
                case "1": // Sun
                    WeatherIcon = "yahoo_32";
                    break;

                case "2": // LightCloud
                case "3": // PartlyCloud
                    WeatherIcon = "yahoo_30";
                    break;

                case "4": // Cloud
                    WeatherIcon = "yahoo_26";
                    break;

                case "5": // LightRainSun
                    WeatherIcon = "owm_321";
                    break;

                case "6": // LightRainThunderSun
                    WeatherIcon = "owm_521";
                    break;

                case "7": // SleetSun
                    WeatherIcon = "owm_602";
                    break;

                case "8": // SnowSun
                    WeatherIcon = "owm_601";
                    break;

                case "9": // LightRain
                    WeatherIcon = "owm_321";
                    break;

                case "10": // Rain
                    WeatherIcon = "owm_501";
                    break;

                case "11": // RainThunder
                    WeatherIcon = "owm_901";
                    break;

                case "12": // Sleet
                    WeatherIcon = "owm_602";
                    break;

                case "13": // Snow
                    WeatherIcon = "owm_601";
                    break;

                case "14": // SnowThunder
                    WeatherIcon = "owm_601";
                    break;

                case "15": // Fog
                    WeatherIcon = "owm_741";
                    break;

                case "20": // SleetSunThunder
                    WeatherIcon = "owm_602";
                    break;

                case "21": // SnowSunThunder
                    WeatherIcon = "owm_601";
                    break;

                case "22": // LightRainThunder
                case "30": // DrizzleThunder
                    WeatherIcon = "owm_531";
                    break;

                case "23": // SleetThunder
                    WeatherIcon = "owm_602";
                    break;

                case "24": // DrizzleThunderSun
                case "25": // RainThunderSun
                    WeatherIcon = "yahoo_1";
                    break;

                case "26": // LightSleetThunderSun
                case "27": // HeavySleetThunderSun
                    WeatherIcon = "owm_602";
                    break;

                case "28": // LightSnowThunderSun
                case "29": // HeavySnowThunderSun
                    WeatherIcon = "owm_601";
                    break;

                case "31": // LightSleetThunder
                case "32": // HeavySleetThunder
                    WeatherIcon = "owm_602";
                    break;

                case "33": // LightSnowThunder
                case "34": // HeavySnowThunder
                    WeatherIcon = "owm_601";
                    break;

                case "40": // DrizzleSun
                case "41": // RainSun
                    WeatherIcon = "owm_301";
                    break;

                case "42": // LightSleetSun
                case "43": // HeavySleetSun
                    WeatherIcon = "owm_602";
                    break;

                case "44": // LightSnowSun
                case "45": // HeavySnowSun
                    WeatherIcon = "owm_601";
                    break;

                case "46": // Drizzle
                    WeatherIcon = "owm_301";
                    break;

                case "47": // LightSleet
                case "48": // HeavySleet
                    WeatherIcon = "owm_602";
                    break;

                case "49": // LightSnow
                case "50": // HeavySnow
                    WeatherIcon = "owm_601";
                    break;
            }

            if (!String.IsNullOrWhiteSpace(WeatherIcon))
            {
                object value = typeof(Resource.Drawable).GetField(WeatherIcon).GetValue(null);
                if (value != null)
                    drawable = (int)value;
            }

            if (drawable == -1)
            {
                // Not Available
                drawable = Resource.Drawable.na;
            }

            return drawable;
        }
    }
}