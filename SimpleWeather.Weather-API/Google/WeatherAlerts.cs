#nullable enable
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SimpleWeather.Weather_API.Google
{
    public static partial class GoogleWeatherProviderExtensions
    {
        public static ICollection<WeatherAlert>? CreateWeatherAlerts(this GoogleWeatherProvider _, AlertsResponse? response)
        {
            return response?.weatherAlerts?.Select(it => _.CreateWeatherAlert(it))?.ToList();
        }

        public static WeatherAlert CreateWeatherAlert(this GoogleWeatherProvider _, PublicAlerts alert)
        {
            return new WeatherAlert
            {
                Title = alert.alertTitle?.text,
                Message = alert.description,
                Attribution = alert.dataSource?.name,
                Date = DateTimeOffset.Parse(alert.startTime, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                ExpiresDate = DateTimeOffset.Parse(alert.expirationTime, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                Severity = alert.severity switch
                {
                    "EXTREME" => WeatherAlertSeverity.Extreme,
                    "SEVERE" => WeatherAlertSeverity.Severe,
                    "MODERATE" => WeatherAlertSeverity.Moderate,
                    "MINOR" => WeatherAlertSeverity.Minor,
                    _ => WeatherAlertSeverity.Unknown
                },
                Type = alert.eventType switch
                {
                    "ACID_RAIN" or "AVALANCHE" or "DROUGHT" or
                    "HUMIDITY" or "LANDSLIDE" or "MONSOON" or "OUTFLOW" or "RADIATION" or "RAIN_EVENT" => WeatherAlertType.SpecialWeatherAlert,

                    "BLIZZARD" or "BLOWING_SNOW" or "COLD" or "FREEZING" or "FREEZING_AIR_TEMPERATURE" or
                    "FREEZING_DRIZZLE" or "FREEZING_RAIN_EVENT" or "FROST" or "GLAZE" or "ICE_STORM" or
                    "LAKE_EFFECT_SNOW" or "SNOWSQUALL" or "SNOW_EVENT" or "WIND_CHILL" or "WINTER_STORM" => WeatherAlertType.WinterWeather,

                    "BUSHFIRE" or "FIRE" or "FIRE_WEATHER" or "INDUSTRIAL_FIRE" or "WILDFIRE" => WeatherAlertType.Fire,
                    "COASTAL_FLOOD" or "COASTAL_HAZARD" or "FLASH_FLOOD" or "FLOOD" or "MUDDY_FLOOD" or "RIVER_FLOODING" => WeatherAlertType.FloodWarning,
                    "DUST_STORM" => WeatherAlertType.DustAdvisory,
                    "AFTERSHOCK" or "EARTHQUAKE" => WeatherAlertType.EarthquakeWarning,
                    "FOG" => WeatherAlertType.DenseFog,
                    "GALE" => WeatherAlertType.GaleWarning,
                    "HEAT" => WeatherAlertType.Heat,
                    "HURRICANE" or "CYCLONE" or "EXTRATROPICAL_CYCLONE" or "TROPICAL_CYCLONE" or
                    "TROPICAL_CYCLONE_WARNINGS_AND_WATCHES" or "TROPICAL_DISTURBANCE" or "TROPICAL_STORM" or "TYPHOON" => WeatherAlertType.HurricaneWindWarning,

                    "SEVERE_THUNDERSTORM_WARNING" or "THUNDER" or "THUNDERSTORM" or "HAIL" => WeatherAlertType.SevereThunderstormWarning,
                    "STORM" or "STORM_SURGE" => WeatherAlertType.StormWarning,
                    "TORNADO" or "TORNADO_WARNING" => WeatherAlertType.TornadoWarning,
                    "TSUNAMI" => WeatherAlertType.TsunamiWarning,
                    "VOLCANIC_ASH" or "VOLCANIC_ERUPTION" => WeatherAlertType.Volcano,
                    "WIND" => WeatherAlertType.HighWind,
                    "HAZARDOUS_SEAS" or "WIND_WAVE" => WeatherAlertType.SmallCraft,
                    _ => WeatherAlertType.SpecialWeatherAlert
                }
            };
        }
    }
}
#nullable restore