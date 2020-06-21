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

            var userlang = Windows.System.UserProfile.GlobalizationPreferences.Languages[0];
            var culture = new CultureInfo(userlang);

            WeatherIcon = hrForecast.icon;

            if (culture.DateTimeFormat.ShortTimePattern.Contains("H"))
            {
                Date = hrForecast.date.ToString("ddd HH:00", culture);
                ShortDate = hrForecast.date.ToString("HH", culture);
            }
            else
            {
                Date = hrForecast.date.ToString("ddd h tt", culture);
                ShortDate = hrForecast.date.ToString("ht", culture);
            }

            Condition = hrForecast.condition;
            try
            {
                HiTemp = (Settings.IsFahrenheit ?
                    Math.Round(double.Parse(hrForecast.high_f)).ToString() : Math.Round(double.Parse(hrForecast.high_c)).ToString()) + "º";
            }
            catch (FormatException ex)
            {
                HiTemp = "--";
                Logger.WriteLine(LoggerLevel.Error, "Invalid number format", ex);
            }
            PoP = hrForecast.pop + "%";
            WindDirection = hrForecast.wind_degrees;
            WindDir = WeatherUtils.GetWindDirection(hrForecast.wind_degrees);
            WindSpeed = (Settings.IsFahrenheit ?
                   Math.Round(hrForecast.wind_mph) + " mph" :
                   Math.Round(hrForecast.wind_kph) + " kph");

            // Extras
            if (hrForecast.extras != null)
            {
                if (hrForecast.extras.feelslike_f != 0 && (hrForecast.extras.feelslike_f != hrForecast.extras.feelslike_c))
                {
                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.FeelsLike,
                           Settings.IsFahrenheit ?
                                Math.Round(hrForecast.extras.feelslike_f) + "º" :
                                Math.Round(hrForecast.extras.feelslike_c) + "º"));
                }

                string Qpf_Rain = Settings.IsFahrenheit ?
                    hrForecast.extras.qpf_rain_in.ToString("0.00", culture) + " in" : hrForecast.extras.qpf_rain_mm.ToString(culture) + " mm";
                string Qpf_Snow = Settings.IsFahrenheit ?
                    hrForecast.extras.qpf_snow_in.ToString("0.00", culture) + " in" : hrForecast.extras.qpf_snow_cm.ToString(culture) + " cm";

                if (WeatherAPI.OpenWeatherMap.Equals(Settings.API) || WeatherAPI.MetNo.Equals(Settings.API))
                {
                    if (hrForecast.extras.qpf_rain_in >= 0)
                        DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.PoPRain, Qpf_Rain));
                    if (hrForecast.extras.qpf_snow_in >= 0)
                        DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.PoPSnow, Qpf_Snow));
                    if (!String.IsNullOrWhiteSpace(hrForecast.pop))
                        DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.PoPCloudiness, PoP));
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(hrForecast.pop))
                        DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.PoPChance, PoP));
                    if (hrForecast.extras.qpf_rain_in >= 0)
                        DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.PoPRain, Qpf_Rain));
                    if (hrForecast.extras.qpf_snow_in >= 0)
                        DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.PoPSnow, Qpf_Snow));
                }

                if (!String.IsNullOrWhiteSpace(hrForecast.extras.humidity))
                {
                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.Humidity,
                        hrForecast.extras.humidity.EndsWith("%", StringComparison.Ordinal) ?
                                hrForecast.extras.humidity : hrForecast.extras.humidity + "%"));
                }

                if (!String.IsNullOrWhiteSpace(hrForecast.extras.dewpoint_f))
                {
                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.Dewpoint,
                           Settings.IsFahrenheit ?
                                Math.Round(float.Parse(hrForecast.extras.dewpoint_f)) + "º" :
                                Math.Round(float.Parse(hrForecast.extras.dewpoint_c)) + "º"));
                }

                if (hrForecast.extras.uv_index >= 0)
                {
                    UV uv = new UV(hrForecast.extras.uv_index);

                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.UV,
                           string.Format("{0}, {1}", uv.index, uv.desc)));
                }

                if (!String.IsNullOrWhiteSpace(hrForecast.extras.pressure_in))
                {
                    var pressureVal = Settings.IsFahrenheit ? hrForecast.extras.pressure_in : hrForecast.extras.pressure_mb;
                    var pressureUnit = Settings.IsFahrenheit ? "in" : "mb";

                    if (float.TryParse(pressureVal, NumberStyles.Float, CultureInfo.InvariantCulture, out float pressure))
                        DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.Pressure,
                            string.Format("{0} {1}", pressure.ToString(culture), pressureUnit)));
                }

                if (!String.IsNullOrWhiteSpace(WindSpeed))
                {
                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.WindSpeed,
                        String.Format(CultureInfo.InvariantCulture, "{0}, {1}", WindSpeed, WindDir), WindDirection));
                }

                if (!String.IsNullOrWhiteSpace(hrForecast.extras.visibility_mi))
                {
                    var visibilityVal = Settings.IsFahrenheit ? hrForecast.extras.visibility_mi : hrForecast.extras.visibility_km;
                    var visibilityUnit = Settings.IsFahrenheit ? "mi" : "km";

                    if (float.TryParse(visibilityVal, NumberStyles.Float, CultureInfo.InvariantCulture, out float visibility))
                        DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.Visibility,
                               string.Format("{0} {1}", visibility.ToString(culture), visibilityUnit)));
                }
            }
        }
    }
}