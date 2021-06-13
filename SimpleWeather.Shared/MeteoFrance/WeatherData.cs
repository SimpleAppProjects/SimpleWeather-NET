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
        public Weather(MeteoFrance.CurrentsRootobject currRoot,
            MeteoFrance.ForecastRootobject foreRoot, MeteoFrance.AlertsRootobject alertRoot)
        {
            location = new Location(foreRoot);
            update_time = DateTimeOffset.UtcNow;

            forecast = new List<Forecast>(foreRoot.daily_forecast.Length);
            hr_forecast = new List<HourlyForecast>(foreRoot.forecast.Length);

            // Forecast
            foreach (var daily in foreRoot.daily_forecast)
            {
                forecast.Add(new Forecast(daily));
            }
            foreach (var hourly in foreRoot.forecast)
            {
                hr_forecast.Add(new HourlyForecast(hourly, foreRoot.probability_forecast));
            }

            condition = new Condition(currRoot);
            atmosphere = new Atmosphere(currRoot);

            ttl = 180;

            // Observation only gives temp & wind
            if (hr_forecast.Any())
            {
                var firstHr = hr_forecast[0];

                atmosphere.humidity = firstHr.extras?.humidity;
                atmosphere.pressure_mb = firstHr.extras?.pressure_mb;
                atmosphere.pressure_in = firstHr.extras?.pressure_in;

                precipitation = new Precipitation()
                {
                    cloudiness = firstHr.extras?.cloudiness,
                    pop = firstHr.extras?.pop,
                    qpf_rain_in = firstHr.extras?.qpf_rain_in,
                    qpf_rain_mm = firstHr.extras?.qpf_rain_mm,
                    qpf_snow_in = firstHr.extras?.qpf_snow_in,
                    qpf_snow_cm = firstHr.extras?.qpf_snow_cm
                };
            }

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

            if (alertRoot?.color_max > 1 && alertRoot?.phenomenons_items != null)
            {
                weather_alerts = new HashSet<WeatherAlert>(alertRoot.phenomenons_items.Length);
                foreach (var phenom in alertRoot.phenomenons_items)
                {
                    // phenomenon_max_color_id == 1 -> OK / No Warning or Watches
                    if (phenom?.phenomenon_max_color_id > 1)
                    {
                        var alert = new WeatherAlert(phenom)
                        {
                            Date = DateTimeOffset.FromUnixTimeSeconds(alertRoot.update_time),
                            ExpiresDate = DateTimeOffset.FromUnixTimeSeconds(alertRoot.end_validity_time)
                        };
                        weather_alerts.Add(alert);
                    }
                }
            }

            source = WeatherAPI.MeteoFrance;
        }
    }

    public partial class Location
    {
        public Location(MeteoFrance.ForecastRootobject foreRoot)
        {
            // Use location name from location provider
            name = null;
            latitude = foreRoot?.position?.lat;
            longitude = foreRoot?.position?.lon;
            tz_long = foreRoot?.position?.timezone;
        }
    }

    public partial class Forecast
    {
        public Forecast(MeteoFrance.Daily_Forecast day)
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.MeteoFrance);
            var culture = CultureUtils.UserCulture;

            date = DateTimeOffset.FromUnixTimeSeconds(day.dt).UtcDateTime;

            if (day.T?.max.HasValue == true)
            {
                high_c = day.T.max;
                high_f = ConversionMethods.CtoF(high_c.Value);
            }

            if (day.T?.min.HasValue == true)
            {
                low_c = day.T.min;
                low_f = ConversionMethods.CtoF(low_c.Value);
            }

            if (culture.TwoLetterISOLanguageName.Equals("en", StringComparison.InvariantCultureIgnoreCase) ||
                culture.TwoLetterISOLanguageName.Equals("fr", StringComparison.InvariantCultureIgnoreCase) ||
                culture.Equals(CultureInfo.InvariantCulture))
            {
                condition = day.weather12H.desc;
            }
            else
            {
                condition = provider.GetWeatherCondition(day.weather12H.icon);
            }
            icon = provider.GetWeatherIcon(false, day.weather12H.icon);

            extras = new ForecastExtras();
            if (day.humidity.max.HasValue && day.humidity.min.HasValue)
            {
                extras.humidity = (int)Math.Round((day.humidity.min.Value + day.humidity.max.Value) / 2f);
            }
            if (day.T.sea.HasValue)
            {
                extras.pressure_mb = day.T.sea;
                extras.pressure_in = ConversionMethods.MBToInHg(day.T.sea.Value);
            }
            if (day.precipitation._24h.HasValue)
            {
                extras.qpf_rain_mm = day.precipitation._24h;
                extras.qpf_rain_in = ConversionMethods.MMToIn(day.precipitation._24h.Value);
            }
            extras.uv_index = day.uv;
        }
    }

    public partial class HourlyForecast
    {
        public HourlyForecast(MeteoFrance.Forecast forecast, MeteoFrance.Probability_Forecast[] probabilityForecasts)
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.MeteoFrance);
            var culture = CultureUtils.UserCulture;

            date = DateTimeOffset.FromUnixTimeSeconds(forecast.dt);

            if (forecast.T?.value.HasValue == true)
            {
                high_c = forecast.T.value;
                high_f = ConversionMethods.CtoF(forecast.T.value.Value);
            }

            if (culture.TwoLetterISOLanguageName.Equals("en", StringComparison.InvariantCultureIgnoreCase) ||
                culture.TwoLetterISOLanguageName.Equals("fr", StringComparison.InvariantCultureIgnoreCase) ||
                culture.Equals(CultureInfo.InvariantCulture))
            {
                condition = forecast.weather.desc;
            }
            else
            {
                condition = provider.GetWeatherCondition(forecast.weather.icon);
            }
            icon = forecast.weather.icon;

            // Extras
            extras = new ForecastExtras();

            if (forecast.T.windchill.HasValue)
            {
                extras.feelslike_c = forecast.T.windchill;
                extras.feelslike_f = ConversionMethods.CtoF(forecast.T.windchill.Value);
            }

            extras.humidity = forecast.humidity;

            if (forecast.sea_level.HasValue)
            {
                extras.pressure_mb = forecast.sea_level;
                extras.pressure_in = ConversionMethods.MBToInHg(forecast.sea_level.Value);
            }

            if (forecast.wind != null)
            {
                if (forecast.wind.speed.HasValue && forecast.wind.direction.HasValue)
                {
                    wind_degrees = forecast.wind.direction;
                    wind_kph = ConversionMethods.MSecToKph(forecast.wind.speed.Value);
                    wind_mph = ConversionMethods.MSecToMph(forecast.wind.speed.Value);

                    extras.wind_degrees = wind_degrees;
                    extras.wind_mph = wind_mph;
                    extras.wind_kph = wind_kph;
                }

                if (forecast.wind.gust.HasValue)
                {
                    extras.windgust_kph = ConversionMethods.MSecToKph(forecast.wind.gust.Value);
                    extras.windgust_mph = ConversionMethods.MSecToMph(forecast.wind.gust.Value);
                }
            }

            if (forecast.rain != null)
            {
                if (forecast.rain._1h.HasValue)
                {
                    extras.qpf_rain_mm = forecast.rain._1h;
                    extras.qpf_rain_in = ConversionMethods.MMToIn(forecast.rain._1h.Value);
                }
                else if (forecast.rain._3h.HasValue)
                {
                    extras.qpf_rain_mm = forecast.rain._3h;
                    extras.qpf_rain_in = ConversionMethods.MMToIn(forecast.rain._3h.Value);
                }
                else if (forecast.rain._6h.HasValue)
                {
                    extras.qpf_rain_mm = forecast.rain._6h;
                    extras.qpf_rain_in = ConversionMethods.MMToIn(forecast.rain._6h.Value);
                }
            }

            if (forecast.snow != null)
            {
                if (forecast.snow._1h.HasValue)
                {
                    extras.qpf_snow_cm = forecast.snow._1h / 10;
                    extras.qpf_snow_in = ConversionMethods.MMToIn(forecast.snow._1h.Value);
                }
                else if (forecast.snow._3h.HasValue)
                {
                    extras.qpf_snow_cm = forecast.snow._3h / 10;
                    extras.qpf_snow_in = ConversionMethods.MMToIn(forecast.snow._3h.Value);
                }
                else if (forecast.snow._6h.HasValue)
                {
                    extras.qpf_snow_cm = forecast.snow._6h / 10;
                    extras.qpf_snow_in = ConversionMethods.MMToIn(forecast.snow._6h.Value);
                }
            }

            extras.cloudiness = forecast.clouds;

            if (probabilityForecasts?.Any() == true)
            {
                // Note: probability forecasts are given either every 3 or 6 hours
                // Rain/Snow object can contain forecast for either next 3 or 6 hrs, or both
                var dt = forecast.dt; // Unix time in seconds
                var hrsInSec = TimeSpan.FromHours(1).TotalSeconds;
                var found3hrForecast = false;
                var _3hrForecastNA = false;

                foreach (var prob in probabilityForecasts)
                {
                    // Check if timestamp is within 3-hr forecast
                    if (dt == prob.dt || dt == (prob.dt + hrsInSec) || dt == (prob.dt + hrsInSec * 2))
                    {
                        if (prob.rain._3h.HasValue)
                        {
                            extras.pop = (int)prob.rain._3h.Value;
                            found3hrForecast = true;
                            _3hrForecastNA = false;
                        }
                        else
                        {
                            found3hrForecast = false;
                            _3hrForecastNA = true;
                        }
                        if (!extras.pop.HasValue && prob.rain._6h.HasValue)
                        {
                            extras.pop = (int)prob.rain._6h.Value;
                            _3hrForecastNA = true;
                            found3hrForecast = false;
                        }
                        if (!extras.pop.HasValue)
                        {
                            if (prob.snow._3h.HasValue)
                            {
                                extras.pop = (int)prob.snow._3h.Value;
                            }
                            if (prob.snow._6h.HasValue)
                            {
                                extras.pop = (int)prob.snow._6h.Value;
                            }
                        }
                    }

                    // Timestamp is not within 3-hr forecast; check 6-hr timeframe
                    // Check if timestamp is within 6-hr forecast
                    if (!extras.pop.HasValue && (dt == (prob.dt + hrsInSec * 3) || dt == (prob.dt + hrsInSec * 4) || dt == (prob.dt + hrsInSec * 5)))
                    {
                        if (prob.rain._6h.HasValue)
                        {
                            extras.pop = (int)prob.rain._6h.Value;
                            _3hrForecastNA = true;
                            found3hrForecast = false;
                        }
                        if (!extras.pop.HasValue)
                        {
                            if (prob.snow._6h.HasValue)
                            {
                                extras.pop = (int)prob.snow._6h.Value;
                            }
                        }
                    }

                    if (extras.pop.HasValue && (found3hrForecast || _3hrForecastNA)) break;
                }
            }
        }
    }

    public partial class Condition
    {
        public Condition(MeteoFrance.CurrentsRootobject currRoot)
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.MeteoFrance);
            var culture = CultureUtils.UserCulture;

            if (currRoot?.observation?.T.HasValue == true)
            {
                temp_c = currRoot.observation.T;
                temp_f = ConversionMethods.CtoF(temp_c.Value);
            }

            if (culture.TwoLetterISOLanguageName.Equals("en", StringComparison.InvariantCultureIgnoreCase) ||
                culture.TwoLetterISOLanguageName.Equals("fr", StringComparison.InvariantCultureIgnoreCase) ||
                culture.Equals(CultureInfo.InvariantCulture))
            {
                weather = currRoot.observation.weather.desc;
            }
            else
            {
                weather = provider.GetWeatherCondition(currRoot.observation.weather.icon);
            }
            icon = currRoot.observation.weather.icon;

            if (currRoot.observation.wind != null)
            {
                wind_degrees = currRoot.observation.wind.direction;
                if (currRoot.observation.wind.speed.HasValue)
                {
                    wind_kph = ConversionMethods.MSecToKph(currRoot.observation.wind.speed.Value);
                    wind_mph = ConversionMethods.MSecToMph(currRoot.observation.wind.speed.Value);
                }
            }

            observation_time = DateTimeOffset.FromUnixTimeSeconds(currRoot.updated_on);
        }
    }

    public partial class Atmosphere
    {
        public Atmosphere(MeteoFrance.CurrentsRootobject currRoot)
        {
            // no-op
        }
    }
}