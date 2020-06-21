using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Globalization;
using System.Text;
using Windows.System.UserProfile;

namespace SimpleWeather.Controls
{
    public class ForecastItemViewModel : BaseForecastItemViewModel
    {
        public string LoTemp { get; set; }

        public string ConditionLong { get; set; }
        public bool ShowExtraDetail { get; set; }

        public ForecastItemViewModel(Forecast forecast, params TextForecast[] txtForecasts)
            : base()
        {
            if (forecast is null)
            {
                throw new ArgumentNullException(nameof(forecast));
            }

            var userlang = GlobalizationPreferences.Languages[0];
            var culture = new CultureInfo(userlang);

            WeatherIcon = forecast.icon;
            Date = forecast.date.ToString("ddd dd", culture);
            ShortDate = forecast.date.ToString("ddd", culture);
            Condition = forecast.condition;
            try
            {
                if (forecast.high_f != null && forecast.high_c != null)
                {
                    HiTemp = (Settings.IsFahrenheit ?
                        Math.Round(double.Parse(forecast.high_f)).ToString() : Math.Round(double.Parse(forecast.high_c)).ToString()) + "º";
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
            try
            {
                if (forecast.low_f != null && forecast.low_c != null)
                {
                    LoTemp = (Settings.IsFahrenheit ?
                        Math.Round(double.Parse(forecast.low_f)).ToString() : Math.Round(double.Parse(forecast.low_c)).ToString()) + "º";
                }
                else
                {
                    LoTemp = "--";
                }
            }
            catch (FormatException ex)
            {
                LoTemp = "--";
                Logger.WriteLine(LoggerLevel.Error, "Invalid number format", ex);
            }

            // Extras
            if (forecast.extras != null)
            {
                if (forecast.extras.feelslike_f != 0 && (forecast.extras.feelslike_f != forecast.extras.feelslike_c))
                {
                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.FeelsLike,
                           Settings.IsFahrenheit ?
                                Math.Round(forecast.extras.feelslike_f) + "º" :
                                Math.Round(forecast.extras.feelslike_c) + "º"));
                }

                string Chance = PoP = forecast.extras.pop + "%";
                string Qpf_Rain = Settings.IsFahrenheit ?
                    forecast.extras.qpf_rain_in.ToString("0.00", culture) + " in" : forecast.extras.qpf_rain_mm.ToString(culture) + " mm";
                string Qpf_Snow = Settings.IsFahrenheit ?
                    forecast.extras.qpf_snow_in.ToString("0.00", culture) + " in" : forecast.extras.qpf_snow_cm.ToString(culture) + " cm";

                if (WeatherAPI.OpenWeatherMap.Equals(Settings.API) || WeatherAPI.MetNo.Equals(Settings.API))
                {
                    if (forecast.extras.qpf_rain_in >= 0)
                        DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.PoPRain, Qpf_Rain));
                    if (forecast.extras.qpf_snow_in >= 0)
                        DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.PoPSnow, Qpf_Snow));
                    if (!String.IsNullOrWhiteSpace(forecast.extras.pop))
                        DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.PoPCloudiness, Chance));
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(forecast.extras.pop))
                        DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.PoPChance, Chance));
                    if (forecast.extras.qpf_rain_in >= 0)
                        DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.PoPRain, Qpf_Rain));
                    if (forecast.extras.qpf_snow_in >= 0)
                        DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.PoPSnow, Qpf_Snow));
                }

                if (!String.IsNullOrWhiteSpace(forecast.extras.humidity))
                {
                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.Humidity,
                        forecast.extras.humidity.EndsWith("%", StringComparison.Ordinal) ?
                                forecast.extras.humidity : forecast.extras.humidity + "%"));
                }

                if (!String.IsNullOrWhiteSpace(forecast.extras.dewpoint_f) && (forecast.extras.dewpoint_f != forecast.extras.dewpoint_c))
                {
                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.Dewpoint,
                           Settings.IsFahrenheit ?
                                Math.Round(float.Parse(forecast.extras.dewpoint_f)) + "º" :
                                Math.Round(float.Parse(forecast.extras.dewpoint_c)) + "º"));
                }

                if (forecast.extras.uv_index >= 0)
                {
                    UV uv = new UV(forecast.extras.uv_index);

                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.UV,
                           string.Format("{0}, {1}", uv.index, uv.desc)));
                }

                if (!String.IsNullOrWhiteSpace(forecast.extras.pressure_in))
                {
                    var pressureVal = Settings.IsFahrenheit ? forecast.extras.pressure_in : forecast.extras.pressure_mb;
                    var pressureUnit = Settings.IsFahrenheit ? "in" : "mb";

                    if (float.TryParse(pressureVal, NumberStyles.Float, CultureInfo.InvariantCulture, out float pressure))
                        DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.Pressure,
                            string.Format("{0} {1}", pressure.ToString(culture), pressureUnit)));
                }

                if (forecast.extras.wind_mph >= 0)
                {
                    WindSpeed = Settings.IsFahrenheit ?
                                Math.Round(forecast.extras.wind_mph) + " mph" :
                                Math.Round(forecast.extras.wind_kph) + " kph";
                    WindDirection = forecast.extras.wind_degrees;
                    WindDir = WeatherUtils.GetWindDirection(forecast.extras.wind_degrees);

                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.WindSpeed,
                           String.Format(CultureInfo.InvariantCulture, "{0}, {1}", WindSpeed, WindDir), WindDirection));
                }

                if (!String.IsNullOrWhiteSpace(forecast.extras.visibility_mi))
                {
                    var visibilityVal = Settings.IsFahrenheit ? forecast.extras.visibility_mi : forecast.extras.visibility_km;
                    var visibilityUnit = Settings.IsFahrenheit ? "mi" : "km";

                    if (float.TryParse(visibilityVal, NumberStyles.Float, CultureInfo.InvariantCulture, out float visibility))
                        DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.Visibility,
                               string.Format("{0} {1}", visibility.ToString(culture), visibilityUnit)));
                }
            }

            if (txtForecasts?.Length > 0)
            {
                try
                {
                    bool dayAndNt = txtForecasts.Length == 2;
                    StringBuilder sb = new StringBuilder();

                    TextForecast fctDay = txtForecasts[0];
                    sb.Append(Settings.IsFahrenheit ? fctDay.fcttext : fctDay.fcttext_metric);

                    if (dayAndNt)
                    {
                        sb.Append(Environment.NewLine).Append(Environment.NewLine);

                        TextForecast fctNt = txtForecasts[1];
                        sb.Append(Settings.IsFahrenheit ? fctNt.fcttext : fctNt.fcttext_metric);
                    }

                    ConditionLong = sb.ToString();
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(LoggerLevel.Debug, ex, "error!");
                }
            }

            ShowExtraDetail = !(String.IsNullOrWhiteSpace(WindSpeed) || String.IsNullOrWhiteSpace(PoP.Replace("%", "")));
        }
    }
}