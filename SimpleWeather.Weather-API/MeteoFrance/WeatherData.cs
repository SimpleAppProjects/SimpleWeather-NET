#nullable enable
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Globalization;
using System.Linq;

namespace SimpleWeather.Weather_API.MeteoFrance
{
    public static partial class MeteoFranceProviderExtensions
    {
        public static Weather CreateWeatherData(this MeteoFranceProvider _, CurrentsRootobject currRoot,
            ForecastRootobject foreRoot, AlertsRootobject? alertRoot = null)
        {
            var weather = new Weather();

            weather.location = _.CreateLocation(foreRoot);
            weather.update_time = DateTimeOffset.UtcNow;

            // Forecast
            weather.forecast = foreRoot.properties?.daily_forecast?.Select(daily => _.CreateForecast(daily))?.ToList();
            weather.hr_forecast = foreRoot.properties?.forecast?.Select(hourly => _.CreateHourlyForecast(hourly, foreRoot.properties?.probability_forecast))?.ToList();

            weather.condition = _.CreateCondition(currRoot);
            weather.atmosphere = _.CreateAtmosphere(currRoot);
            weather.precipitation = _.CreatePrecipitation(currRoot);

            weather.ttl = 180;

            // Observation only gives temp & wind
            if (weather.hr_forecast.IsNullOrEmpty())
            {
                var firstHr = weather.hr_forecast![0];

                if (weather.atmosphere is Atmosphere atmosphere)
                {
                    atmosphere.humidity = firstHr.extras?.humidity;
                    atmosphere.pressure_mb = firstHr.extras?.pressure_mb;
                    atmosphere.pressure_in = firstHr.extras?.pressure_in;
                    atmosphere.dewpoint_c = firstHr.extras?.dewpoint_c;
                    atmosphere.dewpoint_f = firstHr.extras?.dewpoint_f;
                    atmosphere.visibility_km = firstHr.extras?.visibility_km;
                    atmosphere.visibility_mi = firstHr.extras?.visibility_mi;
                }

                if (weather.precipitation is Precipitation precipitation)
                {
                    precipitation.cloudiness = firstHr.extras?.cloudiness;
                    precipitation.pop = firstHr.extras?.pop;
                    precipitation.qpf_rain_in = firstHr.extras?.qpf_rain_in;
                    precipitation.qpf_rain_mm = firstHr.extras?.qpf_rain_mm;
                    precipitation.qpf_snow_in = firstHr.extras?.qpf_snow_in;
                    precipitation.qpf_snow_cm = firstHr.extras?.qpf_snow_cm;
                }
            }

            // Set feelslike temp
            if (!weather.condition.feelslike_f.HasValue && weather.condition.temp_f.HasValue && weather.condition.wind_mph.HasValue && weather.atmosphere.humidity.HasValue)
            {
                weather.condition.feelslike_f = WeatherUtils.GetFeelsLikeTemp(weather.condition.temp_f.Value, weather.condition.wind_mph.Value, weather.atmosphere.humidity.Value);
                weather.condition.feelslike_c = ConversionMethods.FtoC(weather.condition.feelslike_f.Value);
            }

            if ((!weather.condition.high_f.HasValue || !weather.condition.low_f.HasValue) && weather.forecast.Count > 0)
            {
                weather.condition.high_f = weather.forecast[0].high_f;
                weather.condition.high_c = weather.forecast[0].high_c;
                weather.condition.low_f = weather.forecast[0].low_f;
                weather.condition.low_c = weather.forecast[0].low_c;
            }

            weather.weather_alerts = _.CreateWeatherAlerts(alertRoot);

            weather.source = WeatherAPI.MeteoFrance;

            return weather;
        }

        public static Location CreateLocation(this MeteoFranceProvider _, MeteoFrance.ForecastRootobject foreRoot)
        {
            // Use location name from location provider
            return new Location()
            {
                name = null,
                latitude = foreRoot.geometry?.coordinates?.ElementAtOrNull(1),
                longitude = foreRoot?.geometry?.coordinates?.ElementAtOrNull(0),
                tz_long = foreRoot?.properties?.timezone,
            };
        }

        public static SimpleWeather.WeatherData.Forecast CreateForecast(this MeteoFranceProvider _, MeteoFrance.Daily_Forecast day)
        {
            var forecast = new SimpleWeather.WeatherData.Forecast();

            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.MeteoFrance);
            var culture = LocaleUtils.GetLocale();

            forecast.date = day.time!.Value.LocalDateTime;

            if (day.T_max.HasValue)
            {
                forecast.high_c = day.T_max;
                forecast.high_f = ConversionMethods.CtoF(day.T_max.Value);
            }

            if (day.T_min.HasValue)
            {
                forecast.low_c = day.T_min;
                forecast.low_f = ConversionMethods.CtoF(day.T_min.Value);
            }

            if (!day.daily_weather_description.IsNullOrEmpty() &&
                culture.TwoLetterISOLanguageName.Equals("en", StringComparison.InvariantCultureIgnoreCase) ||
                culture.TwoLetterISOLanguageName.Equals("fr", StringComparison.InvariantCultureIgnoreCase) ||
                culture.Equals(CultureInfo.InvariantCulture))
            {
                forecast.condition = day.daily_weather_description;
            }
            else
            {
                forecast.condition = provider.GetWeatherCondition(provider.GetWeatherIcon(false, day.daily_weather_icon));
            }
            forecast.icon = day.daily_weather_icon;

            forecast.extras = new ForecastExtras();
            if (day.relative_humidity_max.HasValue && day.relative_humidity_min.HasValue)
            {
                forecast.extras.humidity = ((day.relative_humidity_min.Value + day.relative_humidity_max.Value) / 2f).RoundToInt();
            }
            if (day.T_sea.HasValue)
            {
                forecast.extras.pressure_mb = day.T_sea;
                forecast.extras.pressure_in = ConversionMethods.MBToInHg(day.T_sea.Value);
            }
            if (day.total_precipitation_24h.HasValue)
            {
                forecast.extras.qpf_rain_mm = day.total_precipitation_24h;
                forecast.extras.qpf_rain_in = ConversionMethods.MMToIn(day.total_precipitation_24h.Value);
            }
            forecast.extras.uv_index = day.uv_index;

            return forecast;
        }

        public static HourlyForecast CreateHourlyForecast(this MeteoFranceProvider _, Forecast forecast, Probability_Forecast[]? probabilityForecasts)
        {
            var hrf = new HourlyForecast();

            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.MeteoFrance);
            var culture = LocaleUtils.GetLocale();

            hrf.date = forecast.time!.Value;

            if (forecast.T.HasValue)
            {
                hrf.high_c = forecast.T.Value;
                hrf.high_f = ConversionMethods.CtoF(forecast.T.Value);
            }

            if (!forecast.weather_description.IsNullOrBlank() &&
                culture.TwoLetterISOLanguageName.Equals("en", StringComparison.InvariantCultureIgnoreCase) ||
                culture.TwoLetterISOLanguageName.Equals("fr", StringComparison.InvariantCultureIgnoreCase) ||
                culture.Equals(CultureInfo.InvariantCulture))
            {
                hrf.condition = forecast.weather_description;
            }
            else
            {
                hrf.condition = provider.GetWeatherCondition(provider.GetWeatherIcon(forecast.weather_icon));
            }
            hrf.icon = forecast.weather_icon;

            // Extras
            hrf.extras = new ForecastExtras();

            if (forecast.T_windchill.HasValue)
            {
                hrf.extras.feelslike_c = forecast.T_windchill;
                hrf.extras.feelslike_f = ConversionMethods.CtoF(forecast.T_windchill.Value);
            }

            hrf.extras.humidity = forecast.relative_humidity;

            if (forecast.P_sea.HasValue)
            {
                hrf.extras.pressure_mb = forecast.P_sea;
                hrf.extras.pressure_in = ConversionMethods.MBToInHg(forecast.P_sea.Value);
            }

            if (forecast.wind_speed.HasValue)
            {
                hrf.extras.wind_kph = ConversionMethods.MSecToKph(forecast.wind_speed.Value);
                hrf.extras.wind_mph = ConversionMethods.MSecToMph(forecast.wind_speed.Value);
                hrf.extras.wind_degrees = forecast.wind_direction;
            }

            if (forecast.wind_speed_gust.HasValue)
            {
                hrf.extras.windgust_kph = ConversionMethods.MSecToKph(forecast.wind_speed_gust.Value);
                hrf.extras.windgust_mph = ConversionMethods.MSecToMph(forecast.wind_speed_gust.Value);
            }

            if (forecast.rain_1h.HasValue)
            {
                hrf.extras.qpf_rain_mm = forecast.rain_1h;
                hrf.extras.qpf_rain_in = ConversionMethods.MMToIn(forecast.rain_1h.Value);
            }
            else if (forecast.rain_3h.HasValue)
            {
                hrf.extras.qpf_rain_mm = forecast.rain_3h;
                hrf.extras.qpf_rain_in = ConversionMethods.MMToIn(forecast.rain_3h.Value);
            }
            else if (forecast.rain_6h.HasValue)
            {
                hrf.extras.qpf_rain_mm = forecast.rain_6h;
                hrf.extras.qpf_rain_in = ConversionMethods.MMToIn(forecast.rain_6h.Value);
            }

            if (forecast.snow_1h.HasValue)
            {
                hrf.extras.qpf_snow_cm = forecast.snow_1h / 10;
                hrf.extras.qpf_snow_in = ConversionMethods.MMToIn(forecast.snow_1h.Value);
            }
            else if (forecast.snow_3h.HasValue)
            {
                hrf.extras.qpf_snow_cm = forecast.snow_3h / 10;
                hrf.extras.qpf_snow_in = ConversionMethods.MMToIn(forecast.snow_3h.Value);
            }
            else if (forecast.snow_6h.HasValue)
            {
                hrf.extras.qpf_snow_cm = forecast.snow_6h / 10;
                hrf.extras.qpf_snow_in = ConversionMethods.MMToIn(forecast.snow_6h.Value);
            }

            hrf.extras.cloudiness = forecast.total_cloud_cover;

            if (hrf.high_c.HasValue && hrf.extras.humidity.HasValue)
            {
                hrf.extras.dewpoint_c = WeatherUtils.CalculateDewpointC(hrf.high_c.Value, hrf.extras.humidity.Value);
            }
            if (hrf.high_f.HasValue && hrf.extras.humidity.HasValue)
            {
                hrf.extras.dewpoint_f = WeatherUtils.CalculateDewpointF(hrf.high_f.Value, hrf.extras.humidity.Value);
            }

            if (!probabilityForecasts.IsNullOrEmpty())
            {
                // Note: probability forecasts are given either every 3 or 6 hours
                // Rain/Snow object can contain forecast for either next 3 or 6 hrs, or both
                var dt = hrf.date.Trim(TimeSpan.TicksPerHour);
                var found3hrForecast = false;
                var _3hrForecastNA = false;

                foreach (var prob in probabilityForecasts!)
                {
                    var probDt = prob.time!.Value.Trim(TimeSpan.TicksPerHour);

                    // Check if timestamp is within 3-hr forecast
                    if (dt == probDt || dt == probDt.AddHours(1) || dt == probDt.AddHours(2))
                    {
                        if (prob.rain_hazard_3h.HasValue)
                        {
                            hrf.extras.pop = (int)prob.rain_hazard_3h.Value;
                            found3hrForecast = true;
                            _3hrForecastNA = false;
                        }
                        else
                        {
                            found3hrForecast = false;
                            _3hrForecastNA = true;
                        }
                        if (!hrf.extras.pop.HasValue && prob.rain_hazard_6h.HasValue)
                        {
                            hrf.extras.pop = (int)prob.rain_hazard_6h.Value;
                            _3hrForecastNA = true;
                            found3hrForecast = false;
                        }
                        if (!hrf.extras.pop.HasValue)
                        {
                            if (prob.snow_hazard_3h.HasValue)
                            {
                                hrf.extras.pop = (int)prob.snow_hazard_3h.Value;
                            }
                            if (prob.snow_hazard_6h.HasValue)
                            {
                                hrf.extras.pop = (int)prob.snow_hazard_6h.Value;
                            }
                        }
                    }

                    // Timestamp is not within 3-hr forecast; check 6-hr timeframe
                    // Check if timestamp is within 6-hr forecast
                    if (!hrf.extras.pop.HasValue && (dt == probDt.AddHours(3) || dt == probDt.AddHours(4) || dt == probDt.AddHours(5)))
                    {
                        if (prob.rain_hazard_6h.HasValue)
                        {
                            hrf.extras.pop = (int)prob.rain_hazard_6h.Value;
                            _3hrForecastNA = true;
                            found3hrForecast = false;
                        }
                        if (!hrf.extras.pop.HasValue)
                        {
                            if (prob.snow_hazard_6h.HasValue)
                            {
                                hrf.extras.pop = (int)prob.snow_hazard_6h.Value;
                            }
                        }
                    }

                    if (hrf.extras.pop.HasValue && (found3hrForecast || _3hrForecastNA)) break;
                }
            }

            return hrf;
        }

        public static Condition CreateCondition(this MeteoFranceProvider _, MeteoFrance.CurrentsRootobject currRoot)
        {
            var condition = new Condition();

            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.MeteoFrance);
            var culture = LocaleUtils.GetLocale();

            if (currRoot.properties?.gridded?.T.HasValue == true)
            {
                condition.temp_c = currRoot.properties.gridded.T;
                condition.temp_f = ConversionMethods.CtoF(currRoot.properties.gridded.T.Value);
            }

            if (!(currRoot.properties?.gridded?.weather_description).IsNullOrBlank() &&
                culture.TwoLetterISOLanguageName.Equals("en", StringComparison.InvariantCultureIgnoreCase) ||
                culture.TwoLetterISOLanguageName.Equals("fr", StringComparison.InvariantCultureIgnoreCase) ||
                culture.Equals(CultureInfo.InvariantCulture))
            {
                condition.weather = currRoot.properties?.gridded?.weather_description;
            }
            else
            {
                condition.weather = provider.GetWeatherCondition(provider.GetWeatherIcon(currRoot.properties?.gridded?.weather_icon));
            }
            condition.icon = currRoot.properties?.gridded?.weather_icon;

            condition.wind_degrees = currRoot.properties?.gridded?.wind_direction;
            if (currRoot.properties?.gridded?.wind_speed.HasValue == true)
            {
                condition.wind_kph = ConversionMethods.MSecToKph(currRoot.properties.gridded.wind_speed.Value);
                condition.wind_mph = ConversionMethods.MSecToMph(currRoot.properties.gridded.wind_speed.Value);
            }

            condition.observation_time = currRoot.properties?.gridded?.time ?? currRoot.update_time ?? DateTimeOffset.UtcNow;

            return condition;
        }

        public static Atmosphere CreateAtmosphere(this MeteoFranceProvider _, MeteoFrance.CurrentsRootobject currRoot)
        {
            // no-op
            return new Atmosphere();
        }

        public static Precipitation CreatePrecipitation(this MeteoFranceProvider _, MeteoFrance.CurrentsRootobject currRoot)
        {
            // no-op
            return new Precipitation();
        }
    }
}
#nullable restore