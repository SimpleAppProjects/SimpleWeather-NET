using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace SimpleWeather.Utils
{
    public static partial class WeatherUtils
    {
        public static string GetAssetFromAlertType(WeatherData.WeatherAlertType type, bool isAbsoluteUri = true)
        {
            string baseuri = "ms-appx:///SimpleWeather.Shared/Assets/WeatherIcons/png/";
            string fileIcon = string.Empty;

            switch (type)
            {
                case WeatherData.WeatherAlertType.DenseFog:
                    fileIcon = "fog.png";
                    break;
                case WeatherData.WeatherAlertType.Fire:
                    fileIcon = "fire.png";
                    break;
                case WeatherData.WeatherAlertType.FloodWarning:
                case WeatherData.WeatherAlertType.FloodWatch:
                    fileIcon = "flood.png";
                    break;
                case WeatherData.WeatherAlertType.Heat:
                    fileIcon = "hot.png";
                    break;
                case WeatherData.WeatherAlertType.HighWind:
                    fileIcon = "strong_wind.png";
                    break;
                case WeatherData.WeatherAlertType.HurricaneLocalStatement:
                case WeatherData.WeatherAlertType.HurricaneWindWarning:
                    fileIcon = "hurricane.png";
                    break;
                case WeatherData.WeatherAlertType.SevereThunderstormWarning:
                case WeatherData.WeatherAlertType.SevereThunderstormWatch:
                    fileIcon = "thunderstorm.png";
                    break;
                case WeatherData.WeatherAlertType.TornadoWarning:
                case WeatherData.WeatherAlertType.TornadoWatch:
                    fileIcon = "tornado.png";
                    break;
                case WeatherData.WeatherAlertType.Volcano:
                    fileIcon = "volcano.png";
                    break;
                case WeatherData.WeatherAlertType.WinterWeather:
                    fileIcon = "snowflake_cold.png";
                    break;
                case WeatherData.WeatherAlertType.DenseSmoke:
                    fileIcon = "smoke.png";
                    break;
                case WeatherData.WeatherAlertType.DustAdvisory:
                    fileIcon = "dust.png";
                    break;
                case WeatherData.WeatherAlertType.EarthquakeWarning:
                    fileIcon = "earthquake.png";
                    break;
                case WeatherData.WeatherAlertType.GaleWarning:
                    fileIcon = "gale_warning.png";
                    break;
                case WeatherData.WeatherAlertType.SmallCraft:
                    fileIcon = "small_craft_advisory.png";
                    break;
                case WeatherData.WeatherAlertType.StormWarning:
                    fileIcon = "storm_warning.png";
                    break;
                case WeatherData.WeatherAlertType.TsunamiWarning:
                case WeatherData.WeatherAlertType.TsunamiWatch:
                    fileIcon = "tsunami.png";
                    break;
                case WeatherData.WeatherAlertType.SevereWeather:
                case WeatherData.WeatherAlertType.SpecialWeatherAlert:
                default:
                    fileIcon = "ic_error_white.png";
                    break;
            }

            if (isAbsoluteUri)
            {
                return baseuri + fileIcon;
            }
            else
            {
                return fileIcon;
            }
        }

        public static Color GetColorFromAlertSeverity(WeatherData.WeatherAlertSeverity severity)
        {
            Color color;

            switch (severity)
            {
                case WeatherData.WeatherAlertSeverity.Severe:
                    color = Colors.OrangeRed;
                    break;
                case WeatherData.WeatherAlertSeverity.Extreme:
                    color = Colors.Red;
                    break;
                case WeatherData.WeatherAlertSeverity.Moderate:
                default:
                    color = Colors.Orange;
                    break;
            }

            return color;
        }
    }
}
