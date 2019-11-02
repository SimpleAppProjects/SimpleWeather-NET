using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Globalization;
using System.ComponentModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Collections.Generic;

namespace SimpleWeather.Controls
{
    public class HourlyForecastItemViewModel : BaseForecastItemViewModel
    {
        public HourlyForecastItemViewModel()
        {
            wm = WeatherManager.GetInstance();
            DetailExtras = new List<DetailItemViewModel>();
        }

        public HourlyForecastItemViewModel(HourlyForecast hr_forecast)
        {
            wm = WeatherManager.GetInstance();
            DetailExtras = new List<DetailItemViewModel>();

            var userlang = Windows.System.UserProfile.GlobalizationPreferences.Languages[0];
            var culture = new CultureInfo(userlang);

            WeatherIcon = hr_forecast.icon;

            if (culture.DateTimeFormat.ShortTimePattern.Contains("H"))
            {
                Date = hr_forecast.date.ToString("ddd HH:00", culture);
                ShortDate = hr_forecast.date.ToString("HH", culture);
            }
            else
            {
                Date = hr_forecast.date.ToString("ddd h tt", culture);
                ShortDate = hr_forecast.date.ToString("ht", culture);
            }

            Condition = hr_forecast.condition;
            try
            {
                HiTemp = (Settings.IsFahrenheit ?
                    Math.Round(double.Parse(hr_forecast.high_f)).ToString() : Math.Round(double.Parse(hr_forecast.high_c)).ToString()) + "º ";
            }
            catch (FormatException ex)
            {
                HiTemp = "--º ";
                Logger.WriteLine(LoggerLevel.Error, "Invalid number format", ex);
            }
            PoP = hr_forecast.pop + "%";
            WindDirection = hr_forecast.wind_degrees;
            WindDir = WeatherUtils.GetWindDirection(hr_forecast.wind_degrees);
            WindSpeed = (Settings.IsFahrenheit ?
                   Math.Round(hr_forecast.wind_mph) + " mph" :
                   Math.Round(hr_forecast.wind_kph) + " kph");

            // Extras
            if (hr_forecast.extras != null)
            {
                DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.FeelsLike,
                       Settings.IsFahrenheit ?
                            Math.Round(hr_forecast.extras.feelslike_f) + "º" :
                            Math.Round(hr_forecast.extras.feelslike_c) + "º"));

                string Qpf_Rain = Settings.IsFahrenheit ?
                    hr_forecast.extras.qpf_rain_in.ToString("0.00", culture) + " in" : hr_forecast.extras.qpf_rain_mm.ToString(culture) + " mm";
                string Qpf_Snow = Settings.IsFahrenheit ?
                    hr_forecast.extras.qpf_snow_in.ToString("0.00", culture) + " in" : hr_forecast.extras.qpf_snow_cm.ToString(culture) + " cm";

                if (WeatherAPI.OpenWeatherMap.Equals(Settings.API) || WeatherAPI.MetNo.Equals(Settings.API))
                {
                    if (hr_forecast.extras.qpf_rain_in >= 0)
                        DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.PoPRain, Qpf_Rain));
                    if (hr_forecast.extras.qpf_snow_in >= 0)
                        DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.PoPSnow, Qpf_Snow));
                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.PoPCloudiness, PoP));
                }
                else
                {
                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.PoPChance, PoP));
                    if (hr_forecast.extras.qpf_rain_in >= 0)
                        DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.PoPRain, Qpf_Rain));
                    if (hr_forecast.extras.qpf_snow_in >= 0)
                        DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.PoPSnow, Qpf_Snow));
                }

                if (!String.IsNullOrWhiteSpace(hr_forecast.extras.humidity))
                {
                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.Humidity,
                        hr_forecast.extras.humidity.EndsWith("%", StringComparison.Ordinal) ?
                                hr_forecast.extras.humidity : hr_forecast.extras.humidity + "%"));
                }

                if (!String.IsNullOrWhiteSpace(hr_forecast.extras.dewpoint_f))
                {
                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.Dewpoint,
                           Settings.IsFahrenheit ?
                                Math.Round(float.Parse(hr_forecast.extras.dewpoint_f)) + "º" :
                                Math.Round(float.Parse(hr_forecast.extras.dewpoint_c)) + "º"));
                }

                if (hr_forecast.extras.uv_index >= 0)
                {
                    UV uv = new UV(hr_forecast.extras.uv_index);

                    DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.UV,
                           string.Format("{0}, {1}", uv.index, uv.desc)));
                }

                if (!String.IsNullOrWhiteSpace(hr_forecast.extras.pressure_in))
                {
                    var pressureVal = Settings.IsFahrenheit ? hr_forecast.extras.pressure_in : hr_forecast.extras.pressure_mb;
                    var pressureUnit = Settings.IsFahrenheit ? "in" : "mb";

                    if (float.TryParse(pressureVal, NumberStyles.Float, CultureInfo.InvariantCulture, out float pressure))
                        DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.Pressure,
                            string.Format("{0} {1}", pressure.ToString(culture), pressureUnit)));
                }

                DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.WindSpeed,
                    String.Format(CultureInfo.InvariantCulture, "{0}, {1}", WindSpeed, WindDir), WindDirection));

                if (!String.IsNullOrWhiteSpace(hr_forecast.extras.visibility_mi))
                {
                    var visibilityVal = Settings.IsFahrenheit ? hr_forecast.extras.visibility_mi : hr_forecast.extras.visibility_km;
                    var visibilityUnit = Settings.IsFahrenheit ? "mi" : "km";

                    if (float.TryParse(visibilityVal, NumberStyles.Float, CultureInfo.InvariantCulture, out float visibility))
                        DetailExtras.Add(new DetailItemViewModel(WeatherDetailsType.Visibility,
                               string.Format("{0} {1}", visibility.ToString(culture), visibilityUnit)));
                }
            }
        }
    }
}
