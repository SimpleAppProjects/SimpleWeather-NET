using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Windows.System.UserProfile;
using Windows.UI.Xaml.Media;

namespace SimpleWeather.Controls
{
    public class ForecastItemViewModel : BaseForecastItemViewModel
    {
        public string LoTemp { get; set; }

        public string ConditionLong { get; set; }
        public bool ShowExtraDetail { get; set; }

        public ForecastItemViewModel()
        {
            wm = WeatherManager.GetInstance();
            DetailExtras = new List<DetailItemViewModel>();
        }

        public ForecastItemViewModel(Forecast forecast, params TextForecastItemViewModel[] txt_forecasts)
        {
            wm = WeatherManager.GetInstance();
            DetailExtras = new List<DetailItemViewModel>();

            var userlang = GlobalizationPreferences.Languages[0];
            var culture = new CultureInfo(userlang);

            WeatherIcon = forecast.icon;
            Date = forecast.date.ToString("ddd dd", culture);
            Condition = forecast.condition;
            try
            {
                HiTemp = (Settings.IsFahrenheit ?
                    Math.Round(double.Parse(forecast.high_f)).ToString() : Math.Round(double.Parse(forecast.high_c)).ToString()) + "º ";
            }
            catch (FormatException ex)
            {
                HiTemp = "--º ";
                Logger.WriteLine(LoggerLevel.Error, "Invalid number format", ex);
            }
            try
            {
                LoTemp = (Settings.IsFahrenheit ?
                    Math.Round(double.Parse(forecast.low_f)).ToString() : Math.Round(double.Parse(forecast.low_c)).ToString()) + "º ";
            }
            catch (FormatException ex)
            {
                LoTemp = "--º ";
                Logger.WriteLine(LoggerLevel.Error, "Invalid number format", ex);
            }

            // Extras
            if (forecast.extras != null)
            {
                DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.FeelsLike,
                       Settings.IsFahrenheit ?
                            Math.Round(forecast.extras.feelslike_f) + "º" :
                            Math.Round(forecast.extras.feelslike_c) + "º"));

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
                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.PoPCloudiness, Chance));
                }
                else
                {
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

                if (!String.IsNullOrWhiteSpace(forecast.extras.dewpoint_f))
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
                    WindDirection = new RotateTransform() { Angle = forecast.extras.wind_degrees };
                    WindDir = WeatherUtils.GetWindDirection(forecast.extras.wind_degrees);

                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.WindSpeed,
                           String.Format(CultureInfo.InvariantCulture, "{0}, {1}", WindSpeed, WindDir), (int)WindDirection.Angle));
                }

                if (!String.IsNullOrWhiteSpace(forecast.extras.visibility_mi))
                {
                    var visibilityVal = Settings.IsFahrenheit ? forecast.extras.visibility_mi : forecast.extras.visibility_km;
                    var visibilityUnit = Settings.IsFahrenheit ? "mi" : "km";

                    if (float.TryParse(visibilityVal, NumberStyles.Float, CultureInfo.InvariantCulture, out float visibility))
                        DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.Visibility,
                               string.Format("{0} {1}", visibility.ToString(culture), visibilityUnit)));
                }

                if (txt_forecasts?.Length > 0)
                {
                    try
                    {
                        bool dayAndNt = txt_forecasts.Length == 2;
                        StringBuilder sb = new StringBuilder();

                        TextForecastItemViewModel fctDay = txt_forecasts[0];
                        sb.AppendFormat(CultureInfo.InvariantCulture, "{0} - {1}", fctDay.Title, fctDay.FctText);

                        if (dayAndNt)
                        {
                            sb.Append(Environment.NewLine).Append(Environment.NewLine);

                            TextForecastItemViewModel fctNt = txt_forecasts[1];
                            sb.AppendFormat(CultureInfo.InvariantCulture, "{0} - {1}", fctNt.Title, fctNt.FctText);
                        }

                        ConditionLong = sb.ToString();
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine(LoggerLevel.Debug, ex, "error!");
                    }
                }
            }

            ShowExtraDetail = !(String.IsNullOrWhiteSpace(WindSpeed) || String.IsNullOrWhiteSpace(PoP.Replace("%", "")));
        }
    }
}
