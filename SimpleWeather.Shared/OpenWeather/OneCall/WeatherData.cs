using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SimpleWeather.WeatherData
{
    public partial class Weather
    {
#if false
        public Weather(OpenWeather.Rootobject root)
        {
            location = new Location(root);
            update_time = DateTimeOffset.FromUnixTimeSeconds(root.current.dt);

            forecast = new List<Forecast>(root.daily.Length);
            txt_forecast = new List<TextForecast>(root.daily.Length);
            foreach (OpenWeather.Daily daily in root.daily)
            {
                forecast.Add(new Forecast(daily));
                txt_forecast.Add(new TextForecast(daily));
            }
            hr_forecast = new List<HourlyForecast>(root.hourly.Length);
            foreach (OpenWeather.Hourly hourly in root.hourly)
            {
                hr_forecast.Add(new HourlyForecast(hourly));
            }
            condition = new Condition(root.current);
            atmosphere = new Atmosphere(root.current);
            astronomy = new Astronomy(root.current);
            precipitation = new Precipitation(root.current);
            ttl = 180;

            query = string.Format(CultureInfo.InvariantCulture, "lat={0:0.####}&lon={1:0.####}", location.latitude, location.longitude);

            if ((!condition.high_f.HasValue || !condition.low_f.HasValue) && forecast.Count > 0)
            {
                condition.high_f = forecast[0].high_f.Value;
                condition.high_c = forecast[0].high_c.Value;
                condition.low_f = forecast[0].low_f.Value;
                condition.low_c = forecast[0].low_c.Value;
            }

            source = WeatherAPI.OpenWeatherMap;
        }
#endif
    }

    public partial class Location
    {
        /* OpenWeather OneCall */
#if false
        public Location(OpenWeather.Rootobject root)
        {
            // Use location name from location provider
            name = null;
            latitude = root.lat;
            longitude = root.lon;
            tz_long = root.timezone;
        }
#endif
    }

    public partial class Forecast
    {
        /* OpenWeather OneCall */
#if false
        public Forecast(OpenWeather.Daily forecast)
        {
            date = DateTimeOffset.FromUnixTimeSeconds(forecast.dt).DateTime;
            high_f = ConversionMethods.KtoF(forecast.temp.max);
            high_c = ConversionMethods.KtoC(forecast.temp.max);
            low_f = ConversionMethods.KtoF(forecast.temp.min);
            low_c = ConversionMethods.KtoC(forecast.temp.min);
            condition = forecast.weather[0].description.ToUpperCase();
            icon = WeatherManager.GetProvider(WeatherAPI.OpenWeatherMap)
                   .GetWeatherIcon(forecast.weather[0].id.ToInvariantString());

            // Extras
            extras = new ForecastExtras()
            {
                dewpoint_f = ConversionMethods.KtoF(forecast.dew_point),
                dewpoint_c = ConversionMethods.KtoC(forecast.dew_point),
                humidity = forecast.humidity,
                cloudiness = forecast.clouds,
                // 1hPA = 1mbar
                pressure_mb = forecast.pressure,
                pressure_in = ConversionMethods.MBToInHg(forecast.pressure),
                wind_degrees = forecast.wind_deg,
                wind_mph = (float)Math.Round(ConversionMethods.MSecToMph(forecast.wind_speed)),
                wind_kph = (float)Math.Round(ConversionMethods.MSecToKph(forecast.wind_speed)),
                uv_index = forecast.uvi
            };
            if (forecast.pop.HasValue)
            {
                extras.pop = (int?)Math.Round(forecast.pop.Value * 100);
            }
            if (forecast.visibility.HasValue)
            {
                extras.visibility_km = forecast.visibility.Value / 1000;
                extras.visibility_mi = ConversionMethods.KmToMi(extras.visibility_km.Value);
            }
            if (forecast.wind_gust.HasValue)
            {
                extras.windgust_mph = (float)Math.Round(ConversionMethods.MSecToMph(forecast.wind_gust.Value));
                extras.windgust_kph = (float)Math.Round(ConversionMethods.MSecToKph(forecast.wind_gust.Value));
            }
            if (forecast.rain.HasValue)
            {
                extras.qpf_rain_mm = forecast.rain.Value;
                extras.qpf_rain_in = ConversionMethods.MMToIn(forecast.rain.Value);
            }
            if (forecast.snow.HasValue)
            {
                extras.qpf_snow_cm = forecast.snow.Value / 10;
                extras.qpf_snow_in = ConversionMethods.MMToIn(forecast.snow.Value);
            }
        }
#endif
    }

    public partial class HourlyForecast
    {
        /* OpenWeather OneCall */
#if false
        public HourlyForecast(OpenWeather.Hourly hr_forecast)
        {
            date = DateTimeOffset.FromUnixTimeSeconds(hr_forecast.dt);
            high_f = ConversionMethods.KtoF(hr_forecast.temp);
            high_c = ConversionMethods.KtoC(hr_forecast.temp);
            condition = hr_forecast.weather[0].description.ToUpperCase();

            // Use icon to determine if day or night
            string ico = hr_forecast.weather[0].icon;
            string dn = ico.Last().ToString();

            if (int.TryParse(dn, NumberStyles.Integer, CultureInfo.InvariantCulture, out int x))
                dn = String.Empty;

            icon = WeatherManager.GetProvider(WeatherAPI.OpenWeatherMap)
                   .GetWeatherIcon(hr_forecast.weather[0].id.ToInvariantString() + dn);

            wind_degrees = hr_forecast.wind_deg;
            wind_mph = (float)Math.Round(ConversionMethods.MSecToMph(hr_forecast.wind_speed));
            wind_kph = (float)Math.Round(ConversionMethods.MSecToKph(hr_forecast.wind_speed));

            // Extras
            extras = new ForecastExtras()
            {
                feelslike_f = ConversionMethods.KtoF(hr_forecast.feels_like),
                feelslike_c = ConversionMethods.KtoC(hr_forecast.feels_like),
                dewpoint_f = ConversionMethods.KtoF(hr_forecast.dew_point),
                dewpoint_c = ConversionMethods.KtoC(hr_forecast.dew_point),
                humidity = hr_forecast.humidity,
                cloudiness = hr_forecast.clouds,
                // 1hPA = 1mbar
                pressure_mb = hr_forecast.pressure,
                pressure_in = ConversionMethods.MBToInHg(hr_forecast.pressure),
                wind_degrees = this.wind_degrees,
                wind_mph = this.wind_mph,
                wind_kph = this.wind_kph
            };
            if (hr_forecast.pop.HasValue)
            {
                extras.pop = (int)Math.Round(hr_forecast.pop.Value * 100);
            }
            if (hr_forecast.wind_gust.HasValue)
            {
                extras.windgust_mph = (float)Math.Round(ConversionMethods.MSecToMph(hr_forecast.wind_gust.Value));
                extras.windgust_kph = (float)Math.Round(ConversionMethods.MSecToKph(hr_forecast.wind_gust.Value));
            }
            if (hr_forecast.visibility.HasValue)
            {
                extras.visibility_km = hr_forecast.visibility.Value / 1000;
                extras.visibility_mi = ConversionMethods.KmToMi(extras.visibility_km.Value);
            }
            if (hr_forecast.rain != null)
            {
                extras.qpf_rain_mm = hr_forecast.rain._1h;
                extras.qpf_rain_in = ConversionMethods.MMToIn(hr_forecast.rain._1h);
            }
            if (hr_forecast.snow != null)
            {
                extras.qpf_snow_cm = hr_forecast.snow._1h / 10;
                extras.qpf_rain_in = ConversionMethods.MMToIn(hr_forecast.snow._1h);
            }
        }
#endif
    }

    public partial class TextForecast
    {
        /* OpenWeather OneCall */
#if false
        public TextForecast(OpenWeather.Daily forecast)
        {
            date = DateTimeOffset.FromUnixTimeSeconds(forecast.dt).DateTime;

            var sb = new StringBuilder();
            sb.AppendFormat(CultureInfo.InvariantCulture,
                "{0} - {1}: {2}°; {3}: {4}°", SimpleLibrary.GetInstance().ResLoader.GetString("Label_Morning"),
                SimpleLibrary.GetInstance().ResLoader.GetString("Temp_Label"),
                Math.Round(ConversionMethods.KtoF(forecast.temp.morn)),
                SimpleLibrary.GetInstance().ResLoader.GetString("FeelsLike_Label"),
                Math.Round(ConversionMethods.KtoF(forecast.feels_like.morn)));
            sb.AppendLine();
            sb.AppendFormat(CultureInfo.InvariantCulture,
                "{0} - {1}: {2}°; {3}: {4}°", SimpleLibrary.GetInstance().ResLoader.GetString("Label_Day"),
                SimpleLibrary.GetInstance().ResLoader.GetString("Temp_Label"),
                Math.Round(ConversionMethods.KtoF(forecast.temp.day)),
                SimpleLibrary.GetInstance().ResLoader.GetString("FeelsLike_Label"),
                Math.Round(ConversionMethods.KtoF(forecast.temp.day)));
            sb.AppendLine();
            sb.AppendFormat(CultureInfo.InvariantCulture,
                "{0} - {1}: {2}°; {3}: {4}°", SimpleLibrary.GetInstance().ResLoader.GetString("Label_Eve"),
                SimpleLibrary.GetInstance().ResLoader.GetString("Temp_Label"),
                Math.Round(ConversionMethods.KtoF(forecast.temp.eve)),
                SimpleLibrary.GetInstance().ResLoader.GetString("FeelsLike_Label"),
                Math.Round(ConversionMethods.KtoF(forecast.feels_like.eve)));
            sb.AppendLine();
            sb.AppendFormat(CultureInfo.InvariantCulture,
                "{0} - {1}: {2}°; {3}: {4}°", SimpleLibrary.GetInstance().ResLoader.GetString("Label_Night"),
                SimpleLibrary.GetInstance().ResLoader.GetString("Temp_Label"),
                Math.Round(ConversionMethods.KtoF(forecast.temp.night)),
                SimpleLibrary.GetInstance().ResLoader.GetString("FeelsLike_Label"),
                Math.Round(ConversionMethods.KtoF(forecast.feels_like.night)));

            fcttext = sb.ToString();

            var sb_metric = new StringBuilder();
            sb_metric.AppendFormat(CultureInfo.InvariantCulture,
                "{0} - {1}: {2}°; {3}: {4}°", SimpleLibrary.GetInstance().ResLoader.GetString("Label_Morning"),
                SimpleLibrary.GetInstance().ResLoader.GetString("Temp_Label"),
                Math.Round(ConversionMethods.KtoC(forecast.temp.morn)),
                SimpleLibrary.GetInstance().ResLoader.GetString("FeelsLike_Label"),
                Math.Round(ConversionMethods.KtoC(forecast.feels_like.morn)));
            sb_metric.AppendLine();
            sb_metric.AppendFormat(CultureInfo.InvariantCulture,
                "{0} - {1}: {2}°; {3}: {4}°", SimpleLibrary.GetInstance().ResLoader.GetString("Label_Day"),
                SimpleLibrary.GetInstance().ResLoader.GetString("Temp_Label"),
                Math.Round(ConversionMethods.KtoC(forecast.temp.day)),
                SimpleLibrary.GetInstance().ResLoader.GetString("FeelsLike_Label"),
                Math.Round(ConversionMethods.KtoC(forecast.feels_like.day)));
            sb_metric.AppendLine();
            sb_metric.AppendFormat(CultureInfo.InvariantCulture,
                "{0} - {1}: {2}°; {3}: {4}°", SimpleLibrary.GetInstance().ResLoader.GetString("Label_Eve"),
                SimpleLibrary.GetInstance().ResLoader.GetString("Temp_Label"),
                Math.Round(ConversionMethods.KtoC(forecast.temp.eve)),
                SimpleLibrary.GetInstance().ResLoader.GetString("FeelsLike_Label"),
                Math.Round(ConversionMethods.KtoC(forecast.feels_like.eve)));
            sb_metric.AppendLine();
            sb_metric.AppendFormat(CultureInfo.InvariantCulture,
                "{0} - {1}: {2}°; {3}: {4}°", SimpleLibrary.GetInstance().ResLoader.GetString("Label_Night"),
                SimpleLibrary.GetInstance().ResLoader.GetString("Temp_Label"),
                Math.Round(ConversionMethods.KtoC(forecast.temp.night)),
                SimpleLibrary.GetInstance().ResLoader.GetString("FeelsLike_Label"),
                Math.Round(ConversionMethods.KtoC(forecast.feels_like.night)));

            fcttext_metric = sb_metric.ToString();
        }
#endif
    }

    public partial class Condition
    {
        /* OpenWeather OneCall */
#if false
        public Condition(OpenWeather.Current current)
        {
            weather = current.weather[0].description.ToUpperCase();
            temp_f = ConversionMethods.KtoF(current.temp);
            temp_c = ConversionMethods.KtoC(current.temp);
            wind_degrees = current.wind_deg;
            wind_mph = ConversionMethods.MSecToMph(current.wind_speed);
            wind_kph = ConversionMethods.MSecToKph(current.wind_speed);
            feelslike_f = ConversionMethods.KtoF(current.feels_like);
            feelslike_c = ConversionMethods.KtoC(current.feels_like);
            if (current.wind_gust.HasValue)
            {
                windgust_mph = ConversionMethods.MSecToMph(current.wind_gust.Value);
                windgust_kph = ConversionMethods.MSecToKph(current.wind_gust.Value);
            }

            string ico = current.weather[0].icon;
            string dn = ico.Last().ToString();

            if (int.TryParse(dn, NumberStyles.Integer, CultureInfo.InvariantCulture, out int x))
                dn = String.Empty;

            icon = WeatherManager.GetProvider(WeatherAPI.OpenWeatherMap)
                   .GetWeatherIcon(current.weather[0].id.ToInvariantString() + dn);

            uv = new UV(current.uvi);
            beaufort = new Beaufort((int)WeatherUtils.GetBeaufortScale(current.wind_speed));

            observation_time = DateTimeOffset.FromUnixTimeSeconds(current.dt);
        }
#endif
    }

    public partial class Atmosphere
    {
        /* OpenWeather OneCall */
#if false
        public Atmosphere(OpenWeather.Current current)
        {
            humidity = current.humidity;
            // 1hPa = 1mbar
            pressure_mb = current.pressure;
            pressure_in = ConversionMethods.MBToInHg(current.pressure);
            pressure_trend = String.Empty;
            visibility_km = current.visibility / 1000;
            visibility_mi = ConversionMethods.KmToMi(visibility_km.Value);
            dewpoint_f = ConversionMethods.KtoF(current.dew_point);
            dewpoint_c = ConversionMethods.KtoC(current.dew_point);
        }
#endif
    }

    public partial class Astronomy
    {
        /* OpenWeather OneCall */
#if false
        public Astronomy(OpenWeather.Current current)
        {
            try
            {
                sunrise = DateTimeOffset.FromUnixTimeSeconds(current.sunrise).UtcDateTime;
            }
            catch (Exception) { }
            try
            {
                sunset = DateTimeOffset.FromUnixTimeSeconds(current.sunset).UtcDateTime;
            }
            catch (Exception) { }

            // If the sun won't set/rise, set time to the future
            if (sunrise == null)
            {
                sunrise = DateTime.Now.Date.AddYears(1).AddTicks(-1);
            }
            if (sunset == null)
            {
                sunset = DateTime.Now.Date.AddYears(1).AddTicks(-1);
            }
            if (moonrise == null)
            {
                moonrise = DateTime.MinValue;
            }
            if (moonset == null)
            {
                moonset = DateTime.MinValue;
            }
        }
#endif
    }

    public partial class Precipitation
    {
        /* OpenWeather OneCall */
#if false
        public Precipitation(OpenWeather.Current current)
        {
            // Use cloudiness value here
            cloudiness = current.clouds;
            if (current.rain != null)
            {
                qpf_rain_in = ConversionMethods.MMToIn(current.rain._1h);
                qpf_rain_mm = current.rain._1h;
            }
            if (current.snow != null)
            {
                qpf_snow_in = ConversionMethods.MMToIn(current.snow._1h);
                qpf_snow_cm = current.snow._1h / 10;
            }
        }
#endif
    }
}