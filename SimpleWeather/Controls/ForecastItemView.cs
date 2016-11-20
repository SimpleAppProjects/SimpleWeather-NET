using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather
{
    public class ForecastItemView
    {
        public string WeatherIcon { get; set; }
        public string Date { get; set; }
        public string Condition { get; set; }
        public string HiTemp { get; set; }
        public string LoTemp { get; set; }

        public ForecastItemView(Forecast forecast)
        {
            updateWeatherIcon(int.Parse(forecast.code));
            Date = forecast.date;
            Condition = forecast.text;
            HiTemp = forecast.high + "º ";
            LoTemp = forecast.low + "º";
        }

        private void updateWeatherIcon(int weatherCode)
        {
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
        }
    }
}
