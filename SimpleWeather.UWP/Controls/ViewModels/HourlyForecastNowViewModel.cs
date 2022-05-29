using SimpleWeather.Controls;
using SimpleWeather.Icons;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.UWP.Controls
{
    public class HourlyForecastNowViewModel
    {
        public string Date { get; set; }
        public string Icon { get; set; }
        public string Temperature { get; set; }
        public string Condition { get; set; }
        public string PoPChance { get; set; }
        public string WindSpeed { get; set; }
        public int WindDirection { get; set; }

        public HourlyForecastNowViewModel(HourlyForecast forecast)
        {
            var culture = CultureUtils.UserCulture;
            var isFahrenheit = Units.FAHRENHEIT.Equals(Settings.TemperatureUnit);
            var wm = WeatherManager.GetInstance();

            if (culture.DateTimeFormat.ShortTimePattern.Contains("H"))
            {
                Date = forecast.date.ToString("ddd HH:00", culture);
            }
            else
            {
                Date = forecast.date.ToString("ddd h tt", culture);
            }

            Icon = forecast.icon;

            try
            {
                if (forecast.high_f.HasValue && forecast.high_c.HasValue)
                {
                    var value = isFahrenheit ? Math.Round(forecast.high_f.Value) : Math.Round(forecast.high_c.Value);
                    Temperature = String.Format(culture, "{0}°", value);
                }
                else
                {
                    Temperature = WeatherIcons.PLACEHOLDER;
                }
            }
            catch (FormatException ex)
            {
                Temperature = WeatherIcons.PLACEHOLDER;
                Logger.WriteLine(LoggerLevel.Error, "Invalid number format", ex);
            }

            Condition = wm.SupportsWeatherLocale ? forecast.condition : wm.GetWeatherCondition(forecast.icon);

            if (forecast.wind_mph.HasValue && forecast.wind_mph >= 0 &&
                forecast.wind_degrees.HasValue && forecast.wind_degrees >= 0)
            {
                string unit = Settings.SpeedUnit;
                int speedVal;
                string speedUnit;

                switch (unit)
                {
                    case Units.MILES_PER_HOUR:
                    default:
                        speedVal = (int)Math.Round(forecast.wind_mph.Value);
                        speedUnit = SharedModule.Instance.ResLoader.GetString("/Units/unit_mph");
                        break;
                    case Units.KILOMETERS_PER_HOUR:
                        speedVal = (int)Math.Round(forecast.wind_kph.Value);
                        speedUnit = SharedModule.Instance.ResLoader.GetString("/Units/unit_kph");
                        break;
                    case Units.METERS_PER_SECOND:
                        speedVal = (int)Math.Round(ConversionMethods.KphToMSec(forecast.wind_kph.Value));
                        speedUnit = SharedModule.Instance.ResLoader.GetString("/Units/unit_msec");
                        break;
                }

                WindDirection = forecast.wind_degrees.GetValueOrDefault(0) + 180;

                WindSpeed = String.Format(culture, "{0} {1}", speedVal, speedUnit);
            }

            if (forecast.extras?.pop.HasValue == true && forecast.extras?.pop >= 0)
            {
                PoPChance = forecast.extras.pop.Value + "%";
            }                
        }
    }
}
