using Microsoft.UI;
using SimpleWeather.Icons;
using Windows.UI;

namespace SimpleWeather.Utils
{
    public static partial class WeatherUtils
    {
        public static string GetAssetFromAlertType(WeatherData.WeatherAlertType type, bool isAbsoluteUri = true)
        {
            string baseuri = WeatherIconsManager.GetPNGBaseUri();
            string fileIcon = string.Empty;

            switch (type)
            {
                case WeatherData.WeatherAlertType.DenseFog:
                    fileIcon = "wi-fog.png";
                    break;
                case WeatherData.WeatherAlertType.Fire:
                    fileIcon = "wi-fire.png";
                    break;
                case WeatherData.WeatherAlertType.FloodWarning:
                case WeatherData.WeatherAlertType.FloodWatch:
                    fileIcon = "wi-flood.png";
                    break;
                case WeatherData.WeatherAlertType.Heat:
                    fileIcon = "wi-hot.png";
                    break;
                case WeatherData.WeatherAlertType.HighWind:
                    fileIcon = "wi-strong-wind.png";
                    break;
                case WeatherData.WeatherAlertType.HurricaneLocalStatement:
                case WeatherData.WeatherAlertType.HurricaneWindWarning:
                    fileIcon = "wi-hurricane.png";
                    break;
                case WeatherData.WeatherAlertType.SevereThunderstormWarning:
                case WeatherData.WeatherAlertType.SevereThunderstormWatch:
                    fileIcon = "wi-thunderstorm.png";
                    break;
                case WeatherData.WeatherAlertType.TornadoWarning:
                case WeatherData.WeatherAlertType.TornadoWatch:
                    fileIcon = "wi-tornado.png";
                    break;
                case WeatherData.WeatherAlertType.Volcano:
                    fileIcon = "wi-volcano.png";
                    break;
                case WeatherData.WeatherAlertType.WinterWeather:
                    fileIcon = "wi-snowflake-cold.png";
                    break;
                case WeatherData.WeatherAlertType.DenseSmoke:
                    fileIcon = "wi-smoke.png";
                    break;
                case WeatherData.WeatherAlertType.DustAdvisory:
                    fileIcon = "wi-dust.png";
                    break;
                case WeatherData.WeatherAlertType.EarthquakeWarning:
                    fileIcon = "wi-earthquake.png";
                    break;
                case WeatherData.WeatherAlertType.GaleWarning:
                    fileIcon = "wi-gale-warning.png";
                    break;
                case WeatherData.WeatherAlertType.SmallCraft:
                    fileIcon = "wi-small-craft-advisory.png";
                    break;
                case WeatherData.WeatherAlertType.StormWarning:
                    fileIcon = "wi-storm-warning.png";
                    break;
                case WeatherData.WeatherAlertType.TsunamiWarning:
                case WeatherData.WeatherAlertType.TsunamiWatch:
                    fileIcon = "wi-tsunami.png";
                    break;
                case WeatherData.WeatherAlertType.SevereWeather:
                case WeatherData.WeatherAlertType.SpecialWeatherAlert:
                default:
                    fileIcon = "ic_error.png";
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
