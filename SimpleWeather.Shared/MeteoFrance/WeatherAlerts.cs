using SimpleWeather.MeteoFrance;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;

namespace SimpleWeather.WeatherData
{
    public partial class WeatherAlert
    {
        public WeatherAlert(Phenomenons_Items phenom)
        {
            String title_en, title_fr;
            String message_en, message_fr;

            /*
             * phenomenon_max_color_id (Severity)
             *
             * 1 - Green
             * 2 - Yellow
             * 3 - Orange
             * 4 - Red
             */
            switch (phenom.phenomenon_max_color_id)
            {
                case 1:
                default:
                    Severity = WeatherAlertSeverity.Minor;
                    message_fr = "Pas de vigilance particulière";
                    message_en = "No particular awareness of the weather is required.";
                    break;

                case 2:
                    Severity = WeatherAlertSeverity.Moderate;
                    message_fr = "Soyez attentif";
                    message_en = "The weather is potentially dangerous.";
                    break;

                case 3:
                    Severity = WeatherAlertSeverity.Severe;
                    message_fr = "Soyez très vigilant";
                    message_en = "The weather is dangerous.";
                    break;

                case 4:
                    Severity = WeatherAlertSeverity.Extreme;
                    message_fr = "Une vigilance Absolue s'impose";
                    message_en = "The weather is very dangerous.";
                    break;
            }

            /*
             * phenomenon_id (Alert Type)
             *
             * 1 - Wind
             * 2 - Rain/Flood
             * 3 - Thunderstorm
             * 4 - Flood
             * 5 - Snow/Ice
             * 6 - Extreme high temp
             * 7 - Extreme low temp
             * 8 - Avalanches
             * 9 - Coastal event
             */
            switch (phenom.phenomenon_id)
            {
                case 1:
                    Type = WeatherAlertType.HighWind;
                    title_fr = "Vent violent";
                    title_en = "High Winds";
                    break;

                case 2:
                    Type = WeatherAlertType.FloodWatch;
                    title_fr = "Pluie-inondation";
                    title_en = "Rain/Flood";
                    break;

                case 3:
                    Type = WeatherAlertType.SevereThunderstormWarning;
                    title_fr = "Orages";
                    title_en = "Thunderstorms";
                    break;

                case 4:
                    Type = WeatherAlertType.FloodWarning;
                    title_fr = "Crues";
                    title_en = "Floods";
                    break;

                case 5:
                    Type = WeatherAlertType.WinterWeather;
                    title_fr = "Neige-verglas";
                    title_en = "Snow/Ice";
                    break;

                case 6:
                    Type = WeatherAlertType.Heat;
                    title_fr = "Canicule";
                    title_en = "Extreme high temperatures";
                    break;

                case 7:
                    Type = WeatherAlertType.WinterWeather;
                    title_fr = "Grand-froid";
                    title_en = "Extreme low temperatures";
                    break;

                case 8:
                    Type = WeatherAlertType.SpecialWeatherAlert;
                    title_fr = "Avalanches";
                    title_en = "Avalanches";
                    break;

                case 9:
                    Type = WeatherAlertType.SpecialWeatherAlert;
                    title_fr = "Vagues-submersion";
                    title_en = "Coastal Event";
                    break;

                default:
                    Type = WeatherAlertType.SpecialWeatherAlert;
                    title_fr = "Alerte météo";
                    title_en = "Weather Alert";
                    break;
            }

            Title = title_fr;

            Message = "français:" +
                    Environment.NewLine +
                    title_fr + " - " + message_fr +
                    Environment.NewLine +
                    Environment.NewLine +
                    "english:" +
                    Environment.NewLine +
                    title_en + " - " + message_en;

            Attribution = "Meteo France";
        }
    }
}