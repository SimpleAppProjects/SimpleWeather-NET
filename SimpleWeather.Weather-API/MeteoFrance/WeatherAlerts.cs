using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;

namespace SimpleWeather.Weather_API.MeteoFrance
{
    public static class MeteoFranceExtensions
    {
        public static ICollection<WeatherAlert> CreateWeatherAlerts(this MeteoFranceProvider _, AlertsRootobject alertRoot)
        {
            ICollection<WeatherAlert> weatherAlerts = null;

            if (alertRoot?.color_max > 1 && alertRoot?.phenomenons_items != null)
            {
                weatherAlerts = new HashSet<WeatherAlert>(alertRoot.phenomenons_items.Length);

                foreach (var phenom in alertRoot.phenomenons_items)
                {
                    // phenomenon_max_color_id == 1 -> OK / No Warning or Watches
                    if (phenom?.phenomenon_max_color_id != null && phenom.phenomenon_max_color_id > 1)
                    {
                        weatherAlerts.Add(_.CreateWeatherAlert(phenom, alertRoot));
                    }
                }
            }

            return weatherAlerts;
        }

        public static WeatherAlert CreateWeatherAlert(this MeteoFranceProvider _, Phenomenons_Items phenom, AlertsRootobject alertRoot)
        {
            var alert = new WeatherAlert();

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
                    alert.Severity = WeatherAlertSeverity.Minor;
                    message_fr = "Pas de vigilance particulière";
                    message_en = "No particular awareness of the weather is required.";
                    break;

                case 2:
                    alert.Severity = WeatherAlertSeverity.Moderate;
                    message_fr = "Soyez attentif";
                    message_en = "The weather is potentially dangerous.";
                    break;

                case 3:
                    alert.Severity = WeatherAlertSeverity.Severe;
                    message_fr = "Soyez très vigilant";
                    message_en = "The weather is dangerous.";
                    break;

                case 4:
                    alert.Severity = WeatherAlertSeverity.Extreme;
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
                    alert.Type = WeatherAlertType.HighWind;
                    title_fr = "Vent violent";
                    title_en = "High Winds";
                    break;

                case 2:
                    alert.Type = WeatherAlertType.FloodWatch;
                    title_fr = "Pluie-inondation";
                    title_en = "Rain/Flood";
                    break;

                case 3:
                    alert.Type = WeatherAlertType.SevereThunderstormWarning;
                    title_fr = "Orages";
                    title_en = "Thunderstorms";
                    break;

                case 4:
                    alert.Type = WeatherAlertType.FloodWarning;
                    title_fr = "Crues";
                    title_en = "Floods";
                    break;

                case 5:
                    alert.Type = WeatherAlertType.WinterWeather;
                    title_fr = "Neige-verglas";
                    title_en = "Snow/Ice";
                    break;

                case 6:
                    alert.Type = WeatherAlertType.Heat;
                    title_fr = "Canicule";
                    title_en = "Extreme high temperatures";
                    break;

                case 7:
                    alert.Type = WeatherAlertType.WinterWeather;
                    title_fr = "Grand-froid";
                    title_en = "Extreme low temperatures";
                    break;

                case 8:
                    alert.Type = WeatherAlertType.SpecialWeatherAlert;
                    title_fr = "Avalanches";
                    title_en = "Avalanches";
                    break;

                case 9:
                    alert.Type = WeatherAlertType.SpecialWeatherAlert;
                    title_fr = "Vagues-submersion";
                    title_en = "Coastal Event";
                    break;

                default:
                    alert.Type = WeatherAlertType.SpecialWeatherAlert;
                    title_fr = "Alerte météo";
                    title_en = "Weather Alert";
                    break;
            }

            alert.Title = title_fr;

            alert.Message = "français:" +
                    Environment.NewLine +
                    title_fr + " - " + message_fr +
                    Environment.NewLine +
                    Environment.NewLine +
                    "english:" +
                    Environment.NewLine +
                    title_en + " - " + message_en;

            alert.Date = DateTimeOffset.FromUnixTimeSeconds(alertRoot.update_time);
            alert.ExpiresDate = DateTimeOffset.FromUnixTimeSeconds(alertRoot.end_validity_time);

            alert.Attribution = "Meteo France";

            return alert;
        }
    }
}