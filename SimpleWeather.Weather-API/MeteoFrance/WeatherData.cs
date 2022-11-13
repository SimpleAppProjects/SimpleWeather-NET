using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SimpleWeather.Weather_API.MeteoFrance
{
    public static partial class MeteoFranceProviderExtensions
    {
        public static SimpleWeather.WeatherData.Weather CreateWeatherData(this MeteoFranceProvider _, MeteoFrance.CurrentsRootobject currRoot,
            MeteoFrance.ForecastRootobject foreRoot, MeteoFrance.AlertsRootobject alertRoot)
        {
            var weather = new SimpleWeather.WeatherData.Weather();

            weather.location = _.CreateLocation(foreRoot);
            weather.update_time = DateTimeOffset.UtcNow;

            weather.forecast = new List<SimpleWeather.WeatherData.Forecast>(foreRoot.daily_forecast.Length);
            weather.hr_forecast = new List<SimpleWeather.WeatherData.HourlyForecast>(foreRoot.forecast.Length);

            // Forecast
            foreach (var daily in foreRoot.daily_forecast)
            {
                weather.forecast.Add(_.CreateForecast(daily));
            }
            foreach (var hourly in foreRoot.forecast)
            {
                weather.hr_forecast.Add(_.CreateHourlyForecast(hourly, foreRoot.probability_forecast));
            }

            weather.condition = _.CreateCondition(currRoot);
            weather.atmosphere = _.CreateAtmosphere(currRoot);

            weather.ttl = 180;

            // Observation only gives temp & wind
            if (weather.hr_forecast.Any())
            {
                var firstHr = weather.hr_forecast[0];

                weather.atmosphere.humidity = firstHr.extras?.humidity;
                weather.atmosphere.pressure_mb = firstHr.extras?.pressure_mb;
                weather.atmosphere.pressure_in = firstHr.extras?.pressure_in;

                weather.precipitation = new SimpleWeather.WeatherData.Precipitation()
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
            if (!weather.condition.feelslike_f.HasValue && weather.condition.temp_f.HasValue && weather.condition.wind_mph.HasValue && weather.atmosphere.humidity.HasValue)
            {
                weather.condition.feelslike_f = WeatherUtils.GetFeelsLikeTemp(weather.condition.temp_f.Value, weather.condition.wind_mph.Value, weather.atmosphere.humidity.Value);
                weather.condition.feelslike_c = ConversionMethods.FtoC(weather.condition.feelslike_f.Value);
            }

            if ((!weather.condition.high_f.HasValue || !weather.condition.low_f.HasValue) && weather.forecast.Count > 0)
            {
                weather.condition.high_f = weather.forecast[0].high_f.Value;
                weather.condition.high_c = weather.forecast[0].high_c.Value;
                weather.condition.low_f = weather.forecast[0].low_f.Value;
                weather.condition.low_c = weather.forecast[0].low_c.Value;
            }

            weather.weather_alerts = _.CreateWeatherAlerts(alertRoot);

            weather.source = WeatherAPI.MeteoFrance;

            return weather;
        }

        public static SimpleWeather.WeatherData.Location CreateLocation(this MeteoFranceProvider _, MeteoFrance.ForecastRootobject foreRoot)
        {
            // Use location name from location provider
            return new Location()
            {
                name = null,
                latitude = foreRoot?.position?.lat,
                longitude = foreRoot?.position?.lon,
                tz_long = foreRoot?.position?.timezone,
            };
        }

        public static SimpleWeather.WeatherData.Forecast CreateForecast(this MeteoFranceProvider _, MeteoFrance.Daily_Forecast day)
        {
            var forecast = new SimpleWeather.WeatherData.Forecast();

            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.MeteoFrance);
            var culture = CultureUtils.UserCulture;

            forecast.date = DateTimeOffset.FromUnixTimeSeconds(day.dt).UtcDateTime;

            if (day.T?.max.HasValue == true)
            {
                forecast.high_c = day.T.max;
                forecast.high_f = ConversionMethods.CtoF(forecast.high_c.Value);
            }

            if (day.T?.min.HasValue == true)
            {
                forecast.low_c = day.T.min;
                forecast.low_f = ConversionMethods.CtoF(forecast.low_c.Value);
            }

            if (culture.TwoLetterISOLanguageName.Equals("en", StringComparison.InvariantCultureIgnoreCase) ||
                culture.TwoLetterISOLanguageName.Equals("fr", StringComparison.InvariantCultureIgnoreCase) ||
                culture.Equals(CultureInfo.InvariantCulture))
            {
                forecast.condition = day.weather12H.desc;
            }
            else
            {
                forecast.condition = provider.GetWeatherCondition(day.weather12H.icon);
            }
            forecast.icon = provider.GetWeatherIcon(false, day.weather12H.icon);

            forecast.extras = new ForecastExtras();
            if (day.humidity.max.HasValue && day.humidity.min.HasValue)
            {
                forecast.extras.humidity = (int)Math.Round((day.humidity.min.Value + day.humidity.max.Value) / 2f);
            }
            if (day.T.sea.HasValue)
            {
                forecast.extras.pressure_mb = day.T.sea;
                forecast.extras.pressure_in = ConversionMethods.MBToInHg(day.T.sea.Value);
            }
            if (day.precipitation._24h.HasValue)
            {
                forecast.extras.qpf_rain_mm = day.precipitation._24h;
                forecast.extras.qpf_rain_in = ConversionMethods.MMToIn(day.precipitation._24h.Value);
            }
            forecast.extras.uv_index = day.uv;

            return forecast;
        }

        public static SimpleWeather.WeatherData.HourlyForecast CreateHourlyForecast(this MeteoFranceProvider _, MeteoFrance.Forecast forecast, MeteoFrance.Probability_Forecast[] probabilityForecasts)
        {
            var hrf = new SimpleWeather.WeatherData.HourlyForecast();

            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.MeteoFrance);
            var culture = CultureUtils.UserCulture;

            hrf.date = DateTimeOffset.FromUnixTimeSeconds(forecast.dt);

            if (forecast.T?.value.HasValue == true)
            {
                hrf.high_c = forecast.T.value;
                hrf.high_f = ConversionMethods.CtoF(forecast.T.value.Value);
            }

            if (culture.TwoLetterISOLanguageName.Equals("en", StringComparison.InvariantCultureIgnoreCase) ||
                culture.TwoLetterISOLanguageName.Equals("fr", StringComparison.InvariantCultureIgnoreCase) ||
                culture.Equals(CultureInfo.InvariantCulture))
            {
                hrf.condition = forecast.weather.desc;
            }
            else
            {
                hrf.condition = provider.GetWeatherCondition(forecast.weather.icon);
            }
            hrf.icon = forecast.weather.icon;

            // Extras
            hrf.extras = new ForecastExtras();

            if (forecast.T.windchill.HasValue)
            {
                hrf.extras.feelslike_c = forecast.T.windchill;
                hrf.extras.feelslike_f = ConversionMethods.CtoF(forecast.T.windchill.Value);
            }

            hrf.extras.humidity = forecast.humidity;

            if (forecast.sea_level.HasValue)
            {
                hrf.extras.pressure_mb = forecast.sea_level;
                hrf.extras.pressure_in = ConversionMethods.MBToInHg(forecast.sea_level.Value);
            }

            if (forecast.wind != null)
            {
                if (forecast.wind.speed.HasValue && forecast.wind.direction.HasValue)
                {
                    hrf.wind_degrees = forecast.wind.direction;
                    hrf.wind_kph = ConversionMethods.MSecToKph(forecast.wind.speed.Value);
                    hrf.wind_mph = ConversionMethods.MSecToMph(forecast.wind.speed.Value);

                    hrf.extras.wind_degrees = hrf.wind_degrees;
                    hrf.extras.wind_mph = hrf.wind_mph;
                    hrf.extras.wind_kph = hrf.wind_kph;
                }

                if (forecast.wind.gust.HasValue)
                {
                    hrf.extras.windgust_kph = ConversionMethods.MSecToKph(forecast.wind.gust.Value);
                    hrf.extras.windgust_mph = ConversionMethods.MSecToMph(forecast.wind.gust.Value);
                }
            }

            if (forecast.rain != null)
            {
                if (forecast.rain._1h.HasValue)
                {
                    hrf.extras.qpf_rain_mm = forecast.rain._1h;
                    hrf.extras.qpf_rain_in = ConversionMethods.MMToIn(forecast.rain._1h.Value);
                }
                else if (forecast.rain._3h.HasValue)
                {
                    hrf.extras.qpf_rain_mm = forecast.rain._3h;
                    hrf.extras.qpf_rain_in = ConversionMethods.MMToIn(forecast.rain._3h.Value);
                }
                else if (forecast.rain._6h.HasValue)
                {
                    hrf.extras.qpf_rain_mm = forecast.rain._6h;
                    hrf.extras.qpf_rain_in = ConversionMethods.MMToIn(forecast.rain._6h.Value);
                }
            }

            if (forecast.snow != null)
            {
                if (forecast.snow._1h.HasValue)
                {
                    hrf.extras.qpf_snow_cm = forecast.snow._1h / 10;
                    hrf.extras.qpf_snow_in = ConversionMethods.MMToIn(forecast.snow._1h.Value);
                }
                else if (forecast.snow._3h.HasValue)
                {
                    hrf.extras.qpf_snow_cm = forecast.snow._3h / 10;
                    hrf.extras.qpf_snow_in = ConversionMethods.MMToIn(forecast.snow._3h.Value);
                }
                else if (forecast.snow._6h.HasValue)
                {
                    hrf.extras.qpf_snow_cm = forecast.snow._6h / 10;
                    hrf.extras.qpf_snow_in = ConversionMethods.MMToIn(forecast.snow._6h.Value);
                }
            }

            hrf.extras.cloudiness = forecast.clouds;

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
                            hrf.extras.pop = (int)prob.rain._3h.Value;
                            found3hrForecast = true;
                            _3hrForecastNA = false;
                        }
                        else
                        {
                            found3hrForecast = false;
                            _3hrForecastNA = true;
                        }
                        if (!hrf.extras.pop.HasValue && prob.rain._6h.HasValue)
                        {
                            hrf.extras.pop = (int)prob.rain._6h.Value;
                            _3hrForecastNA = true;
                            found3hrForecast = false;
                        }
                        if (!hrf.extras.pop.HasValue)
                        {
                            if (prob.snow._3h.HasValue)
                            {
                                hrf.extras.pop = (int)prob.snow._3h.Value;
                            }
                            if (prob.snow._6h.HasValue)
                            {
                                hrf.extras.pop = (int)prob.snow._6h.Value;
                            }
                        }
                    }

                    // Timestamp is not within 3-hr forecast; check 6-hr timeframe
                    // Check if timestamp is within 6-hr forecast
                    if (!hrf.extras.pop.HasValue && (dt == (prob.dt + hrsInSec * 3) || dt == (prob.dt + hrsInSec * 4) || dt == (prob.dt + hrsInSec * 5)))
                    {
                        if (prob.rain._6h.HasValue)
                        {
                            hrf.extras.pop = (int)prob.rain._6h.Value;
                            _3hrForecastNA = true;
                            found3hrForecast = false;
                        }
                        if (!hrf.extras.pop.HasValue)
                        {
                            if (prob.snow._6h.HasValue)
                            {
                                hrf.extras.pop = (int)prob.snow._6h.Value;
                            }
                        }
                    }

                    if (hrf.extras.pop.HasValue && (found3hrForecast || _3hrForecastNA)) break;
                }
            }

            return hrf;
        }

        public static SimpleWeather.WeatherData.Condition CreateCondition(this MeteoFranceProvider _, MeteoFrance.CurrentsRootobject currRoot)
        {
            var condition = new SimpleWeather.WeatherData.Condition();

            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.MeteoFrance);
            var culture = CultureUtils.UserCulture;

            if (currRoot?.observation?.T.HasValue == true)
            {
                condition.temp_c = currRoot.observation.T;
                condition.temp_f = ConversionMethods.CtoF(condition.temp_c.Value);
            }

            if (culture.TwoLetterISOLanguageName.Equals("en", StringComparison.InvariantCultureIgnoreCase) ||
                culture.TwoLetterISOLanguageName.Equals("fr", StringComparison.InvariantCultureIgnoreCase) ||
                culture.Equals(CultureInfo.InvariantCulture))
            {
                condition.weather = currRoot.observation.weather.desc;
            }
            else
            {
                condition.weather = provider.GetWeatherCondition(currRoot.observation.weather.icon);
            }
            condition.icon = currRoot.observation.weather.icon;

            if (currRoot.observation.wind != null)
            {
                condition.wind_degrees = currRoot.observation.wind.direction;
                if (currRoot.observation.wind.speed.HasValue)
                {
                    condition.wind_kph = ConversionMethods.MSecToKph(currRoot.observation.wind.speed.Value);
                    condition.wind_mph = ConversionMethods.MSecToMph(currRoot.observation.wind.speed.Value);
                }
            }

            condition.observation_time = DateTimeOffset.FromUnixTimeSeconds(currRoot.updated_on);

            return condition;
        }

        public static SimpleWeather.WeatherData.Atmosphere CreateAtmosphere(this MeteoFranceProvider _, MeteoFrance.CurrentsRootobject currRoot)
        {
            // no-op
            return new Atmosphere();
        }
    }
}