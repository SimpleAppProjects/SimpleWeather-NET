using SimpleWeather.Icons;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Globalization;

namespace SimpleWeather.Controls
{
    public class HourlyForecastItemViewModel : BaseForecastItemViewModel
    {
        internal HourlyForecast forecast { get; private set; }

        public HourlyForecastItemViewModel(HourlyForecast hrForecast)
            : base()
        {
            if (hrForecast is null)
            {
                throw new ArgumentNullException(nameof(hrForecast));
            }
            this.forecast = hrForecast;

            var culture = CultureUtils.UserCulture;
            var isFahrenheit = Units.FAHRENHEIT.Equals(Settings.TemperatureUnit);

            WeatherIcon = forecast.icon;

            if (culture.DateTimeFormat.ShortTimePattern.Contains("H"))
            {
                Date = forecast.date.ToString("ddd HH:00", culture);
                ShortDate = forecast.date.ToString("HH:00", culture);
            }
            else
            {
                Date = forecast.date.ToString("ddd h tt", culture);
                ShortDate = forecast.date.ToString("ht", culture);
            }

            Condition = wm.SupportsWeatherLocale ? forecast.condition : wm.GetWeatherCondition(forecast.icon);
            try
            {
                if (forecast.high_f.HasValue && forecast.high_c.HasValue)
                {
                    var value = isFahrenheit ? Math.Round(forecast.high_f.Value) : Math.Round(forecast.high_c.Value);
                    HiTemp = String.Format(culture, "{0}°", value);
                }
                else
                {
                    HiTemp = WeatherIcons.PLACEHOLDER;
                }
            }
            catch (FormatException ex)
            {
                HiTemp = WeatherIcons.PLACEHOLDER;
                Logger.WriteLine(LoggerLevel.Error, "Invalid number format", ex);
            }

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
                        speedUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_mph");
                        break;
                    case Units.KILOMETERS_PER_HOUR:
                        speedVal = (int)Math.Round(forecast.wind_kph.Value);
                        speedUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_kph");
                        break;
                    case Units.METERS_PER_SECOND:
                        speedVal = (int)Math.Round(ConversionMethods.KphToMSec(forecast.wind_kph.Value));
                        speedUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_msec");
                        break;
                }

                WindDirection = forecast.wind_degrees.GetValueOrDefault(0);
                WindDir = WeatherUtils.GetWindDirection(WindDirection);

                WindSpeed = String.Format(culture, "{0} {1}", speedVal, speedUnit);
            }

            // Extras
            if (forecast.extras != null)
            {
                if (forecast.extras.feelslike_f.HasValue && (forecast.extras.feelslike_f != forecast.extras.feelslike_c))
                {
                    var value = isFahrenheit ? Math.Round(forecast.extras.feelslike_f.Value) : Math.Round(forecast.extras.feelslike_c.Value);

                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.FeelsLike,
                           String.Format(culture, "{0}°", value)));
                }

                if (forecast.extras.pop.HasValue && forecast.extras.pop >= 0)
                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.PoPChance, forecast.extras.pop.Value + "%"));
                if (forecast.extras.qpf_rain_in.HasValue && forecast.extras.qpf_rain_in >= 0)
                {
                    string unit = Settings.PrecipitationUnit;
                    float precipValue;
                    string precipUnit;

                    switch (unit)
                    {
                        case Units.INCHES:
                        default:
                            precipValue = forecast.extras.qpf_rain_in.Value;
                            precipUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_in");
                            break;
                        case Units.MILLIMETERS:
                            precipValue = forecast.extras.qpf_rain_mm.Value;
                            precipUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_mm");
                            break;
                    }

                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.PoPRain,
                        String.Format(culture, "{0:0.##} {1}", precipValue, precipUnit)));
                }
                if (forecast.extras.qpf_snow_in.HasValue && forecast.extras.qpf_snow_in >= 0)
                {
                    string unit = Settings.PrecipitationUnit;
                    float precipValue;
                    string precipUnit;

                    switch (unit)
                    {
                        case Units.INCHES:
                        default:
                            precipValue = forecast.extras.qpf_snow_in.Value;
                            precipUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_in");
                            break;
                        case Units.MILLIMETERS:
                            precipValue = forecast.extras.qpf_snow_cm.Value * 10;
                            precipUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_mm");
                            break;
                    }

                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.PoPSnow,
                        String.Format(culture, "{0:0.##} {1}", precipValue, precipUnit)));
                }
                if (forecast.extras.cloudiness.HasValue && forecast.extras.cloudiness >= 0)
                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.PoPCloudiness, forecast.extras.cloudiness.Value + "%"));

                if (forecast.extras.humidity.HasValue)
                {
                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.Humidity,
                        String.Format(culture, "{0}%", forecast.extras.humidity)));
                }

                if (forecast.extras.dewpoint_f.HasValue && (forecast.extras.dewpoint_f != forecast.extras.dewpoint_c))
                {
                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.Dewpoint,
                        String.Format(culture, "{0}°",
                        isFahrenheit ?
                                Math.Round(forecast.extras.dewpoint_f.Value) :
                                Math.Round(forecast.extras.dewpoint_c.Value))));
                }

                if (forecast.extras.uv_index.HasValue && forecast.extras.uv_index >= 0)
                {
                    UV uv = new UV(forecast.extras.uv_index.Value);

                    DetailExtras.Add(new DetailItemViewModel(uv));
                }

                if (forecast.extras.pressure_in.HasValue)
                {
                    string unit = Settings.PressureUnit;
                    float pressureVal;
                    string pressureUnit;

                    switch (unit)
                    {
                        case Units.INHG:
                        default:
                            pressureVal = forecast.extras.pressure_in.Value;
                            pressureUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_inHg");
                            break;
                        case Units.MILLIBAR:
                            pressureVal = forecast.extras.pressure_mb.Value;
                            pressureUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_mBar");
                            break;
                    }

                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.Pressure,
                        String.Format(culture, "{0:0.##} {1}", pressureVal, pressureUnit)));
                }

                if (!String.IsNullOrWhiteSpace(WindSpeed))
                {
                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.WindSpeed,
                        String.Format(culture, "{0}, {1}", WindSpeed, WindDir), WindDirection));
                }

                if (forecast.extras.windgust_mph.HasValue && forecast.extras.windgust_kph.HasValue && forecast.extras.windgust_mph >= 0)
                {
                    string unit = Settings.SpeedUnit;
                    int speedVal;
                    string speedUnit;

                    switch (unit)
                    {
                        case Units.MILES_PER_HOUR:
                        default:
                            speedVal = (int)Math.Round(forecast.extras.windgust_mph.Value);
                            speedUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_mph");
                            break;
                        case Units.KILOMETERS_PER_HOUR:
                            speedVal = (int)Math.Round(forecast.extras.windgust_kph.Value);
                            speedUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_kph");
                            break;
                        case Units.METERS_PER_SECOND:
                            speedVal = (int)Math.Round(ConversionMethods.KphToMSec(forecast.extras.windgust_kph.Value));
                            speedUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_msec");
                            break;
                    }

                    var windGustSpeed = String.Format(culture, "{0} {1}", speedVal, speedUnit);
                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.WindGust, windGustSpeed));
                }

                if (forecast.extras.visibility_mi.HasValue && forecast.extras.visibility_mi >= 0)
                {
                    string unit = Settings.DistanceUnit;
                    int visibilityVal;
                    string visibilityUnit;

                    switch (unit)
                    {
                        case Units.MILES:
                        default:
                            visibilityVal = (int)Math.Round(forecast.extras.visibility_mi.Value);
                            visibilityUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_miles");
                            break;
                        case Units.KILOMETERS:
                            visibilityVal = (int)Math.Round(forecast.extras.visibility_km.Value);
                            visibilityUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_kilometers");
                            break;
                    }

                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.Visibility,
                           String.Format(culture, "{0:0.##} {1}", visibilityVal, visibilityUnit)));
                }
            }
        }
    }
}