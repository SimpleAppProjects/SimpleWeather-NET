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
        public static string GetAssetFromAlertType(WeatherData.WeatherAlertType type, bool isAbsoluteUri = true, bool isLight = false)
        {
            var wip = SharedModule.Instance.WeatherIconsManager.GetIconProvider(WeatherIconsEFProvider.KEY);
            string baseuri = wip.GetPNGBaseUri(isLight);
            string fileIcon = type switch
            {
                WeatherData.WeatherAlertType.DenseFog => "wi_fog.png",
                WeatherData.WeatherAlertType.Fire => "wi_fire.png",
                WeatherData.WeatherAlertType.FloodWarning or WeatherData.WeatherAlertType.FloodWatch => "wi_flood.png",
                WeatherData.WeatherAlertType.Heat => "wi_hot.png",
                WeatherData.WeatherAlertType.HighWind => "wi_strong_wind.png",
                WeatherData.WeatherAlertType.HurricaneLocalStatement or WeatherData.WeatherAlertType.HurricaneWindWarning => "wi_hurricane.png",
                WeatherData.WeatherAlertType.SevereThunderstormWarning or WeatherData.WeatherAlertType.SevereThunderstormWatch => "wi_thunderstorm.png",
                WeatherData.WeatherAlertType.TornadoWarning or WeatherData.WeatherAlertType.TornadoWatch => "wi_tornado.png",
                WeatherData.WeatherAlertType.Volcano => "wi_volcano.png",
                WeatherData.WeatherAlertType.WinterWeather => "wi_snowflake_cold.png",
                WeatherData.WeatherAlertType.DenseSmoke => "wi_smoke.png",
                WeatherData.WeatherAlertType.DustAdvisory => "wi_dust.png",
                WeatherData.WeatherAlertType.EarthquakeWarning => "wi_earthquake.png",
                WeatherData.WeatherAlertType.GaleWarning => "wi_gale_warning.png",
                WeatherData.WeatherAlertType.SmallCraft => "wi_small_craft_advisory.png",
                WeatherData.WeatherAlertType.StormWarning => "wi_storm_warning.png",
                WeatherData.WeatherAlertType.TsunamiWarning or WeatherData.WeatherAlertType.TsunamiWatch => "wi_tsunami.png",
                _ => "ic_error.png",
            };

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
            return severity switch
            {
                WeatherData.WeatherAlertSeverity.Severe => Colors.OrangeRed,
                WeatherData.WeatherAlertSeverity.Extreme => Colors.Red,
                _ => Colors.Orange,
            };
        }
#else
        public static Color GetColorFromAlertSeverity(WeatherData.WeatherAlertSeverity severity)
        {
            return severity switch
            {
                WeatherData.WeatherAlertSeverity.Severe => Colors.OrangeRed,
                WeatherData.WeatherAlertSeverity.Extreme => Colors.Red,
                _ => Colors.Orange,
            };
        }
#endif
    }
}
