using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Globalization;

namespace SimpleWeather.Controls
{
    public class HourlyForecastItemViewModel : BaseForecastItemViewModel
    {
        internal HourlyForecast Forecast { get; private set; }

        public HourlyForecastItemViewModel(HourlyForecast hrForecast)
            : base()
        {
            if (hrForecast is null)
            {
                throw new ArgumentNullException(nameof(hrForecast));
            }
            this.Forecast = hrForecast;

            var culture = CultureUtils.UserCulture;

            WeatherIcon = hrForecast.icon;

            if (culture.DateTimeFormat.ShortTimePattern.Contains("H"))
            {
                Date = hrForecast.date.ToString("ddd HH:00", culture);
                ShortDate = hrForecast.date.ToString("HH:00", culture);
            }
            else
            {
                Date = hrForecast.date.ToString("ddd h tt", culture);
                ShortDate = hrForecast.date.ToString("ht", culture);
            }

            Condition = wm.SupportsWeatherLocale ? hrForecast.condition : wm.GetWeatherCondition(hrForecast.icon);
            try
            {
                if (hrForecast.high_f.HasValue && hrForecast.high_c.HasValue)
                {
                    var value = Settings.IsFahrenheit ? Math.Round(hrForecast.high_f.Value) : Math.Round(hrForecast.high_c.Value);
                    HiTemp = String.Format(culture, "{0}°", value);
                }
                else
                {
                    HiTemp = "--";
                }
            }
            catch (FormatException ex)
            {
                HiTemp = "--";
                Logger.WriteLine(LoggerLevel.Error, "Invalid number format", ex);
            }

            if (hrForecast.wind_mph.HasValue && hrForecast.wind_mph >= 0 &&
                hrForecast.wind_degrees.HasValue && hrForecast.wind_degrees >= 0)
            {
                WindDirection = hrForecast.wind_degrees.GetValueOrDefault(0);

                WindDir = WeatherUtils.GetWindDirection(WindDirection);

                var speedVal = Settings.IsFahrenheit ? Math.Round(hrForecast.wind_mph.Value) : Math.Round(hrForecast.wind_kph.Value);
                var speedUnit = WeatherUtils.SpeedUnit;
                WindSpeed = String.Format(culture, "{0} {1}", speedVal, speedUnit);
            }

            // Extras
            if (hrForecast.extras != null)
            {
                if (hrForecast.extras.feelslike_f.HasValue && (hrForecast.extras.feelslike_f != hrForecast.extras.feelslike_c))
                {
                    var value = Settings.IsFahrenheit ? Math.Round(hrForecast.extras.feelslike_f.Value) : Math.Round(hrForecast.extras.feelslike_c.Value);

                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.FeelsLike,
                           String.Format(culture, "{0}°", value)));
                }

                if (hrForecast.extras.pop.HasValue && hrForecast.extras.pop >= 0)
                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.PoPChance, hrForecast.extras.pop.Value + "%"));
                if (hrForecast.extras.qpf_rain_in.HasValue && hrForecast.extras.qpf_rain_in >= 0)
                {
                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.PoPRain,
                        String.Format(culture, "{0:0.00} {1}",
                            Settings.IsFahrenheit ? hrForecast.extras.qpf_rain_in.Value : hrForecast.extras.qpf_rain_mm.Value,
                            WeatherUtils.GetPrecipitationUnit(false))));
                }
                if (hrForecast.extras.qpf_snow_in.HasValue && hrForecast.extras.qpf_snow_in >= 0)
                {
                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.PoPSnow,
                        String.Format(culture, "{0:0.00} {1}",
                            Settings.IsFahrenheit ? hrForecast.extras.qpf_snow_in.Value : hrForecast.extras.qpf_snow_cm.Value,
                            WeatherUtils.GetPrecipitationUnit(true))));
                }
                if (hrForecast.extras.cloudiness.HasValue && hrForecast.extras.cloudiness >= 0)
                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.PoPCloudiness, hrForecast.extras.cloudiness.Value + "%"));

                if (hrForecast.extras.humidity.HasValue)
                {
                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.Humidity,
                        String.Format(culture, "{0}%", hrForecast.extras.humidity)));
                }

                if (hrForecast.extras.dewpoint_f.HasValue && (hrForecast.extras.dewpoint_f != hrForecast.extras.dewpoint_c))
                {
                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.Dewpoint,
                        String.Format(culture, "{0}°",
                        Settings.IsFahrenheit ?
                                Math.Round(hrForecast.extras.dewpoint_f.Value) :
                                Math.Round(hrForecast.extras.dewpoint_c.Value))));
                }

                if (hrForecast.extras.uv_index.HasValue && hrForecast.extras.uv_index >= 0)
                {
                    UV uv = new UV(hrForecast.extras.uv_index.Value);

                    DetailExtras.Add(new DetailItemViewModel(uv));
                }

                if (hrForecast.extras.pressure_in.HasValue)
                {
                    var pressureVal = Settings.IsFahrenheit ? hrForecast.extras.pressure_in.Value : hrForecast.extras.pressure_mb.Value;
                    var pressureUnit = WeatherUtils.PressureUnit;

                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.Pressure,
                        String.Format(culture, "{0:0.00} {1}", pressureVal, pressureUnit)));
                }

                if (!String.IsNullOrWhiteSpace(WindSpeed))
                {
                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.WindSpeed,
                        String.Format(culture, "{0}, {1}", WindSpeed, WindDir), WindDirection));
                }

                if (hrForecast.extras.windgust_mph.HasValue && hrForecast.extras.windgust_kph.HasValue && hrForecast.extras.windgust_mph >= 0)
                {
                    var speedVal = Settings.IsFahrenheit ? Math.Round(hrForecast.extras.windgust_mph.Value) : Math.Round(hrForecast.extras.windgust_kph.Value);
                    var speedUnit = WeatherUtils.SpeedUnit;

                    var windGustSpeed = String.Format(culture, "{0} {1}", speedVal, speedUnit);
                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.WindGust, windGustSpeed));
                }

                if (hrForecast.extras.visibility_mi.HasValue && hrForecast.extras.visibility_mi >= 0)
                {
                    var visibilityVal = Settings.IsFahrenheit ? hrForecast.extras.visibility_mi.Value : hrForecast.extras.visibility_km.Value;
                    var visibilityUnit = WeatherUtils.DistanceUnit;

                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.Visibility,
                           String.Format(culture, "{0:0.00} {1}", visibilityVal, visibilityUnit)));
                }
            }
        }
    }
}