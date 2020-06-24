using SimpleWeather.Utils;
using SimpleWeather.UWP;
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
        public Weather(WeatherYahoo.Rootobject root)
        {
            location = new Location(root.location);
            update_time = ConversionMethods.ToEpochDateTime(root.current_observation.pubDate.ToString(CultureInfo.InvariantCulture));
            forecast = new List<Forecast>(root.forecasts.Length);
            for (int i = 0; i < root.forecasts.Length; i++)
            {
                forecast.Add(new Forecast(root.forecasts[i]));
            }
            condition = new Condition(root.current_observation);
            atmosphere = new Atmosphere(root.current_observation.atmosphere);
            astronomy = new Astronomy(root.current_observation.astronomy);
            ttl = "120";

            // Set feelslike temp
            if (condition.temp_f > 80)
            {
                condition.feelslike_f = (float)WeatherUtils.CalculateHeatIndex(condition.temp_f, int.Parse(atmosphere.humidity.Replace("%", "")));
                condition.feelslike_c = float.Parse(ConversionMethods.FtoC(condition.feelslike_f.ToString()));
            }

            if (condition.high_f == condition.high_c && forecast.Count > 0)
            {
                condition.high_f = float.Parse(forecast[0].high_f);
                condition.high_c = float.Parse(forecast[0].high_c);
                condition.low_f = float.Parse(forecast[0].low_f);
                condition.low_c = float.Parse(forecast[0].low_c);
            }

            condition.observation_time = update_time;

            source = WeatherAPI.Yahoo;
        }

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
            ttl = "120";

            query = string.Format("lat={0}&lon={1}", location.latitude, location.longitude);

            if (condition.high_f == condition.high_c && forecast.Count > 0)
            {
                condition.high_f = float.Parse(forecast[0].high_f);
                condition.high_c = float.Parse(forecast[0].high_c);
                condition.low_f = float.Parse(forecast[0].low_f);
                condition.low_c = float.Parse(forecast[0].low_c);
            }

            source = WeatherAPI.OpenWeatherMap;

            // Check for outdated observation
            int ttlMins = int.Parse(ttl);
            if ((DateTimeOffset.Now - condition.observation_time).TotalMinutes > ttlMins)
            {
                if (hr_forecast.FirstOrDefault() is HourlyForecast hrf)
                {
                    condition.weather = hrf.condition;
                    condition.icon = hrf.icon;

                    condition.temp_f = float.Parse(hrf.high_f, CultureInfo.InvariantCulture);
                    condition.temp_c = float.Parse(hrf.high_c, CultureInfo.InvariantCulture);

                    condition.wind_mph = hrf.wind_mph;
                    condition.wind_kph = hrf.wind_kph;
                    condition.wind_degrees = hrf.wind_degrees;

                    condition.beaufort = null;
                    condition.feelslike_f = hrf.extras?.feelslike_f ?? 0.0f;
                    condition.feelslike_c = hrf.extras?.feelslike_c ?? 0.0f;
                    condition.uv = null;

                    atmosphere.dewpoint_f = hrf.extras?.dewpoint_f;
                    atmosphere.dewpoint_c = hrf.extras?.dewpoint_c;
                    atmosphere.humidity = hrf.extras?.humidity;
                    atmosphere.pressure_trend = null;
                    atmosphere.pressure_in = hrf.extras?.pressure_in;
                    atmosphere.pressure_mb = hrf.extras?.pressure_mb;
                    atmosphere.visibility_mi = hrf.extras?.visibility_mi;
                    atmosphere.visibility_km = hrf.extras?.visibility_km;

                    precipitation.pop = hrf.extras?.pop;
                    precipitation.qpf_rain_in = hrf.extras?.qpf_rain_in > 0 ? hrf.extras.qpf_rain_in : 0.0f;
                    precipitation.qpf_rain_mm = hrf.extras?.qpf_rain_mm > 0 ? hrf.extras.qpf_rain_mm : 0.0f;
                    precipitation.qpf_snow_in = hrf.extras?.qpf_snow_in > 0 ? hrf.extras.qpf_snow_in : 0.0f;
                    precipitation.qpf_snow_cm = hrf.extras?.qpf_snow_cm > 0 ? hrf.extras.qpf_snow_cm : 0.0f;
                }
            }
        }

        public Weather(Metno.Rootobject foreRoot, Metno.AstroRootobject astroRoot)
        {
            var now = DateTimeOffset.UtcNow;

            location = new Location(foreRoot);
            update_time = now;

            // 9-day forecast / hrly -> 6hrly forecast
            forecast = new List<Forecast>(10);
            hr_forecast = new List<HourlyForecast>(foreRoot.properties.timeseries.Length);

            // Store potential min/max values
            float dayMax = float.NaN;
            float dayMin = float.NaN;

            DateTime currentDate = DateTime.MinValue;
            Forecast fcast = null;

            // Metno data is troublesome to parse thru
            for (int i = 0; i < foreRoot.properties.timeseries.Length; i++)
            {
                var time = foreRoot.properties.timeseries[i];
                DateTime date = time.time;

                // Create condition for next 2hrs from data
                if (i == 0)
                {
                    condition = new Condition(time);
                    atmosphere = new Atmosphere(time);
                    precipitation = new Precipitation(time);
                }

                // Add a new hour
                if (time.time >= now.UtcDateTime.Trim(TimeSpan.TicksPerHour))
                    hr_forecast.Add(new HourlyForecast(time));

                // Create new forecast
                if (currentDate.Date != date.Date)
                {
                    // Last forecast for day; create forecast
                    if (fcast != null)
                    {
                        // condition (set in provider GetWeather method)
                        // date
                        fcast.date = currentDate;
                        // high
                        fcast.high_f = ConversionMethods.CtoF(dayMax.ToString(CultureInfo.InvariantCulture));
                        fcast.high_c = Math.Round(dayMax).ToString();
                        // low
                        fcast.low_f = ConversionMethods.CtoF(dayMin.ToString(CultureInfo.InvariantCulture));
                        fcast.low_c = Math.Round(dayMin).ToString();

                        forecast.Add(fcast);
                    }

                    currentDate = date;
                    fcast = new Forecast(time)
                    {
                        date = date
                    };

                    // Reset
                    dayMax = float.NaN;
                    dayMin = float.NaN;
                }

                // Find max/min for each hour
                float temp = time.data.instant.details.air_temperature ?? float.NaN;
                if (!float.IsNaN(temp) && (float.IsNaN(dayMax) || temp > dayMax))
                {
                    dayMax = temp;
                }
                if (!float.IsNaN(temp) && (float.IsNaN(dayMin) || temp < dayMin))
                {
                    dayMin = temp;
                }
            }

            fcast = forecast.LastOrDefault();
            if (fcast?.condition == null && fcast?.icon == null)
            {
                forecast.RemoveAt(forecast.Count - 1);
            }

            if (hr_forecast.LastOrDefault() is HourlyForecast hrfcast &&
                hrfcast?.condition == null && hrfcast?.icon == null)
            {
                hr_forecast.RemoveAt(hr_forecast.Count - 1);
            }

            astronomy = new Astronomy(astroRoot);
            ttl = "120";

            query = string.Format("lat={0}&lon={1}", location.latitude, location.longitude);

            // Set feelslike temp
            condition.feelslike_f = float.Parse(WeatherUtils.GetFeelsLikeTemp(condition.temp_f.ToString(), condition.wind_mph.ToString(), atmosphere.humidity));
            condition.feelslike_c = float.Parse(ConversionMethods.FtoC(condition.feelslike_f.ToString()));

            if (condition.high_f == condition.high_c && forecast.Count > 0)
            {
                condition.high_f = float.Parse(forecast[0].high_f);
                condition.high_c = float.Parse(forecast[0].high_c);
                condition.low_f = float.Parse(forecast[0].low_f);
                condition.low_c = float.Parse(forecast[0].low_c);
            }

            condition.observation_time = foreRoot.properties.meta.updated_at;

            source = WeatherAPI.MetNo;

            // Check for outdated observation
            int ttlMins = int.Parse(ttl);
            if ((DateTimeOffset.Now - condition.observation_time).TotalMinutes > ttlMins)
            {
                if (hr_forecast.FirstOrDefault() is HourlyForecast hrf)
                {
                    condition.weather = hrf.condition;
                    condition.icon = hrf.icon;

                    condition.temp_f = float.Parse(hrf.high_f, CultureInfo.InvariantCulture);
                    condition.temp_c = float.Parse(hrf.high_c, CultureInfo.InvariantCulture);

                    condition.wind_mph = hrf.wind_mph;
                    condition.wind_kph = hrf.wind_kph;
                    condition.wind_degrees = hrf.wind_degrees;

                    condition.beaufort = null;
                    condition.feelslike_f = hrf.extras?.feelslike_f ?? 0.0f;
                    condition.feelslike_c = hrf.extras?.feelslike_c ?? 0.0f;
                    condition.uv = hrf.extras != null && hrf.extras.uv_index >= 0 ? new UV(hrf.extras.uv_index) : null;

                    atmosphere.dewpoint_f = hrf.extras?.dewpoint_f;
                    atmosphere.dewpoint_c = hrf.extras?.dewpoint_c;
                    atmosphere.humidity = hrf.extras?.humidity;
                    atmosphere.pressure_trend = null;
                    atmosphere.pressure_in = hrf.extras?.pressure_in;
                    atmosphere.pressure_mb = hrf.extras?.pressure_mb;
                    atmosphere.visibility_mi = hrf.extras?.visibility_mi;
                    atmosphere.visibility_km = hrf.extras?.visibility_km;

                    precipitation.pop = hrf.extras?.pop;
                    precipitation.qpf_rain_in = hrf.extras?.qpf_rain_in > 0 ? hrf.extras.qpf_rain_in : 0.0f;
                    precipitation.qpf_rain_mm = hrf.extras?.qpf_rain_mm > 0 ? hrf.extras.qpf_rain_mm : 0.0f;
                    precipitation.qpf_snow_in = hrf.extras?.qpf_snow_in > 0 ? hrf.extras.qpf_snow_in : 0.0f;
                    precipitation.qpf_snow_cm = hrf.extras?.qpf_snow_cm > 0 ? hrf.extras.qpf_snow_cm : 0.0f;
                }
            }
        }

        public Weather(HERE.Rootobject root)
        {
            var now = root.feedCreation;

            location = new Location(root.observations.location[0]);
            update_time = now;
            forecast = new List<Forecast>(root.dailyForecasts.forecastLocation.forecast.Length);
            txt_forecast = new List<TextForecast>(root.dailyForecasts.forecastLocation.forecast.Length);
            foreach (HERE.Forecast fcast in root.dailyForecasts.forecastLocation.forecast)
            {
                forecast.Add(new Forecast(fcast));
                txt_forecast.Add(new TextForecast(fcast));
            }
            hr_forecast = new List<HourlyForecast>(root.hourlyForecasts.forecastLocation.forecast.Length);
            foreach (HERE.Forecast1 forecast1 in root.hourlyForecasts.forecastLocation.forecast)
            {
                if (forecast1.utcTime.UtcDateTime < now.UtcDateTime.Trim(TimeSpan.TicksPerHour))
                    continue;

                hr_forecast.Add(new HourlyForecast(forecast1));
            }

            var observation = root.observations.location[0].observation[0];
            var todaysForecast = root.dailyForecasts.forecastLocation.forecast[0];

            condition = new Condition(observation, todaysForecast);
            atmosphere = new Atmosphere(observation);
            astronomy = new Astronomy(root.astronomy.astronomy);
            precipitation = new Precipitation(todaysForecast);
            ttl = "180";

            source = WeatherAPI.Here;

            // Check for outdated observation
            int ttlMins = int.Parse(ttl);
            if ((DateTimeOffset.Now - condition.observation_time).TotalMinutes > ttlMins)
            {
                if (hr_forecast.FirstOrDefault() is HourlyForecast hrf)
                {
                    condition.weather = hrf.condition;
                    condition.icon = hrf.icon;

                    condition.temp_f = float.Parse(hrf.high_f, CultureInfo.InvariantCulture);
                    condition.temp_c = float.Parse(hrf.high_c, CultureInfo.InvariantCulture);

                    condition.wind_mph = hrf.wind_mph;
                    condition.wind_kph = hrf.wind_kph;
                    condition.wind_degrees = hrf.wind_degrees;

                    condition.beaufort = null;
                    condition.feelslike_f = hrf.extras?.feelslike_f ?? 0.0f;
                    condition.feelslike_c = hrf.extras?.feelslike_c ?? 0.0f;
                    condition.uv = null;

                    atmosphere.dewpoint_f = hrf.extras?.dewpoint_f;
                    atmosphere.dewpoint_c = hrf.extras?.dewpoint_c;
                    atmosphere.humidity = hrf.extras?.humidity;
                    atmosphere.pressure_trend = null;
                    atmosphere.pressure_in = hrf.extras?.pressure_in;
                    atmosphere.pressure_mb = hrf.extras?.pressure_mb;
                    atmosphere.visibility_mi = hrf.extras?.visibility_mi;
                    atmosphere.visibility_km = hrf.extras?.visibility_km;

                    precipitation.pop = hrf.extras?.pop;
                    precipitation.qpf_rain_in = hrf.extras?.qpf_rain_in > 0 ? hrf.extras.qpf_rain_in : 0.0f;
                    precipitation.qpf_rain_mm = hrf.extras?.qpf_rain_mm > 0 ? hrf.extras.qpf_rain_mm : 0.0f;
                    precipitation.qpf_snow_in = hrf.extras?.qpf_snow_in > 0 ? hrf.extras.qpf_snow_in : 0.0f;
                    precipitation.qpf_snow_cm = hrf.extras?.qpf_snow_cm > 0 ? hrf.extras.qpf_snow_cm : 0.0f;
                }
            }
        }

        public Weather(NWS.PointsRootobject pointsRootobject, NWS.ForecastRootobject forecastRootobject,
            NWS.ForecastRootobject hourlyForecastRootobject, NWS.ObservationsCurrentRootobject obsCurrentRootObject)
        {
            location = new Location(pointsRootobject);
            var now = DateTimeOffset.UtcNow;
            update_time = now;

            // ~8-day forecast
            forecast = new List<Forecast>(8);
            txt_forecast = new List<TextForecast>(16);

            for (int i = 0; i < forecastRootobject?.periods?.Length; i++)
            {
                NWS.Period forecastItem = forecastRootobject.periods[i];

                if ((forecast.Count == 0 && !forecastItem.isDaytime) ||
                    (forecast.Count == forecastRootobject.periods.Length - 1 && forecastItem.isDaytime))
                {
                    forecast.Add(new Forecast(forecastItem));
                    txt_forecast.Add(new TextForecast(forecastItem));
                }
                else if (forecastItem.isDaytime && (i + 1) < forecastRootobject.periods.Length)
                {
                    NWS.Period ntForecastItem = forecastRootobject.periods[i + 1];

                    forecast.Add(new Forecast(forecastItem, ntForecastItem));
                    txt_forecast.Add(new TextForecast(forecastItem, ntForecastItem));

                    i++;
                }
            }
            if (hourlyForecastRootobject != null)
            {
                hr_forecast = new List<HourlyForecast>(hourlyForecastRootobject.periods.Length);
                foreach (NWS.Period period in hourlyForecastRootobject.periods)
                {
                    if (period.startTime.UtcDateTime < now.UtcDateTime.Trim(TimeSpan.TicksPerHour))
                        continue;

                    hr_forecast.Add(new HourlyForecast(period));
                }
            }
            condition = new Condition(obsCurrentRootObject);
            atmosphere = new Atmosphere(obsCurrentRootObject);
            //astronomy = new Astronomy(obsCurrentRootObject);
            precipitation = new Precipitation(obsCurrentRootObject);
            ttl = "180";

            if (condition.high_f == condition.high_c && forecast.Count > 0)
            {
                if (forecast[0].high_f != null)
                    condition.high_f = float.Parse(forecast[0].high_f);
                if (forecast[0].high_c != null)
                    condition.high_c = float.Parse(forecast[0].high_c);
            }
            if (condition.low_f == condition.low_c && forecast.Count > 0)
            {
                if (forecast[0].low_f != null)
                    condition.low_f = float.Parse(forecast[0].low_f);
                if (forecast[0].low_c != null)
                    condition.low_c = float.Parse(forecast[0].low_c);
            }

            source = WeatherAPI.NWS;

            // Check for outdated observation
            int ttlMins = int.Parse(ttl);
            if ((DateTimeOffset.Now - condition.observation_time).TotalMinutes > ttlMins)
            {
                if (hr_forecast.FirstOrDefault() is HourlyForecast hrf)
                {
                    condition.weather = hrf.condition;
                    condition.icon = hrf.icon;

                    condition.temp_f = float.Parse(hrf.high_f, CultureInfo.InvariantCulture);
                    condition.temp_c = float.Parse(hrf.high_c, CultureInfo.InvariantCulture);

                    condition.wind_mph = hrf.wind_mph;
                    condition.wind_kph = hrf.wind_kph;
                    condition.wind_degrees = hrf.wind_degrees;
                }
            }
        }
    }

    public partial class Location
    {
        public Location(WeatherYahoo.Location location)
        {
            // Use location name from location provider
            name = null;
            latitude = location.lat.ToString(CultureInfo.InvariantCulture);
            longitude = location._long.ToString(CultureInfo.InvariantCulture);
            var nodaTz = NodaTime.DateTimeZoneProviders.Tzdb.GetZoneOrNull(location.timezone_id);
            if (nodaTz != null)
            {
                var Instant = NodaTime.SystemClock.Instance.GetCurrentInstant();
                tz_short = nodaTz.GetZoneInterval(Instant).Name;
                tz_offset = nodaTz.GetUtcOffset(Instant).ToTimeSpan();
                tz_long = location.timezone_id;
            }
            else
            {
                tz_offset = TimeSpan.Zero;
                tz_short = "UTC";
            }
        }

        public Location(OpenWeather.Rootobject root)
        {
            // Use location name from location provider
            name = null;
            latitude = root.lat.ToString(CultureInfo.InvariantCulture);
            longitude = root.lon.ToString(CultureInfo.InvariantCulture);
            var nodaTz = NodaTime.DateTimeZoneProviders.Tzdb.GetZoneOrNull(root.timezone);
            if (nodaTz != null)
            {
                var Instant = NodaTime.SystemClock.Instance.GetCurrentInstant();
                tz_short = nodaTz.GetZoneInterval(Instant).Name;
                tz_offset = nodaTz.GetUtcOffset(Instant).ToTimeSpan();
                tz_long = root.timezone;
            }
            else
            {
                tz_offset = TimeSpan.Zero;
                tz_short = "UTC";
            }
        }

        public Location(Metno.Rootobject foreRoot)
        {
            // API doesn't provide location name (at all)
            name = null;
            latitude = foreRoot.geometry.coordinates[1].ToString("0.####", CultureInfo.InvariantCulture);
            longitude = foreRoot.geometry.coordinates[0].ToString("0.####", CultureInfo.InvariantCulture);
            tz_offset = TimeSpan.Zero;
            tz_short = "UTC";
        }

        public Location(HERE.Location location)
        {
            // Use location name from location provider
            name = null;
            latitude = location.latitude.ToString(CultureInfo.InvariantCulture);
            longitude = location.longitude.ToString(CultureInfo.InvariantCulture);
            tz_offset = TimeSpan.Zero;
            tz_short = "UTC";
        }

        public Location(NWS.PointsRootobject pointsRootobject)
        {
            // Use location name from location provider
            name = null;
            var nodaTz = NodaTime.DateTimeZoneProviders.Tzdb.GetZoneOrNull(pointsRootobject.timeZone);
            if (nodaTz != null)
            {
                var Instant = NodaTime.SystemClock.Instance.GetCurrentInstant();
                tz_short = nodaTz.GetZoneInterval(Instant).Name;
                tz_offset = nodaTz.GetUtcOffset(Instant).ToTimeSpan();
                tz_long = pointsRootobject.timeZone;
            }
            else
            {
                tz_offset = TimeSpan.Zero;
                tz_short = "UTC";
            }
        }
    }

    public partial class Forecast
    {
        public Forecast(WeatherYahoo.Forecast forecast)
        {
            date = ConversionMethods.ToEpochDateTime(forecast.date.ToString(CultureInfo.InvariantCulture));
            high_f = forecast.high.ToString(CultureInfo.InvariantCulture);
            high_c = ConversionMethods.FtoC(high_f);
            low_f = forecast.low.ToString(CultureInfo.InvariantCulture);
            low_c = ConversionMethods.FtoC(low_f);
            condition = forecast.text;
            icon = WeatherManager.GetProvider(WeatherAPI.Yahoo)
                   .GetWeatherIcon(forecast.code.ToString(CultureInfo.InvariantCulture));
        }

        public Forecast(OpenWeather.Daily forecast)
        {
            date = DateTimeOffset.FromUnixTimeSeconds(forecast.dt).DateTime;
            high_f = ConversionMethods.KtoF(forecast.temp.max.ToString(CultureInfo.InvariantCulture));
            high_c = ConversionMethods.KtoC(forecast.temp.max.ToString(CultureInfo.InvariantCulture));
            low_f = ConversionMethods.KtoF(forecast.temp.min.ToString(CultureInfo.InvariantCulture));
            low_c = ConversionMethods.KtoC(forecast.temp.min.ToString(CultureInfo.InvariantCulture));
            condition = forecast.weather[0].description.ToUpperCase();
            icon = WeatherManager.GetProvider(WeatherAPI.OpenWeatherMap)
                   .GetWeatherIcon(forecast.weather[0].id.ToString(CultureInfo.InvariantCulture));

            // Extras
            extras = new ForecastExtras()
            {
                dewpoint_f = ConversionMethods.KtoF(forecast.dew_point.ToString(CultureInfo.InvariantCulture)),
                dewpoint_c = ConversionMethods.KtoC(forecast.dew_point.ToString(CultureInfo.InvariantCulture)),
                humidity = forecast.humidity.ToString(CultureInfo.InvariantCulture),
                pop = forecast.clouds.ToString(CultureInfo.InvariantCulture),
                // 1hPA = 1mbar
                pressure_mb = forecast.pressure.ToString(CultureInfo.InvariantCulture),
                pressure_in = ConversionMethods.MBToInHg(forecast.pressure.ToString(CultureInfo.InvariantCulture)),
                wind_degrees = forecast.wind_deg,
                wind_mph = (float)Math.Round(double.Parse(ConversionMethods.MSecToMph(forecast.wind_speed.ToString(CultureInfo.InvariantCulture)))),
                wind_kph = (float)Math.Round(double.Parse(ConversionMethods.MSecToKph(forecast.wind_speed.ToString(CultureInfo.InvariantCulture)))),
                uv_index = forecast.uvi
            };
            if (forecast.visibility.HasValue)
            {
                extras.visibility_km = forecast.visibility.Value.ToString(CultureInfo.InvariantCulture);
                extras.visibility_mi = ConversionMethods.KmToMi(extras.visibility_km);
            }
            if (forecast.rain.HasValue)
            {
                extras.qpf_rain_mm = forecast.rain.Value;
                extras.qpf_rain_in = float.Parse(ConversionMethods.MMToIn(forecast.rain.Value.ToString(CultureInfo.InvariantCulture)));
            }
            if (forecast.snow.HasValue)
            {
                extras.qpf_snow_cm = forecast.snow.Value / 10;
                extras.qpf_rain_in = float.Parse(ConversionMethods.MMToIn(forecast.snow.Value.ToString(CultureInfo.InvariantCulture)));
            }
        }

        public Forecast(Metno.Timesery time)
        {
            date = time.time;

            if (time.data.next_12_hours != null)
            {
                icon = time.data.next_12_hours.summary.symbol_code;
            }
            else if (time.data.next_6_hours != null)
            {
                icon = time.data.next_6_hours.summary.symbol_code;
            }
            else if (time.data.next_1_hours != null)
            {
                icon = time.data.next_1_hours.summary.symbol_code;
            }
            // Don't bother setting other values; they're not available yet
        }

        public Forecast(HERE.Forecast forecast)
        {
            date = forecast.utcTime.UtcDateTime;
            try
            {
                high_f = forecast.highTemperature;
                high_c = ConversionMethods.FtoC(forecast.highTemperature);
            }
            catch (FormatException)
            {
                high_f = null;
                high_c = null;
            }
            try
            {
                low_f = forecast.lowTemperature;
                low_c = ConversionMethods.FtoC(forecast.lowTemperature);
            }
            catch (FormatException)
            {
                low_f = null;
                low_c = null;
            }
            condition = forecast.description.ToPascalCase();
            icon = WeatherManager.GetProvider(WeatherAPI.Here)
                   .GetWeatherIcon(string.Format("{0}_{1}", forecast.daylight, forecast.iconName));

            // Extras
            extras = new ForecastExtras();
            if (float.TryParse(forecast.comfort, out float comfortTemp_f))
            {
                extras.feelslike_f = comfortTemp_f;
                extras.feelslike_c = float.Parse(ConversionMethods.FtoC(comfortTemp_f.ToString(CultureInfo.InvariantCulture)));
            }
            extras.humidity = forecast.humidity;
            try
            {
                extras.dewpoint_f = forecast.dewPoint;
                extras.dewpoint_c = ConversionMethods.FtoC(forecast.dewPoint);
            }
            catch (FormatException)
            {
                extras.dewpoint_f = null;
                extras.dewpoint_c = null;
            }
            extras.pop = forecast.precipitationProbability;
            if (float.TryParse(forecast.rainFall, out float rain_in))
            {
                extras.qpf_rain_in = rain_in;
                extras.qpf_rain_mm = float.Parse(ConversionMethods.InToMM(rain_in.ToString(CultureInfo.InvariantCulture)));
            }
            if (float.TryParse(forecast.snowFall, out float snow_in))
            {
                extras.qpf_snow_in = snow_in;
                extras.qpf_snow_cm = float.Parse(ConversionMethods.InToMM((snow_in / 10).ToString(CultureInfo.InvariantCulture)));
            }
            try
            {
                extras.pressure_in = forecast.barometerPressure;
                extras.pressure_mb = ConversionMethods.InHgToMB(forecast.barometerPressure);
            }
            catch (FormatException)
            {
                extras.pressure_in = null;
                extras.pressure_mb = null;
            }
            extras.wind_degrees = int.Parse(forecast.windDirection);
            if (float.TryParse(forecast.windSpeed, out float windSpeed))
            {
                extras.wind_mph = windSpeed;
                extras.wind_kph = float.Parse(ConversionMethods.MphToKph(windSpeed.ToString(CultureInfo.InvariantCulture)));
            }
            if (float.TryParse(forecast.uvIndex, out float uv_index))
            {
                extras.uv_index = uv_index;
            }
        }

        public Forecast(NWS.Period forecastItem)
        {
            date = forecastItem.startTime.DateTime;
            if (forecastItem.isDaytime)
            {
                high_f = forecastItem.temperature.ToString();
                high_c = ConversionMethods.FtoC(high_f);
            }
            else
            {
                low_f = forecastItem.temperature.ToString();
                low_c = ConversionMethods.FtoC(low_f);
            }
            condition = forecastItem.shortForecast;
            icon = WeatherManager.GetProvider(WeatherAPI.NWS)
                        .GetWeatherIcon(forecastItem.icon);
        }

        public Forecast(NWS.Period forecastItem, NWS.Period ntForecastItem)
        {
            date = forecastItem.startTime.DateTime;
            high_f = forecastItem.temperature.ToString();
            high_c = ConversionMethods.FtoC(high_f);
            low_f = ntForecastItem.temperature.ToString();
            low_c = ConversionMethods.FtoC(low_f);
            condition = forecastItem.shortForecast;
            icon = WeatherManager.GetProvider(WeatherAPI.NWS)
                        .GetWeatherIcon(forecastItem.icon);
        }
    }

    public partial class HourlyForecast
    {
        public HourlyForecast(OpenWeather.Hourly hr_forecast)
        {
            date = DateTimeOffset.FromUnixTimeSeconds(hr_forecast.dt);
            high_f = ConversionMethods.KtoF(hr_forecast.temp.ToString(CultureInfo.InvariantCulture));
            high_c = ConversionMethods.KtoC(hr_forecast.temp.ToString(CultureInfo.InvariantCulture));
            condition = hr_forecast.weather[0].description.ToUpperCase();

            // Use icon to determine if day or night
            string ico = hr_forecast.weather[0].icon;
            string dn = ico.Last().ToString();

            if (int.TryParse(dn, out int x))
                dn = String.Empty;

            icon = WeatherManager.GetProvider(WeatherAPI.OpenWeatherMap)
                   .GetWeatherIcon(hr_forecast.weather[0].id.ToString() + dn);

            // Use cloudiness value here
            pop = hr_forecast.clouds.ToString(CultureInfo.InvariantCulture);
            wind_degrees = hr_forecast.wind_deg;
            wind_mph = (float)Math.Round(double.Parse(ConversionMethods.MSecToMph(hr_forecast.wind_speed.ToString())));
            wind_kph = (float)Math.Round(double.Parse(ConversionMethods.MSecToKph(hr_forecast.wind_speed.ToString())));

            // Extras
            extras = new ForecastExtras()
            {
                feelslike_f = float.Parse(ConversionMethods.KtoF(hr_forecast.feels_like.ToString(CultureInfo.InvariantCulture))),
                feelslike_c = float.Parse(ConversionMethods.KtoC(hr_forecast.feels_like.ToString(CultureInfo.InvariantCulture))),
                dewpoint_f = ConversionMethods.KtoF(hr_forecast.dew_point.ToString(CultureInfo.InvariantCulture)),
                dewpoint_c = ConversionMethods.KtoC(hr_forecast.dew_point.ToString(CultureInfo.InvariantCulture)),
                humidity = hr_forecast.humidity.ToString(CultureInfo.InvariantCulture),
                pop = hr_forecast.clouds.ToString(CultureInfo.InvariantCulture),
                // 1hPA = 1mbar
                pressure_mb = hr_forecast.pressure.ToString(CultureInfo.InvariantCulture),
                pressure_in = ConversionMethods.MBToInHg(hr_forecast.pressure.ToString(CultureInfo.InvariantCulture)),
                wind_degrees = this.wind_degrees,
                wind_mph = this.wind_mph,
                wind_kph = this.wind_kph
            };
            if (hr_forecast.visibility.HasValue)
            {
                extras.visibility_km = hr_forecast.visibility.Value.ToString(CultureInfo.InvariantCulture);
                extras.visibility_mi = ConversionMethods.KmToMi(extras.visibility_km);
            }
            if (hr_forecast.rain != null)
            {
                extras.qpf_rain_mm = hr_forecast.rain._1h;
                extras.qpf_rain_in = float.Parse(ConversionMethods.MMToIn(hr_forecast.rain._1h.ToString(CultureInfo.InvariantCulture)));
            }
            if (hr_forecast.snow != null)
            {
                extras.qpf_snow_cm = hr_forecast.snow._1h / 10;
                extras.qpf_rain_in = float.Parse(ConversionMethods.MMToIn(hr_forecast.snow._1h.ToString(CultureInfo.InvariantCulture)));
            }
        }

        public HourlyForecast(Metno.Timesery hr_forecast)
        {
            date = new DateTimeOffset(hr_forecast.time, TimeSpan.Zero);
            high_f = ConversionMethods.CtoF(hr_forecast.data.instant.details.air_temperature.Value.ToString(CultureInfo.InvariantCulture));
            high_c = hr_forecast.data.instant.details.air_temperature.Value.ToString();
            // Use cloudiness value here
            pop = ((int)Math.Round(hr_forecast.data.instant.details.cloud_area_fraction.Value)).ToString();
            wind_degrees = (int)Math.Round(hr_forecast.data.instant.details.wind_from_direction.Value);
            wind_mph = (float)Math.Round(double.Parse(ConversionMethods.MSecToMph(hr_forecast.data.instant.details.wind_speed.Value.ToString())));
            wind_kph = (float)Math.Round(double.Parse(ConversionMethods.MSecToKph(hr_forecast.data.instant.details.wind_speed.Value.ToString())));

            if (hr_forecast.data.next_1_hours != null)
            {
                icon = hr_forecast.data.next_1_hours.summary.symbol_code;
            }
            else if (hr_forecast.data.next_6_hours != null)
            {
                icon = hr_forecast.data.next_6_hours.summary.symbol_code;
            }
            else if (hr_forecast.data.next_12_hours != null)
            {
                icon = hr_forecast.data.next_12_hours.summary.symbol_code;
            }

            float humidity = hr_forecast.data.instant.details.relative_humidity.Value;
            // Extras
            extras = new ForecastExtras()
            {
                feelslike_f = float.Parse(WeatherUtils.GetFeelsLikeTemp(high_f, wind_mph.ToString(CultureInfo.InvariantCulture), Math.Round(humidity).ToString(CultureInfo.InvariantCulture))),
                feelslike_c = float.Parse(ConversionMethods.FtoC(WeatherUtils.GetFeelsLikeTemp(high_f, wind_mph.ToString(CultureInfo.InvariantCulture), Math.Round(humidity).ToString(CultureInfo.InvariantCulture)))),
                humidity = Math.Round(humidity).ToString(CultureInfo.InvariantCulture),
                dewpoint_f = ConversionMethods.CtoF(hr_forecast.data.instant.details.dew_point_temperature.Value.ToString(CultureInfo.InvariantCulture)),
                dewpoint_c = hr_forecast.data.instant.details.dew_point_temperature.Value.ToString(CultureInfo.InvariantCulture),
                pop = pop,
                pressure_in = ConversionMethods.MBToInHg(hr_forecast.data.instant.details.air_pressure_at_sea_level.Value.ToString(CultureInfo.InvariantCulture)),
                pressure_mb = hr_forecast.data.instant.details.air_pressure_at_sea_level.Value.ToString(CultureInfo.InvariantCulture),
                wind_degrees = wind_degrees,
                wind_mph = wind_mph,
                wind_kph = wind_kph
            };
            if (hr_forecast.data.instant.details.fog_area_fraction.HasValue)
            {
                float visMi = 10.0f;
                extras.visibility_mi = (visMi - (visMi * hr_forecast.data.instant.details.fog_area_fraction.Value / 100)).ToString(CultureInfo.InvariantCulture);
                extras.visibility_km = ConversionMethods.MiToKm(extras.visibility_mi);
            }
            if (hr_forecast.data.instant.details.ultraviolet_index_clear_sky.HasValue)
            {
                extras.uv_index = hr_forecast.data.instant.details.ultraviolet_index_clear_sky.Value;
            }
        }

        public HourlyForecast(HERE.Forecast1 hr_forecast)
        {
            date = hr_forecast.utcTime;
            try
            {
                high_f = hr_forecast.temperature;
                high_c = ConversionMethods.FtoC(hr_forecast.temperature);
            }
            catch (FormatException)
            {
                high_f = null;
                high_c = null;
            }
            condition = hr_forecast.description.ToPascalCase();

            icon = WeatherManager.GetProvider(WeatherAPI.Here)
                   .GetWeatherIcon(string.Format("{0}_{1}", hr_forecast.daylight, hr_forecast.iconName));

            pop = hr_forecast.precipitationProbability;
            if (int.TryParse(hr_forecast.windDirection, out int windDeg))
                wind_degrees = windDeg;
            if (float.TryParse(hr_forecast.windSpeed, out float windSpeed))
            {
                wind_mph = windSpeed;
                wind_kph = float.Parse(ConversionMethods.MphToKph(windSpeed.ToString(CultureInfo.InvariantCulture)));
            }

            // Extras
            extras = new ForecastExtras();
            if (float.TryParse(hr_forecast.comfort, out float comfortTemp_f))
            {
                extras.feelslike_f = comfortTemp_f;
                extras.feelslike_c = float.Parse(ConversionMethods.FtoC(comfortTemp_f.ToString(CultureInfo.InvariantCulture)));
            }
            extras.humidity = hr_forecast.humidity;
            try
            {
                extras.dewpoint_f = hr_forecast.dewPoint;
                extras.dewpoint_c = ConversionMethods.FtoC(hr_forecast.dewPoint);
            }
            catch (FormatException)
            {
                extras.dewpoint_f = null;
                extras.dewpoint_c = null;
            }
            extras.pop = hr_forecast.precipitationProbability;
            if (float.TryParse(hr_forecast.rainFall, out float rain_in))
            {
                extras.qpf_rain_in = rain_in;
                extras.qpf_rain_mm = float.Parse(ConversionMethods.InToMM(rain_in.ToString(CultureInfo.InvariantCulture)));
            }
            if (float.TryParse(hr_forecast.snowFall, out float snow_in))
            {
                extras.qpf_snow_in = snow_in;
                extras.qpf_snow_cm = float.Parse(ConversionMethods.InToMM((snow_in / 10).ToString(CultureInfo.InvariantCulture)));
            }
            //extras.pressure_in = hr_forecast.barometerPressure;
            //extras.pressure_mb = ConversionMethods.InHgToMB(hr_forecast.barometerPressure);
            extras.wind_degrees = wind_degrees;
            extras.wind_mph = wind_mph;
            extras.wind_kph = wind_kph;
        }

        public HourlyForecast(NWS.Period forecastItem)
        {
            date = forecastItem.startTime;
            high_f = forecastItem.temperature.ToString();
            high_c = ConversionMethods.FtoC(high_f);
            condition = forecastItem.shortForecast;
            icon = WeatherManager.GetProvider(WeatherAPI.NWS)
                        .GetWeatherIcon(forecastItem.icon);
            if (float.TryParse(forecastItem.windSpeed.RemoveNonDigitChars(), out float windSpeed))
            {
                wind_mph = windSpeed;
                wind_kph = float.Parse(ConversionMethods.MphToKph(windSpeed.ToString(CultureInfo.InvariantCulture)));
            }
            pop = null;
            wind_degrees = WeatherUtils.GetWindDirection(forecastItem.windDirection);

            // Extras
            extras = new ForecastExtras();
            extras.wind_degrees = wind_degrees;
            extras.wind_mph = wind_mph;
            extras.wind_kph = wind_kph;
        }
    }

    public partial class TextForecast
    {
        public TextForecast(OpenWeather.Daily forecast)
        {
            date = DateTimeOffset.FromUnixTimeSeconds(forecast.dt).DateTime;

            var sb = new StringBuilder();
            sb.AppendFormat(CultureInfo.InvariantCulture,
                "{0} - {1}: {2}°; {3}: {4}°", SimpleLibrary.ResLoader.GetString("Label_Morning"),
                SimpleLibrary.ResLoader.GetString("Temp_Label"),
                ConversionMethods.KtoF(forecast.temp.morn.ToString(CultureInfo.InvariantCulture)),
                SimpleLibrary.ResLoader.GetString("FeelsLike_Label"),
                ConversionMethods.KtoF(forecast.feels_like.morn.ToString(CultureInfo.InvariantCulture)));
            sb.AppendLine();
            sb.AppendFormat(CultureInfo.InvariantCulture,
                "{0} - {1}: {2}°; {3}: {4}°", SimpleLibrary.ResLoader.GetString("Label_Day"),
                SimpleLibrary.ResLoader.GetString("Temp_Label"),
                ConversionMethods.KtoF(forecast.temp.day.ToString(CultureInfo.InvariantCulture)),
                SimpleLibrary.ResLoader.GetString("FeelsLike_Label"),
                ConversionMethods.KtoF(forecast.feels_like.day.ToString(CultureInfo.InvariantCulture)));
            sb.AppendLine();
            sb.AppendFormat(CultureInfo.InvariantCulture,
                "{0} - {1}: {2}°; {3}: {4}°", SimpleLibrary.ResLoader.GetString("Label_Eve"),
                SimpleLibrary.ResLoader.GetString("Temp_Label"),
                ConversionMethods.KtoF(forecast.temp.eve.ToString(CultureInfo.InvariantCulture)),
                SimpleLibrary.ResLoader.GetString("FeelsLike_Label"),
                ConversionMethods.KtoF(forecast.feels_like.eve.ToString(CultureInfo.InvariantCulture)));
            sb.AppendLine();
            sb.AppendFormat(CultureInfo.InvariantCulture,
                "{0} - {1}: {2}°; {3}: {4}°", SimpleLibrary.ResLoader.GetString("Label_Night"),
                SimpleLibrary.ResLoader.GetString("Temp_Label"),
                ConversionMethods.KtoF(forecast.temp.night.ToString(CultureInfo.InvariantCulture)),
                SimpleLibrary.ResLoader.GetString("FeelsLike_Label"),
                ConversionMethods.KtoF(forecast.feels_like.night.ToString(CultureInfo.InvariantCulture)));

            fcttext = sb.ToString();

            var sb_metric = new StringBuilder();
            sb_metric.AppendFormat(CultureInfo.InvariantCulture,
                "{0} - {1}: {2}°; {3}: {4}°", SimpleLibrary.ResLoader.GetString("Label_Morning"),
                SimpleLibrary.ResLoader.GetString("Temp_Label"),
                ConversionMethods.KtoC(forecast.temp.morn.ToString(CultureInfo.InvariantCulture)),
                SimpleLibrary.ResLoader.GetString("FeelsLike_Label"),
                ConversionMethods.KtoC(forecast.feels_like.morn.ToString(CultureInfo.InvariantCulture)));
            sb_metric.AppendLine();
            sb_metric.AppendFormat(CultureInfo.InvariantCulture,
                "{0} - {1}: {2}°; {3}: {4}°", SimpleLibrary.ResLoader.GetString("Label_Day"),
                SimpleLibrary.ResLoader.GetString("Temp_Label"),
                ConversionMethods.KtoC(forecast.temp.day.ToString(CultureInfo.InvariantCulture)),
                SimpleLibrary.ResLoader.GetString("FeelsLike_Label"),
                ConversionMethods.KtoC(forecast.feels_like.day.ToString(CultureInfo.InvariantCulture)));
            sb_metric.AppendLine();
            sb_metric.AppendFormat(CultureInfo.InvariantCulture,
                "{0} - {1}: {2}°; {3}: {4}°", SimpleLibrary.ResLoader.GetString("Label_Eve"),
                SimpleLibrary.ResLoader.GetString("Temp_Label"),
                ConversionMethods.KtoC(forecast.temp.eve.ToString(CultureInfo.InvariantCulture)),
                SimpleLibrary.ResLoader.GetString("FeelsLike_Label"),
                ConversionMethods.KtoC(forecast.feels_like.eve.ToString(CultureInfo.InvariantCulture)));
            sb_metric.AppendLine();
            sb_metric.AppendFormat(CultureInfo.InvariantCulture,
                "{0} - {1}: {2}°; {3}: {4}°", SimpleLibrary.ResLoader.GetString("Label_Night"),
                SimpleLibrary.ResLoader.GetString("Temp_Label"),
                ConversionMethods.KtoC(forecast.temp.night.ToString(CultureInfo.InvariantCulture)),
                SimpleLibrary.ResLoader.GetString("FeelsLike_Label"),
                ConversionMethods.KtoC(forecast.feels_like.night.ToString(CultureInfo.InvariantCulture)));

            fcttext_metric = sb_metric.ToString();
        }

        public TextForecast(HERE.Forecast forecast)
        {
            date = forecast.utcTime;
            fcttext = String.Format(CultureInfo.InvariantCulture, "{0} - {1} {2}",
                forecast.weekday,
                forecast.description.ToPascalCase(), forecast.beaufortDescription.ToPascalCase());
            fcttext_metric = fcttext;
        }

        public TextForecast(NWS.Period forecastItem)
        {
            date = forecastItem.startTime;
            fcttext = String.Format(CultureInfo.InvariantCulture,
                "{0} - {1}", forecastItem.name, forecastItem.detailedForecast);
            fcttext_metric = fcttext;
        }

        public TextForecast(NWS.Period forecastItem, NWS.Period ntForecastItem)
        {
            date = forecastItem.startTime;
            fcttext = String.Format(CultureInfo.InvariantCulture,
                "{0} - {1}\n\n{2} - {3}",
                forecastItem.name, forecastItem.detailedForecast,
                ntForecastItem.name, ntForecastItem.detailedForecast);
            fcttext_metric = fcttext;
        }
    }

    public partial class Condition
    {
        public Condition(WeatherYahoo.Current_Observation observation)
        {
            weather = observation.condition.text;
            temp_f = observation.condition.temperature;
            temp_c = float.Parse(ConversionMethods.FtoC(observation.condition.temperature.ToString(CultureInfo.InvariantCulture)));
            wind_degrees = observation.wind.direction;
            wind_mph = observation.wind.speed;
            wind_kph = float.Parse(ConversionMethods.MphToKph(observation.wind.speed.ToString(CultureInfo.InvariantCulture)));
            feelslike_f = observation.wind.chill;
            feelslike_c = float.Parse(ConversionMethods.FtoC(observation.wind.chill.ToString(CultureInfo.InvariantCulture)));
            icon = WeatherManager.GetProvider(WeatherAPI.Yahoo)
                   .GetWeatherIcon(observation.condition.code.ToString(CultureInfo.InvariantCulture));
        }

        public Condition(OpenWeather.Current current)
        {
            weather = current.weather[0].description.ToUpperCase();
            temp_f = float.Parse(ConversionMethods.KtoF(current.temp.ToString(CultureInfo.InvariantCulture)));
            temp_c = float.Parse(ConversionMethods.KtoC(current.temp.ToString(CultureInfo.InvariantCulture)));
            wind_degrees = current.wind_deg;
            wind_mph = float.Parse(ConversionMethods.MSecToMph(current.wind_speed.ToString(CultureInfo.InvariantCulture)));
            wind_kph = float.Parse(ConversionMethods.MSecToKph(current.wind_speed.ToString(CultureInfo.InvariantCulture)));
            feelslike_f = float.Parse(ConversionMethods.KtoF(current.feels_like.ToString(CultureInfo.InvariantCulture)));
            feelslike_c = float.Parse(ConversionMethods.KtoC(current.feels_like.ToString(CultureInfo.InvariantCulture)));

            string ico = current.weather[0].icon;
            string dn = ico.Last().ToString();

            if (int.TryParse(dn, out int x))
                dn = String.Empty;

            icon = WeatherManager.GetProvider(WeatherAPI.OpenWeatherMap)
                   .GetWeatherIcon(current.weather[0].id.ToString() + dn);

            uv = new UV(current.uvi);

            observation_time = DateTimeOffset.FromUnixTimeSeconds(current.dt);
        }

        public Condition(Metno.Timesery time)
        {
            // weather
            temp_f = float.Parse(ConversionMethods.CtoF(time.data.instant.details.air_temperature.Value.ToString()));
            temp_c = (float)time.data.instant.details.air_temperature.Value;
            wind_degrees = (int)Math.Round(time.data.instant.details.wind_from_direction.Value);
            wind_mph = (float)Math.Round(double.Parse(ConversionMethods.MSecToMph(time.data.instant.details.wind_speed.Value.ToString())));
            wind_kph = (float)Math.Round(double.Parse(ConversionMethods.MSecToKph(time.data.instant.details.wind_speed.Value.ToString())));
            // This will be calculated after with formula
            feelslike_f = temp_f;
            feelslike_c = temp_c;

            if (time.data.next_12_hours != null)
            {
                icon = time.data.next_12_hours.summary.symbol_code;
            }
            else if (time.data.next_6_hours != null)
            {
                icon = time.data.next_6_hours.summary.symbol_code;
            }
            else if (time.data.next_1_hours != null)
            {
                icon = time.data.next_1_hours.summary.symbol_code;
            }
        }

        public Condition(HERE.Observation observation, HERE.Forecast forecastItem)
        {
            weather = observation.description.ToPascalCase();
            if (float.TryParse(observation.temperature, out float tempF))
            {
                temp_f = tempF;
                temp_c = float.Parse(ConversionMethods.FtoC(tempF.ToString()));
            }
            else
            {
                temp_f = 0.00f;
                temp_c = 0.00f;
            }

            if (float.TryParse(observation.highTemperature, out float hiTempF) &&
                float.TryParse(observation.lowTemperature, out float loTempF))
            {
                high_f = hiTempF;
                high_c = float.Parse(ConversionMethods.FtoC(hiTempF.ToString()));
                low_f = loTempF;
                low_c = float.Parse(ConversionMethods.FtoC(loTempF.ToString()));
            }
            else if (float.TryParse(forecastItem.highTemperature, out hiTempF) &&
                float.TryParse(forecastItem.lowTemperature, out loTempF))
            {
                high_f = hiTempF;
                high_c = float.Parse(ConversionMethods.FtoC(hiTempF.ToString()));
                low_f = loTempF;
                low_c = float.Parse(ConversionMethods.FtoC(loTempF.ToString()));
            }
            else
            {
                high_f = 0.00f;
                high_c = 0.00f;
                low_f = 0.00f;
                low_c = 0.00f;
            }

            if (int.TryParse(observation.windDirection, out int windDegrees))
                wind_degrees = windDegrees;
            else
                wind_degrees = 0;

            if (float.TryParse(observation.windSpeed, out float wind_Speed))
            {
                wind_mph = wind_Speed;
                wind_kph = float.Parse(ConversionMethods.MphToKph(observation.windSpeed));
            }
            else
            {
                wind_mph = 0.00f;
                wind_kph = 0.00f;
            }

            if (float.TryParse(observation.comfort, out float comfortTempF))
            {
                feelslike_f = comfortTempF;
                feelslike_c = float.Parse(ConversionMethods.FtoC(comfortTempF.ToString()));
            }
            else
            {
                feelslike_f = 0.00f;
                feelslike_c = 0.00f;
            }
            icon = WeatherManager.GetProvider(WeatherAPI.Here)
                   .GetWeatherIcon(string.Format("{0}_{1}", observation.daylight, observation.iconName));

            if (int.TryParse(forecastItem.beaufortScale, out int scale))
                beaufort = new Beaufort(scale, forecastItem.beaufortDescription);

            if (float.TryParse(forecastItem.uvIndex, out float index))
                uv = new UV(index, forecastItem.uvDesc);

            observation_time = observation.utcTime;
        }

        public Condition(NWS.ObservationsCurrentRootobject obsCurrentRootObject)
        {
            weather = obsCurrentRootObject.textDescription;
            if (obsCurrentRootObject.temperature.value.HasValue)
            {
                temp_c = obsCurrentRootObject.temperature.value.GetValueOrDefault(0.00f);
                temp_f = float.Parse(ConversionMethods.CtoF(temp_c.ToString()));
            }
            else
            {
                temp_c = 0.00f;
                temp_f = 0.00f;
            }

            if (obsCurrentRootObject.windDirection.value.HasValue)
            {
                wind_degrees = (int)obsCurrentRootObject.windDirection.value.GetValueOrDefault(0);
            }
            else
            {
                wind_degrees = 0;
            }

            if (obsCurrentRootObject.windSpeed.value.HasValue)
            {
                wind_kph = obsCurrentRootObject.windSpeed.value.GetValueOrDefault(0.00f);
                wind_mph = float.Parse(ConversionMethods.KphToMph(wind_kph.ToString()));
            }
            else
            {
                wind_mph = -1.00f;
                wind_kph = -1.00f;
            }

            if (obsCurrentRootObject.heatIndex.value.HasValue)
            {
                feelslike_c = obsCurrentRootObject.heatIndex.value.GetValueOrDefault(0.00f);
                feelslike_f = float.Parse(ConversionMethods.CtoF(feelslike_c.ToString()));
            }
            else if (obsCurrentRootObject.windChill.value.HasValue)
            {
                feelslike_c = obsCurrentRootObject.windChill.value.GetValueOrDefault(0.00f);
                feelslike_f = float.Parse(ConversionMethods.CtoF(feelslike_c.ToString()));
            }
            else if (temp_f != temp_c)
            {
                float humidity = obsCurrentRootObject.relativeHumidity.value.GetValueOrDefault(-1.0f);
                feelslike_f = float.Parse(WeatherUtils.GetFeelsLikeTemp(temp_f.ToString(),
                    wind_mph.ToString(), humidity.ToString()));
                feelslike_c = float.Parse(ConversionMethods.FtoC(feelslike_f.ToString()));
            }
            else
            {
                feelslike_f = -1.00f;
                feelslike_c = -1.00f;
            }
            icon = WeatherManager.GetProvider(WeatherAPI.NWS)
                        .GetWeatherIcon(obsCurrentRootObject.icon);

            observation_time = obsCurrentRootObject.timestamp;
        }
    }

    public partial class Atmosphere
    {
        public Atmosphere(WeatherYahoo.Atmosphere atmosphere)
        {
            humidity = atmosphere.humidity;
            pressure_in = atmosphere.pressure.ToString(CultureInfo.InvariantCulture);
            pressure_mb = ConversionMethods.InHgToMB(pressure_in);
            pressure_trend = atmosphere.rising.ToString(CultureInfo.InvariantCulture);
            visibility_mi = atmosphere.visibility.ToString(CultureInfo.InvariantCulture);
            visibility_km = ConversionMethods.MiToKm(visibility_mi);
        }

        public Atmosphere(OpenWeather.Current current)
        {
            humidity = current.humidity.ToString(CultureInfo.InvariantCulture);
            // 1hPa = 1mbar
            pressure_mb = current.pressure.ToString(CultureInfo.InvariantCulture);
            pressure_in = ConversionMethods.MBToInHg(pressure_mb);
            pressure_trend = String.Empty;
            visibility_km = (current.visibility / 1000).ToString(CultureInfo.InvariantCulture);
            visibility_mi = ConversionMethods.KmToMi(visibility_km);
            dewpoint_f = ConversionMethods.KtoF(current.dew_point.ToString(CultureInfo.InvariantCulture));
            dewpoint_c = ConversionMethods.KtoC(current.dew_point.ToString(CultureInfo.InvariantCulture));
        }

        public Atmosphere(Metno.Timesery time)
        {
            humidity = Math.Round(time.data.instant.details.relative_humidity.Value).ToString();
            pressure_mb = time.data.instant.details.air_pressure_at_sea_level.Value.ToString(CultureInfo.InvariantCulture);
            pressure_in = ConversionMethods.MBToInHg(time.data.instant.details.air_pressure_at_sea_level.Value.ToString(CultureInfo.InvariantCulture));
            pressure_trend = String.Empty;

            if (time.data.instant.details.fog_area_fraction.HasValue)
            {
                float visMi = 10.0f;
                visibility_mi = (visMi - (visMi * time.data.instant.details.fog_area_fraction.Value / 100)).ToString(CultureInfo.InvariantCulture);
                visibility_km = ConversionMethods.MiToKm(visibility_mi);
            }

            if (time.data.instant.details.dew_point_temperature.HasValue)
            {
                dewpoint_f = ConversionMethods.CtoF(time.data.instant.details.dew_point_temperature.Value.ToString(CultureInfo.InvariantCulture));
                dewpoint_c = (time.data.instant.details.dew_point_temperature.Value).ToString(CultureInfo.InvariantCulture);
            }
        }

        public Atmosphere(HERE.Observation observation)
        {
            humidity = observation.humidity;
            try
            {
                pressure_mb = ConversionMethods.InHgToMB(observation.barometerPressure);
                pressure_in = observation.barometerPressure;
            }
            catch (FormatException)
            {
                pressure_mb = null;
                pressure_in = null;
            }
            pressure_trend = observation.barometerTrend;

            try
            {
                visibility_mi = observation.visibility;
                visibility_km = ConversionMethods.MiToKm(observation.visibility);
            }
            catch (FormatException)
            {
                visibility_mi = null;
                visibility_km = null;
            }

            try
            {
                dewpoint_f = observation.dewPoint;
                dewpoint_c = ConversionMethods.FtoC(observation.dewPoint);
            }
            catch (FormatException)
            {
                dewpoint_f = null;
                dewpoint_c = null;
            }
        }

        public Atmosphere(NWS.ObservationsCurrentRootobject obsCurrentRootObject)
        {
            if (obsCurrentRootObject.relativeHumidity.value.HasValue)
            {
                humidity = ((int)Math.Round(obsCurrentRootObject.relativeHumidity.value.GetValueOrDefault(0.00f))).ToString();
            }

            if (obsCurrentRootObject.barometricPressure.value.HasValue)
            {
                var pressure_pa = obsCurrentRootObject.barometricPressure.value.GetValueOrDefault(0.00f);
                pressure_in = ConversionMethods.PaToInHg(pressure_pa.ToString());
                pressure_mb = ConversionMethods.PaToMB(pressure_pa.ToString());
            }
            pressure_trend = String.Empty;

            if (obsCurrentRootObject.visibility.value.HasValue)
            {
                visibility_km = (obsCurrentRootObject.visibility.value.GetValueOrDefault(0.00f) / 1000).ToString();
                visibility_mi = ConversionMethods.KmToMi((obsCurrentRootObject.visibility.value.GetValueOrDefault(0.00f) / 1000).ToString());
            }

            if (obsCurrentRootObject.dewpoint.value.HasValue)
            {
                dewpoint_c = obsCurrentRootObject.dewpoint.value.GetValueOrDefault(0.00f).ToString();
                dewpoint_f = ConversionMethods.CtoF(obsCurrentRootObject.dewpoint.value.GetValueOrDefault(0.00f).ToString());
            }
        }
    }

    public partial class Astronomy
    {
        public Astronomy(WeatherYahoo.Astronomy astronomy)
        {
            if (DateTime.TryParse(astronomy.sunrise, out DateTime sunrise))
                this.sunrise = sunrise;
            if (DateTime.TryParse(astronomy.sunset, out DateTime sunset))
                this.sunset = sunset;

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

        public Astronomy(Metno.AstroRootobject astroRoot)
        {
            int moonPhaseValue = -1;

            foreach (Metno.Time time in astroRoot.location.time)
            {
                if (time.sunrise != null)
                {
                    sunrise = time.sunrise.time.ToUniversalTime();
                }
                if (time.sunset != null)
                {
                    sunset = time.sunset.time.ToUniversalTime();
                }

                if (time.moonrise != null)
                {
                    moonrise = time.moonrise.time.ToUniversalTime();
                }
                if (time.moonset != null)
                {
                    moonset = time.moonset.time.ToUniversalTime();
                }

                if (time.moonphase != null)
                {
                    moonPhaseValue = (int)Math.Round(double.Parse(time.moonphase.value));
                }
            }

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

            MoonPhase.MoonPhaseType moonPhaseType;
            if (moonPhaseValue >= 2 && moonPhaseValue < 23)
            {
                moonPhaseType = MoonPhase.MoonPhaseType.WaxingCrescent;
            }
            else if (moonPhaseValue >= 23 && moonPhaseValue < 26)
            {
                moonPhaseType = MoonPhase.MoonPhaseType.FirstQtr;
            }
            else if (moonPhaseValue >= 26 && moonPhaseValue < 48)
            {
                moonPhaseType = MoonPhase.MoonPhaseType.WaxingGibbous;
            }
            else if (moonPhaseValue >= 48 && moonPhaseValue < 52)
            {
                moonPhaseType = MoonPhase.MoonPhaseType.FullMoon;
            }
            else if (moonPhaseValue >= 52 && moonPhaseValue < 73)
            {
                moonPhaseType = MoonPhase.MoonPhaseType.WaningGibbous;
            }
            else if (moonPhaseValue >= 73 && moonPhaseValue < 76)
            {
                moonPhaseType = MoonPhase.MoonPhaseType.LastQtr;
            }
            else if (moonPhaseValue >= 76 && moonPhaseValue < 98)
            {
                moonPhaseType = MoonPhase.MoonPhaseType.WaningCrescent;
            }
            else
            {
                // 0, 1, 98, 99, 100
                moonPhaseType = MoonPhase.MoonPhaseType.NewMoon;
            }

            this.moonphase = new MoonPhase(moonPhaseType);
        }

        public Astronomy(HERE.Astronomy1[] astronomy)
        {
            var astroData = astronomy[0];

            if (DateTime.TryParse(astroData.sunrise, out DateTime sunrise))
                this.sunrise = sunrise;
            if (DateTime.TryParse(astroData.sunset, out DateTime sunset))
                this.sunset = sunset;
            if (DateTime.TryParse(astroData.moonrise, out DateTime moonrise))
                this.moonrise = moonrise;
            if (DateTime.TryParse(astroData.moonset, out DateTime moonset))
                this.moonset = moonset;

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

            switch (astroData.iconName)
            {
                case "cw_new_moon":
                default:
                    this.moonphase = new MoonPhase(MoonPhase.MoonPhaseType.NewMoon,
                            astroData.moonPhaseDesc);
                    break;

                case "cw_waxing_crescent":
                    this.moonphase = new MoonPhase(MoonPhase.MoonPhaseType.WaxingCrescent,
                            astroData.moonPhaseDesc);
                    break;

                case "cw_first_qtr":
                    this.moonphase = new MoonPhase(MoonPhase.MoonPhaseType.FirstQtr,
                            astroData.moonPhaseDesc);
                    break;

                case "cw_waxing_gibbous":
                    this.moonphase = new MoonPhase(MoonPhase.MoonPhaseType.WaxingGibbous,
                            astroData.moonPhaseDesc);
                    break;

                case "cw_full_moon":
                    this.moonphase = new MoonPhase(MoonPhase.MoonPhaseType.FullMoon,
                            astroData.moonPhaseDesc);
                    break;

                case "cw_waning_gibbous":
                    this.moonphase = new MoonPhase(MoonPhase.MoonPhaseType.WaningGibbous,
                            astroData.moonPhaseDesc);
                    break;

                case "cw_last_quarter":
                    this.moonphase = new MoonPhase(MoonPhase.MoonPhaseType.LastQtr,
                            astroData.moonPhaseDesc);
                    break;

                case "cw_waning_crescent":
                    this.moonphase = new MoonPhase(MoonPhase.MoonPhaseType.WaningCrescent,
                            astroData.moonPhaseDesc);
                    break;
            }
        }
    }

    public partial class Precipitation
    {
        public Precipitation(OpenWeather.Current current)
        {
            // Use cloudiness value here
            pop = current.clouds.ToString(CultureInfo.InvariantCulture);
            if (current.rain != null)
            {
                qpf_rain_in = float.Parse(ConversionMethods.MMToIn(current.rain._1h.ToString(CultureInfo.InvariantCulture)));
                qpf_rain_mm = current.rain._1h;
            }
            if (current.snow != null)
            {
                qpf_snow_in = float.Parse(ConversionMethods.MMToIn(current.snow._1h.ToString(CultureInfo.InvariantCulture)));
                qpf_snow_cm = current.snow._1h / 10;
            }
        }

        public Precipitation(Metno.Timesery time)
        {
            // Use cloudiness value here
            pop = Math.Round(time.data.instant.details.cloud_area_fraction.Value).ToString();
            // The rest DNE
        }

        public Precipitation(HERE.Forecast forecast)
        {
            pop = forecast.precipitationProbability;

            if (float.TryParse(forecast.rainFall, NumberStyles.Float, CultureInfo.InvariantCulture, out float rain_in))
            {
                qpf_rain_in = rain_in;
                qpf_rain_mm = float.Parse(ConversionMethods.InToMM(qpf_rain_in.ToString(CultureInfo.InvariantCulture)));
            }
            else
            {
                qpf_rain_in = 0.00f;
                qpf_rain_mm = 0.00f;
            }

            if (float.TryParse(forecast.snowFall, NumberStyles.Float, CultureInfo.InvariantCulture, out float snow_in))
            {
                qpf_snow_in = snow_in;
                qpf_snow_cm = float.Parse(ConversionMethods.InToMM(qpf_snow_in.ToString(CultureInfo.InvariantCulture))) / 10;
            }
            else
            {
                qpf_snow_in = 0.00f;
                qpf_snow_cm = 0.00f;
            }
        }

        public Precipitation(NWS.ObservationsCurrentRootobject obsCurrentRootObject)
        {
            if (obsCurrentRootObject.precipitationLastHour.value.HasValue)
            {
                // "unit:m"
                qpf_rain_mm = obsCurrentRootObject.precipitationLastHour.value.GetValueOrDefault(0.00f) * 1000;
                qpf_rain_in = float.Parse(ConversionMethods.MMToIn(qpf_rain_mm.ToString(CultureInfo.InvariantCulture)));
            }
            else if (obsCurrentRootObject.precipitationLast3Hours.value.HasValue)
            {
                // "unit:m"
                qpf_rain_mm = obsCurrentRootObject.precipitationLast3Hours.value.GetValueOrDefault(0.00f) * 1000;
                qpf_rain_in = float.Parse(ConversionMethods.MMToIn(qpf_rain_mm.ToString(CultureInfo.InvariantCulture)));
            }
            // The rest DNE
        }
    }

    public partial class Beaufort
    {
        public Beaufort(int beaufortScale)
        {
            switch (beaufortScale)
            {
                case 0:
                    scale = BeaufortScale.B0;
                    desc = SimpleLibrary.ResLoader.GetString("Beaufort_0");
                    break;

                case 1:
                    scale = BeaufortScale.B1;
                    desc = SimpleLibrary.ResLoader.GetString("Beaufort_1");
                    break;

                case 2:
                    scale = BeaufortScale.B2;
                    desc = SimpleLibrary.ResLoader.GetString("Beaufort_2");
                    break;

                case 3:
                    scale = BeaufortScale.B3;
                    desc = SimpleLibrary.ResLoader.GetString("Beaufort_3");
                    break;

                case 4:
                    scale = BeaufortScale.B4;
                    desc = SimpleLibrary.ResLoader.GetString("Beaufort_4");
                    break;

                case 5:
                    scale = BeaufortScale.B5;
                    desc = SimpleLibrary.ResLoader.GetString("Beaufort_5");
                    break;

                case 6:
                    scale = BeaufortScale.B6;
                    desc = SimpleLibrary.ResLoader.GetString("Beaufort_6");
                    break;

                case 7:
                    scale = BeaufortScale.B7;
                    desc = SimpleLibrary.ResLoader.GetString("Beaufort_7");
                    break;

                case 8:
                    scale = BeaufortScale.B8;
                    desc = SimpleLibrary.ResLoader.GetString("Beaufort_8");
                    break;

                case 9:
                    scale = BeaufortScale.B9;
                    desc = SimpleLibrary.ResLoader.GetString("Beaufort_9");
                    break;

                case 10:
                    scale = BeaufortScale.B10;
                    desc = SimpleLibrary.ResLoader.GetString("Beaufort_10");
                    break;

                case 11:
                    scale = BeaufortScale.B11;
                    desc = SimpleLibrary.ResLoader.GetString("Beaufort_11");
                    break;

                case 12:
                    scale = BeaufortScale.B12;
                    desc = SimpleLibrary.ResLoader.GetString("Beaufort_12");
                    break;
            }
        }

        public Beaufort(int beaufortScale, String description)
            : this(beaufortScale)
        {
            if (!String.IsNullOrWhiteSpace(description))
                this.desc = description;
        }
    }

    public partial class MoonPhase
    {
        public MoonPhase(MoonPhaseType moonPhaseType)
        {
            this.phase = moonPhaseType;

            switch (moonPhaseType)
            {
                case MoonPhaseType.NewMoon:
                    desc = SimpleLibrary.ResLoader.GetString("MoonPhase_New");
                    break;

                case MoonPhaseType.WaxingCrescent:
                    desc = SimpleLibrary.ResLoader.GetString("MoonPhase_WaxCrescent");
                    break;

                case MoonPhaseType.FirstQtr:
                    desc = SimpleLibrary.ResLoader.GetString("MoonPhase_FirstQtr");
                    break;

                case MoonPhaseType.WaxingGibbous:
                    desc = SimpleLibrary.ResLoader.GetString("MoonPhase_WaxGibbous");
                    break;

                case MoonPhaseType.FullMoon:
                    desc = SimpleLibrary.ResLoader.GetString("MoonPhase_Full");
                    break;

                case MoonPhaseType.WaningGibbous:
                    desc = SimpleLibrary.ResLoader.GetString("MoonPhase_WanGibbous");
                    break;

                case MoonPhaseType.LastQtr:
                    desc = SimpleLibrary.ResLoader.GetString("MoonPhase_LastQtr");
                    break;

                case MoonPhaseType.WaningCrescent:
                    desc = SimpleLibrary.ResLoader.GetString("MoonPhase_WanCrescent");
                    break;
            }
        }

        public MoonPhase(MoonPhaseType moonPhaseType, String description)
            : this(moonPhaseType)
        {
            if (!String.IsNullOrWhiteSpace(description))
                this.desc = description;
        }
    }

    public partial class UV
    {
        public UV(float index)
        {
            this.index = index;

            if (index >= 0 && index < 3)
            {
                desc = SimpleLibrary.ResLoader.GetString("UV_0");
            }
            else if (index >= 3 && index < 6)
            {
                desc = SimpleLibrary.ResLoader.GetString("UV_3");
            }
            else if (index >= 6 && index < 8)
            {
                desc = SimpleLibrary.ResLoader.GetString("UV_6");
            }
            else if (index >= 8 && index < 11)
            {
                desc = SimpleLibrary.ResLoader.GetString("UV_8");
            }
            else if (index >= 11)
            {
                desc = SimpleLibrary.ResLoader.GetString("UV_11");
            }
        }

        public UV(float index, String description)
            : this(index)
        {
            if (!String.IsNullOrWhiteSpace(description))
                this.desc = description;
        }
    }

    public partial class AirQuality
    {
        public AirQuality(AQICN.Rootobject root)
        {
            this.index = root.data.aqi;
        }
    }
}