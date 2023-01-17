using SimpleWeather.Icons;
#if WINUI
using Microsoft.UI;
using Windows.UI;
#else
using Microsoft.Maui.Graphics;
#endif

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
                    fileIcon = "wi_fog.png";
                    break;
                case WeatherData.WeatherAlertType.Fire:
                    fileIcon = "wi_fire.png";
                    break;
                case WeatherData.WeatherAlertType.FloodWarning:
                case WeatherData.WeatherAlertType.FloodWatch:
                    fileIcon = "wi_flood.png";
                    break;
                case WeatherData.WeatherAlertType.Heat:
                    fileIcon = "wi_hot.png";
                    break;
                case WeatherData.WeatherAlertType.HighWind:
                    fileIcon = "wi_strong_wind.png";
                    break;
                case WeatherData.WeatherAlertType.HurricaneLocalStatement:
                case WeatherData.WeatherAlertType.HurricaneWindWarning:
                    fileIcon = "wi_hurricane.png";
                    break;
                case WeatherData.WeatherAlertType.SevereThunderstormWarning:
                case WeatherData.WeatherAlertType.SevereThunderstormWatch:
                    fileIcon = "wi_thunderstorm.png";
                    break;
                case WeatherData.WeatherAlertType.TornadoWarning:
                case WeatherData.WeatherAlertType.TornadoWatch:
                    fileIcon = "wi_tornado.png";
                    break;
                case WeatherData.WeatherAlertType.Volcano:
                    fileIcon = "wi_volcano.png";
                    break;
                case WeatherData.WeatherAlertType.WinterWeather:
                    fileIcon = "wi_snowflake_cold.png";
                    break;
                case WeatherData.WeatherAlertType.DenseSmoke:
                    fileIcon = "wi_smoke.png";
                    break;
                case WeatherData.WeatherAlertType.DustAdvisory:
                    fileIcon = "wi_dust.png";
                    break;
                case WeatherData.WeatherAlertType.EarthquakeWarning:
                    fileIcon = "wi_earthquake.png";
                    break;
                case WeatherData.WeatherAlertType.GaleWarning:
                    fileIcon = "wi_gale_warning.png";
                    break;
                case WeatherData.WeatherAlertType.SmallCraft:
                    fileIcon = "wi_small_craft_advisory.png";
                    break;
                case WeatherData.WeatherAlertType.StormWarning:
                    fileIcon = "wi_storm_warning.png";
                    break;
                case WeatherData.WeatherAlertType.TsunamiWarning:
                case WeatherData.WeatherAlertType.TsunamiWatch:
                    fileIcon = "wi_tsunami.png";
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

#if WINUI
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
#else
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
#endif
    }
}
