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
        public Weather(OpenWeather.CurrentRootobject currRoot, OpenWeather.ForecastRootobject foreRoot)
        {
            location = new Location(foreRoot);
            update_time = DateTimeOffset.FromUnixTimeSeconds(currRoot.dt);

            // 5-day forecast / 3-hr forecast
            // 24hr / 3hr = 8items for each day
            forecast = new List<Forecast>(5);
            hr_forecast = new List<HourlyForecast>(foreRoot.list.Length);

            // Store potential min/max values
            float dayMax = float.NaN;
            float dayMin = float.NaN;
            int lastDay = -1;

            for (int i = 0; i < foreRoot.list.Length; i++)
            {
                hr_forecast.Add(new HourlyForecast(foreRoot.list[i]));

                float max = foreRoot.list[i].main.temp_max;
                if (!float.IsNaN(max) && (float.IsNaN(dayMax) || max > dayMax))
                {
                    dayMax = max;
                }

                float min = foreRoot.list[i].main.temp_min;
                if (!float.IsNaN(min) && (float.IsNaN(dayMin) || min < dayMin))
                {
                    dayMin = min;
                }

                // Add mid-day forecast
                var currHour = hr_forecast[i].date.AddSeconds(currRoot.timezone).Hour;
                if (currHour >= 11 && currHour <= 13)
                {
                    forecast.Add(new Forecast(foreRoot.list[i]));
                    lastDay = forecast.Count - 1;
                }

                // This is possibly the last forecast for the day (3-hrly forecast)
                // Set the min / max temp here and reset
                if (currHour >= 21)
                {
                    if (lastDay >= 0)
                    {
                        if (!float.IsNaN(dayMax))
                        {
                            forecast[lastDay].high_f = ConversionMethods.KtoF(dayMax);
                            forecast[lastDay].high_c = ConversionMethods.KtoC(dayMax);
                        }
                        if (!float.IsNaN(dayMin))
                        {
                            forecast[lastDay].low_f = ConversionMethods.KtoF(dayMin);
                            forecast[lastDay].low_c = ConversionMethods.KtoC(dayMin);
                        }
                    }

                    dayMax = float.NaN;
                    dayMin = float.NaN;
                }
            }
            condition = new Condition(currRoot);
            atmosphere = new Atmosphere(currRoot);
            astronomy = new Astronomy(currRoot);
            precipitation = new Precipitation(currRoot);
            ttl = 180;

            query = currRoot.id.ToString();

            // Set feelslike temp
            if (!condition.feelslike_f.HasValue && condition.temp_f.HasValue && condition.wind_mph.HasValue && atmosphere.humidity.HasValue)
            {
                condition.feelslike_f = WeatherUtils.GetFeelsLikeTemp(condition.temp_f.Value, condition.wind_mph.Value, atmosphere.humidity.Value);
                condition.feelslike_c = ConversionMethods.FtoC(condition.feelslike_f.Value);
            }

            if ((!condition.high_f.HasValue || !condition.low_f.HasValue) && forecast.Count > 0)
            {
                condition.high_f = forecast[0].high_f.Value;
                condition.high_c = forecast[0].high_c.Value;
                condition.low_f = forecast[0].low_f.Value;
                condition.low_c = forecast[0].low_c.Value;
            }

            if (!atmosphere.dewpoint_c.HasValue && condition.temp_c.HasValue && atmosphere.humidity.HasValue &&
                condition.temp_c > 0 && condition.temp_c < 60 && atmosphere.humidity > 1)
            {
                atmosphere.dewpoint_c = (float)Math.Round(WeatherUtils.CalculateDewpointC(condition.temp_c.Value, atmosphere.humidity.Value));
                atmosphere.dewpoint_f = (float)Math.Round(ConversionMethods.CtoF(atmosphere.dewpoint_c.Value));
            }

            source = WeatherAPI.OpenWeatherMap;
        }
    }

    public partial class Location
    {
        public Location(OpenWeather.ForecastRootobject root)
        {
            // Use location name from location provider
            name = null;
            latitude = root.city.coord.lat;
            longitude = root.city.coord.lon;
            tz_long = null;
        }
    }

    public partial class Forecast
    {
        public Forecast(OpenWeather.List forecast)
        {
            date = DateTimeOffset.FromUnixTimeSeconds(forecast.dt).DateTime;
            high_f = ConversionMethods.KtoF(forecast.main.temp_max);
            high_c = ConversionMethods.KtoC(forecast.main.temp_max);
            low_f = ConversionMethods.KtoF(forecast.main.temp_min);
            low_c = ConversionMethods.KtoC(forecast.main.temp_min);
            condition = forecast.weather[0].description.ToUpperCase();
            icon = WeatherManager.GetProvider(WeatherAPI.OpenWeatherMap)
                   .GetWeatherIcon(forecast.weather[0].id.ToInvariantString());

            // Extras
            extras = new ForecastExtras()
            {
                humidity = forecast.main.humidity,
                cloudiness = forecast.clouds.all,
                // 1hPA = 1mbar
                pressure_mb = forecast.main.pressure,
                pressure_in = ConversionMethods.MBToInHg(forecast.main.pressure),
                wind_degrees = (int)Math.Round(forecast.wind.deg),
                wind_mph = (float)Math.Round(ConversionMethods.MSecToMph(forecast.wind.speed)),
                wind_kph = (float)Math.Round(ConversionMethods.MSecToKph(forecast.wind.speed)),
            };
            if (ConversionMethods.KtoC(forecast.main.temp) is float temp_c &&
                temp_c > 0 && temp_c < 60 && forecast.main.humidity > 1)
            {
                extras.dewpoint_c = (float)Math.Round(WeatherUtils.CalculateDewpointC(ConversionMethods.KtoC(forecast.main.temp), forecast.main.humidity));
                extras.dewpoint_f = (float)Math.Round(ConversionMethods.CtoF(extras.dewpoint_c.Value));
            }
            if (forecast.main.feels_like.HasValue)
            {
                extras.feelslike_f = ConversionMethods.KtoF(forecast.main.feels_like.Value);
                extras.feelslike_c = ConversionMethods.KtoC(forecast.main.feels_like.Value);
            }
            if (forecast.pop.HasValue)
            {
                extras.pop = (int)Math.Round(forecast.pop.Value * 100);
            }
            if (forecast.visibility.HasValue)
            {
                extras.visibility_km = forecast.visibility.Value / 1000;
                extras.visibility_mi = ConversionMethods.KmToMi(extras.visibility_km.Value);
            }
            if (forecast.wind.gust.HasValue)
            {
                extras.windgust_mph = (float)Math.Round(ConversionMethods.MSecToMph(forecast.wind.gust.Value));
                extras.windgust_kph = (float)Math.Round(ConversionMethods.MSecToKph(forecast.wind.gust.Value));
            }
            if (forecast.rain?._3h.HasValue == true)
            {
                extras.qpf_rain_mm = forecast.rain._3h.Value;
                extras.qpf_rain_in = ConversionMethods.MMToIn(forecast.rain._3h.Value);
            }
            if (forecast.snow?._3h.HasValue == true)
            {
                extras.qpf_snow_cm = forecast.snow._3h.Value / 10;
                extras.qpf_snow_in = ConversionMethods.MMToIn(forecast.snow._3h.Value);
            }
        }
    }

    public partial class HourlyForecast
    {
        public HourlyForecast(OpenWeather.List hr_forecast)
        {
            date = DateTimeOffset.FromUnixTimeSeconds(hr_forecast.dt);
            high_f = ConversionMethods.KtoF(hr_forecast.main.temp);
            high_c = ConversionMethods.KtoC(hr_forecast.main.temp);
            condition = hr_forecast.weather[0].description.ToUpperCase();

            // Use icon to determine if day or night
            string ico = hr_forecast.weather[0].icon;
            string dn = ico.Last().ToString();

            if (int.TryParse(dn, NumberStyles.Integer, CultureInfo.InvariantCulture, out int x))
                dn = String.Empty;

            icon = WeatherManager.GetProvider(WeatherAPI.OpenWeatherMap)
                   .GetWeatherIcon(hr_forecast.weather[0].id.ToInvariantString() + dn);

            wind_degrees = (int)hr_forecast.wind.deg;
            wind_mph = (float)Math.Round(ConversionMethods.MSecToMph(hr_forecast.wind.speed));
            wind_kph = (float)Math.Round(ConversionMethods.MSecToKph(hr_forecast.wind.speed));

            // Extras
            extras = new ForecastExtras
            {
                humidity = hr_forecast.main.humidity,
                cloudiness = hr_forecast.clouds.all,
                // 1hPA = 1mbar
                pressure_mb = hr_forecast.main.pressure,
                pressure_in = ConversionMethods.MBToInHg(hr_forecast.main.pressure),
                wind_degrees = wind_degrees,
                wind_mph = wind_mph,
                wind_kph = wind_kph
            };
            if (high_c.Value > 0 && high_c.Value < 60 && hr_forecast.main.humidity > 1)
            {
                extras.dewpoint_c = (float)Math.Round(WeatherUtils.CalculateDewpointC(high_c.Value, hr_forecast.main.humidity));
                extras.dewpoint_f = (float)Math.Round(ConversionMethods.CtoF(extras.dewpoint_c.Value));
            }
            if (hr_forecast.main.feels_like.HasValue)
            {
                extras.feelslike_f = ConversionMethods.KtoF(hr_forecast.main.feels_like.Value);
                extras.feelslike_c = ConversionMethods.KtoC(hr_forecast.main.feels_like.Value);
            }
            if (hr_forecast.pop.HasValue)
            {
                extras.pop = (int)Math.Round(hr_forecast.pop.Value * 100);
            }
            if (hr_forecast.wind.gust.HasValue)
            {
                extras.windgust_mph = (float)Math.Round(ConversionMethods.MSecToMph(hr_forecast.wind.gust.Value));
                extras.windgust_kph = (float)Math.Round(ConversionMethods.MSecToKph(hr_forecast.wind.gust.Value));
            }
            if (hr_forecast.visibility.HasValue)
            {
                extras.visibility_km = hr_forecast.visibility.Value / 1000;
                extras.visibility_mi = ConversionMethods.KmToMi(extras.visibility_km.Value);
            }
            if (hr_forecast.rain?._3h.HasValue == true)
            {
                extras.qpf_rain_mm = hr_forecast.rain._3h.Value;
                extras.qpf_rain_in = ConversionMethods.MMToIn(hr_forecast.rain._3h.Value);
            }
            if (hr_forecast.snow?._3h.HasValue == true)
            {
                extras.qpf_snow_cm = hr_forecast.snow._3h.Value / 10;
                extras.qpf_snow_in = ConversionMethods.MMToIn(hr_forecast.snow._3h.Value);
            }
        }
    }

    public partial class Condition
    {
        public Condition(OpenWeather.CurrentRootobject current)
        {
            weather = current.weather[0].description.ToUpperCase();
            temp_f = ConversionMethods.KtoF(current.main.temp);
            temp_c = ConversionMethods.KtoC(current.main.temp);
            high_f = ConversionMethods.KtoF(current.main.temp_max);
            high_c = ConversionMethods.KtoC(current.main.temp_max);
            low_f = ConversionMethods.KtoF(current.main.temp_min);
            low_c = ConversionMethods.KtoC(current.main.temp_min);
            wind_degrees = (int)current.wind.deg;
            wind_mph = ConversionMethods.MSecToMph(current.wind.speed);
            wind_kph = ConversionMethods.MSecToKph(current.wind.speed);
            if (current.main.feels_like.HasValue)
            {
                feelslike_f = ConversionMethods.KtoF(current.main.feels_like.Value);
                feelslike_c = ConversionMethods.KtoC(current.main.feels_like.Value);
            }
            if (current.wind.gust.HasValue)
            {
                windgust_mph = ConversionMethods.MSecToMph(current.wind.gust.Value);
                windgust_kph = ConversionMethods.MSecToKph(current.wind.gust.Value);
            }

            string ico = current.weather[0].icon;
            string dn = ico.Last().ToString();

            if (int.TryParse(dn, NumberStyles.Integer, CultureInfo.InvariantCulture, out int x))
                dn = String.Empty;

            icon = WeatherManager.GetProvider(WeatherAPI.OpenWeatherMap)
                   .GetWeatherIcon(current.weather[0].id.ToInvariantString() + dn);

            beaufort = new Beaufort((int)WeatherUtils.GetBeaufortScale(current.wind.speed));

            observation_time = DateTimeOffset.FromUnixTimeSeconds(current.dt);
        }
    }

    public partial class Atmosphere
    {
        public Atmosphere(OpenWeather.CurrentRootobject root)
        {
            humidity = root.main.humidity;
            // 1hPa = 1mbar
            pressure_mb = root.main.pressure;
            pressure_in = ConversionMethods.MBToInHg(root.main.pressure);
            pressure_trend = String.Empty;
            visibility_km = root.visibility / 1000;
            visibility_mi = ConversionMethods.KmToMi(visibility_km.Value);
        }
    }

    public partial class Astronomy
    {
        public Astronomy(OpenWeather.CurrentRootobject root)
        {
            try
            {
                sunrise = DateTimeOffset.FromUnixTimeSeconds(root.sys.sunrise.Value).UtcDateTime;
            }
            catch (Exception) { }
            try
            {
                sunset = DateTimeOffset.FromUnixTimeSeconds(root.sys.sunset.Value).UtcDateTime;
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
    }

    public partial class Precipitation
    {
        public Precipitation(OpenWeather.CurrentRootobject root)
        {
            // Use cloudiness value here
            cloudiness = root.clouds.all;
            if (root.rain?._1h.HasValue == true)
            {
                qpf_rain_in = ConversionMethods.MMToIn(root.rain._1h.Value);
                qpf_rain_mm = root.rain._1h.Value;
            }
            else if (root.rain?._3h.HasValue == true)
            {
                qpf_rain_in = ConversionMethods.MMToIn(root.rain._3h.Value);
                qpf_rain_mm = root.rain._3h.Value;
            }

            if (root.snow?._1h.HasValue == true)
            {
                qpf_snow_in = ConversionMethods.MMToIn(root.snow._1h.Value);
                qpf_snow_cm = root.snow._1h.Value / 10;
            }
            else if (root.snow?._3h.HasValue == true)
            {
                qpf_snow_in = ConversionMethods.MMToIn(root.snow._3h.Value);
                qpf_snow_cm = root.snow._3h.Value / 10;
            }
        }
    }
}