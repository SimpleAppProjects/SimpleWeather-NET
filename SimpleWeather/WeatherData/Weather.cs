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
            update_time = ConversionMethods.ToEpochDateTime(root.current_observation.pubDate);
            forecast = new List<Forecast>(root.forecasts.Length);
            for (int i = 0; i < root.forecasts.Length; i++)
            {
                forecast.Add(new Forecast(root.forecasts[i]));
            }
            condition = new Condition(root.current_observation);
            atmosphere = new Atmosphere(root.current_observation.atmosphere);
            astronomy = new Astronomy(root.current_observation.astronomy);
            ttl = 120;

            // Set feelslike temp
            if (condition.temp_f.HasValue && condition.temp_f > 80 && atmosphere.humidity.HasValue)
            {
                condition.feelslike_f = WeatherUtils.CalculateHeatIndex(condition.temp_f.Value, atmosphere.humidity.Value);
                condition.feelslike_c = ConversionMethods.FtoC(condition.feelslike_f.Value);
            }

            if ((!condition.high_f.HasValue || !condition.low_f.HasValue) && forecast.Count > 0)
            {
                condition.high_f = forecast[0].high_f;
                condition.high_c = forecast[0].high_c;
                condition.low_f = forecast[0].low_f;
                condition.low_c = forecast[0].low_c;
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
            ttl = 120;

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
                if (currentDate.Date != date.Date && date >= currentDate.AddDays(1))
                {
                    // Last forecast for day; create forecast
                    if (fcast != null)
                    {
                        // condition (set in provider GetWeather method)
                        // date
                        fcast.date = currentDate;
                        // high
                        fcast.high_f = ConversionMethods.CtoF(dayMax);
                        fcast.high_c = (float)Math.Round(dayMax);
                        // low
                        fcast.low_f = ConversionMethods.CtoF(dayMin);
                        fcast.low_c = (float)Math.Round(dayMin);

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
            ttl = 120;

            query = string.Format(CultureInfo.InvariantCulture, "lat={0:0.####}&lon={1:0.####}", location.latitude, location.longitude);

            if ((!condition.high_f.HasValue || !condition.low_f.HasValue) && forecast.Count > 0)
            {
                condition.high_f = forecast[0].high_f.Value;
                condition.high_c = forecast[0].high_c.Value;
                condition.low_f = forecast[0].low_f.Value;
                condition.low_c = forecast[0].low_c.Value;
            }

            condition.observation_time = foreRoot.properties.meta.updated_at;

            source = WeatherAPI.MetNo;
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
            ttl = 180;

            source = WeatherAPI.Here;
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
            ttl = 180;

            if (!condition.high_f.HasValue && forecast.Count > 0)
            {
                condition.high_f = forecast[0].high_f;
                condition.high_c = forecast[0].high_c;
            }
            if (!condition.low_f.HasValue && forecast.Count > 0)
            {
                condition.low_f = forecast[0].low_f;
                condition.low_c = forecast[0].low_c;
            }

            source = WeatherAPI.NWS;
        }
    }

    public partial class Location
    {
        public Location(WeatherYahoo.Location location)
        {
            // Use location name from location provider
            name = null;
            latitude = (float)location.lat;
            longitude = (float)location._long;
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
            latitude = root.lat;
            longitude = root.lon;
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
            latitude = foreRoot.geometry.coordinates[1];
            longitude = foreRoot.geometry.coordinates[0];
            tz_offset = TimeSpan.Zero;
            tz_short = "UTC";
        }

        public Location(HERE.Location location)
        {
            // Use location name from location provider
            name = null;
            latitude = location.latitude;
            longitude = location.longitude;
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
            var provider = WeatherManager.GetProvider(WeatherAPI.Yahoo);
            var culture = CultureUtils.UserCulture;

            if (culture.TwoLetterISOLanguageName.Equals("en", StringComparison.InvariantCultureIgnoreCase) || culture.Equals(CultureInfo.InvariantCulture))
            {
                condition = forecast.text;
            }
            else
            {
                condition = provider.GetWeatherCondition(forecast.code.ToInvariantString());
            }
            icon = provider.GetWeatherIcon(forecast.code.ToInvariantString());

            date = ConversionMethods.ToEpochDateTime(forecast.date);
            high_f = forecast.high;
            high_c = ConversionMethods.FtoC(forecast.high);
            low_f = forecast.low;
            low_c = ConversionMethods.FtoC(forecast.low);
        }

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
            if (float.TryParse(forecast.highTemperature, NumberStyles.Float, CultureInfo.InvariantCulture, out float highF))
            {
                high_f = highF;
                high_c = ConversionMethods.FtoC(highF);
            }
            if (float.TryParse(forecast.lowTemperature, NumberStyles.Float, CultureInfo.InvariantCulture, out float lowF))
            {
                low_f = lowF;
                low_c = ConversionMethods.FtoC(lowF);
            }
            condition = forecast.description.ToPascalCase();
            icon = WeatherManager.GetProvider(WeatherAPI.Here)
                   .GetWeatherIcon(string.Format(CultureInfo.InvariantCulture, "{0}_{1}", forecast.daylight, forecast.iconName));

            // Extras
            extras = new ForecastExtras();
            if (float.TryParse(forecast.comfort, NumberStyles.Float, CultureInfo.InvariantCulture, out float comfortTempF))
            {
                extras.feelslike_f = comfortTempF;
                extras.feelslike_c = ConversionMethods.FtoC(comfortTempF);
            }
            if (int.TryParse(forecast.humidity, NumberStyles.Integer, CultureInfo.InvariantCulture, out int humidity))
            {
                extras.humidity = humidity;
            }
            if (float.TryParse(forecast.dewPoint, NumberStyles.Float, CultureInfo.InvariantCulture, out float dewpointF))
            {
                extras.dewpoint_f = dewpointF;
                extras.dewpoint_c = ConversionMethods.FtoC(dewpointF);
            }
            if (int.TryParse(forecast.precipitationProbability, NumberStyles.Integer, CultureInfo.InvariantCulture, out int pop))
            {
                extras.pop = pop;
            }
            if (float.TryParse(forecast.rainFall, NumberStyles.Float, CultureInfo.InvariantCulture, out float rain_in))
            {
                extras.qpf_rain_in = rain_in;
                extras.qpf_rain_mm = ConversionMethods.InToMM(rain_in);
            }
            if (float.TryParse(forecast.snowFall, NumberStyles.Float, CultureInfo.InvariantCulture, out float snow_in))
            {
                extras.qpf_snow_in = snow_in;
                extras.qpf_snow_cm = ConversionMethods.InToMM(snow_in / 10);
            }
            if (float.TryParse(forecast.barometerPressure, NumberStyles.Float, CultureInfo.InvariantCulture, out float pressureIN))
            {
                extras.pressure_in = pressureIN;
                extras.pressure_mb = ConversionMethods.InHgToMB(pressureIN);
            }
            if (int.TryParse(forecast.windDirection, NumberStyles.Integer, CultureInfo.InvariantCulture, out int windDegrees))
            {
                extras.wind_degrees = windDegrees;
            }
            if (float.TryParse(forecast.windSpeed, NumberStyles.Float, CultureInfo.InvariantCulture, out float windSpeed))
            {
                extras.wind_mph = windSpeed;
                extras.wind_kph = ConversionMethods.MphToKph(windSpeed);
            }
            if (float.TryParse(forecast.uvIndex, NumberStyles.Float, CultureInfo.InvariantCulture, out float uv_index))
            {
                extras.uv_index = uv_index;
            }
        }

        public Forecast(NWS.Period forecastItem)
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.NWS);
            var culture = CultureUtils.UserCulture;

            date = forecastItem.startTime.DateTime;
            if (forecastItem.isDaytime)
            {
                high_f = forecastItem.temperature;
                high_c = ConversionMethods.FtoC(forecastItem.temperature);
            }
            else
            {
                low_f = forecastItem.temperature;
                low_c = ConversionMethods.FtoC(forecastItem.temperature);
            }

            if (culture.TwoLetterISOLanguageName.Equals("en", StringComparison.InvariantCultureIgnoreCase) || culture.Equals(CultureInfo.InvariantCulture))
            {
                condition = forecastItem.shortForecast;
            }
            else
            {
                condition = provider.GetWeatherCondition(forecastItem.icon);
            }
            icon = provider.GetWeatherIcon(forecastItem.icon);

            if (forecastItem.windSpeed != null && forecastItem.windDirection != null)
            {
                var speeds = forecastItem.windSpeed.Replace(" mph", String.Empty).Split(" to ");
                var maxWindSpeed = speeds.LastOrDefault();
                if (!String.IsNullOrWhiteSpace(maxWindSpeed) && int.TryParse(maxWindSpeed, NumberStyles.Integer, CultureInfo.InvariantCulture, out int windSpeed))
                {
                    extras = new ForecastExtras()
                    {
                        wind_degrees = WeatherUtils.GetWindDirection(forecastItem.windDirection),
                        wind_mph = windSpeed,
                        wind_kph = ConversionMethods.MphToKph(windSpeed)
                    };
                }
            }
        }

        public Forecast(NWS.Period forecastItem, NWS.Period ntForecastItem)
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.NWS);
            var culture = CultureUtils.UserCulture;

            date = forecastItem.startTime.DateTime;
            high_f = forecastItem.temperature;
            high_c = ConversionMethods.FtoC(forecastItem.temperature);
            low_f = ntForecastItem.temperature;
            low_c = ConversionMethods.FtoC(ntForecastItem.temperature);

            if (culture.TwoLetterISOLanguageName.Equals("en", StringComparison.InvariantCultureIgnoreCase) || culture.Equals(CultureInfo.InvariantCulture))
            {
                condition = forecastItem.shortForecast;
            }
            else
            {
                condition = provider.GetWeatherCondition(forecastItem.icon);
            }
            icon = provider.GetWeatherIcon(forecastItem.icon);

            if (forecastItem.windSpeed != null && forecastItem.windDirection != null)
            {
                // windSpeed is reported usually as, for ex., '7 to 10 mph'
                // Format and split text into min and max
                var speeds = forecastItem.windSpeed.Replace(" mph", String.Empty).Split(" to ");
                var maxWindSpeed = speeds.LastOrDefault();
                if (!String.IsNullOrWhiteSpace(maxWindSpeed) && int.TryParse(maxWindSpeed, NumberStyles.Integer, CultureInfo.InvariantCulture, out int windSpeed))
                {
                    extras = new ForecastExtras()
                    {
                        wind_degrees = WeatherUtils.GetWindDirection(forecastItem.windDirection),
                        wind_mph = windSpeed,
                        wind_kph = ConversionMethods.MphToKph(windSpeed)
                    };
                }
            }
        }
    }

    public partial class HourlyForecast
    {
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

            // Use cloudiness value here
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

        public HourlyForecast(Metno.Timesery hr_forecast)
        {
            date = new DateTimeOffset(hr_forecast.time, TimeSpan.Zero);
            high_f = ConversionMethods.CtoF(hr_forecast.data.instant.details.air_temperature.Value);
            high_c = hr_forecast.data.instant.details.air_temperature.Value;
            wind_degrees = (int)Math.Round(hr_forecast.data.instant.details.wind_from_direction.Value);
            wind_mph = (float)Math.Round(ConversionMethods.MSecToMph(hr_forecast.data.instant.details.wind_speed.Value));
            wind_kph = (float)Math.Round(ConversionMethods.MSecToKph(hr_forecast.data.instant.details.wind_speed.Value));

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
                feelslike_f = WeatherUtils.GetFeelsLikeTemp(high_f.Value, wind_mph.Value, (int)Math.Round(humidity)),
                feelslike_c = ConversionMethods.FtoC(WeatherUtils.GetFeelsLikeTemp(high_f.Value, wind_mph.Value, (int)Math.Round(humidity))),
                humidity = (int)Math.Round(humidity),
                dewpoint_f = ConversionMethods.CtoF(hr_forecast.data.instant.details.dew_point_temperature.Value),
                dewpoint_c = hr_forecast.data.instant.details.dew_point_temperature.Value,
                pressure_in = ConversionMethods.MBToInHg(hr_forecast.data.instant.details.air_pressure_at_sea_level.Value),
                pressure_mb = hr_forecast.data.instant.details.air_pressure_at_sea_level.Value,
                wind_degrees = wind_degrees,
                wind_mph = wind_mph,
                wind_kph = wind_kph
            };
            if (hr_forecast.data.instant.details.cloud_area_fraction.HasValue)
            {
                extras.cloudiness = (int)Math.Round(hr_forecast.data.instant.details.cloud_area_fraction.Value);
            }
            // Precipitation
            if (hr_forecast.data.instant.details?.probability_of_precipitation.HasValue == true)
            {
                extras.pop = (int)Math.Round(hr_forecast.data.instant.details.probability_of_precipitation.Value);
            }
            else if (hr_forecast.data.next_1_hours?.details?.probability_of_precipitation.HasValue == true)
            {
                extras.pop = (int)Math.Round(hr_forecast.data.next_1_hours.details.probability_of_precipitation.Value);
            }
            else if (hr_forecast.data.next_6_hours?.details?.probability_of_precipitation.HasValue == true)
            {
                extras.pop = (int)Math.Round(hr_forecast.data.next_6_hours.details.probability_of_precipitation.Value);
            }
            else if (hr_forecast.data.next_12_hours?.details?.probability_of_precipitation.HasValue == true)
            {
                extras.pop = (int)Math.Round(hr_forecast.data.next_12_hours.details.probability_of_precipitation.Value);
            }
            if (hr_forecast.data.instant.details.wind_speed_of_gust.HasValue)
            {
                extras.windgust_mph = (float)Math.Round(ConversionMethods.MSecToMph(hr_forecast.data.instant.details.wind_speed_of_gust.Value));
                extras.windgust_kph = (float)Math.Round(ConversionMethods.MSecToKph(hr_forecast.data.instant.details.wind_speed_of_gust.Value));
            }
            if (hr_forecast.data.instant.details.fog_area_fraction.HasValue)
            {
                float visMi = 10.0f;
                extras.visibility_mi = (visMi - (visMi * hr_forecast.data.instant.details.fog_area_fraction.Value / 100));
                extras.visibility_km = ConversionMethods.MiToKm(extras.visibility_mi.Value);
            }
            if (hr_forecast.data.instant.details.ultraviolet_index_clear_sky.HasValue)
            {
                extras.uv_index = hr_forecast.data.instant.details.ultraviolet_index_clear_sky.Value;
            }
        }

        public HourlyForecast(HERE.Forecast1 hr_forecast)
        {
            date = hr_forecast.utcTime;
            if (float.TryParse(hr_forecast.temperature, NumberStyles.Float, CultureInfo.InvariantCulture, out float highF))
            {
                high_f = highF;
                high_c = ConversionMethods.FtoC(highF);
            }
            condition = hr_forecast.description.ToPascalCase();

            icon = WeatherManager.GetProvider(WeatherAPI.Here)
                   .GetWeatherIcon(string.Format(CultureInfo.InvariantCulture, "{0}_{1}", hr_forecast.daylight, hr_forecast.iconName));

            if (int.TryParse(hr_forecast.windDirection, NumberStyles.Integer, CultureInfo.InvariantCulture, out int windDeg))
                wind_degrees = windDeg;
            if (float.TryParse(hr_forecast.windSpeed, NumberStyles.Float, CultureInfo.InvariantCulture, out float windSpeed))
            {
                wind_mph = windSpeed;
                wind_kph = ConversionMethods.MphToKph(windSpeed);
            }

            // Extras
            extras = new ForecastExtras();
            if (float.TryParse(hr_forecast.comfort, NumberStyles.Float, CultureInfo.InvariantCulture, out float comfortTemp_f))
            {
                extras.feelslike_f = comfortTemp_f;
                extras.feelslike_c = ConversionMethods.FtoC(comfortTemp_f);
            }
            if (int.TryParse(hr_forecast.humidity, NumberStyles.Integer, CultureInfo.InvariantCulture, out int humidity))
            {
                extras.humidity = humidity;
            }
            if (float.TryParse(hr_forecast.dewPoint, NumberStyles.Float, CultureInfo.InvariantCulture, out float dewpointF))
            {
                extras.dewpoint_f = dewpointF;
                extras.dewpoint_c = ConversionMethods.FtoC(dewpointF);
            }
            if (float.TryParse(hr_forecast.visibility, NumberStyles.Float, CultureInfo.InvariantCulture, out float visibilityMI))
            {
                extras.visibility_mi = visibilityMI;
                extras.visibility_km = ConversionMethods.MiToKm(visibilityMI);
            }
            if (int.TryParse(hr_forecast.precipitationProbability, NumberStyles.Integer, CultureInfo.InvariantCulture, out int PoP))
            {
                extras.pop = PoP;
            }
            if (float.TryParse(hr_forecast.rainFall, NumberStyles.Float, CultureInfo.InvariantCulture, out float rain_in))
            {
                extras.qpf_rain_in = rain_in;
                extras.qpf_rain_mm = ConversionMethods.InToMM(rain_in);
            }
            if (float.TryParse(hr_forecast.snowFall, NumberStyles.Float, CultureInfo.InvariantCulture, out float snow_in))
            {
                extras.qpf_snow_in = snow_in;
                extras.qpf_snow_cm = ConversionMethods.InToMM(snow_in / 10);
            }
            if (float.TryParse(hr_forecast.barometerPressure, NumberStyles.Float, CultureInfo.InvariantCulture, out float pressureIN))
            {
                extras.pressure_in = pressureIN;
                extras.pressure_mb = ConversionMethods.InHgToMB(pressureIN);
            }
            extras.wind_degrees = wind_degrees;
            extras.wind_mph = wind_mph;
            extras.wind_kph = wind_kph;
        }

        public HourlyForecast(NWS.Period forecastItem)
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.NWS);
            var culture = CultureUtils.UserCulture;

            date = forecastItem.startTime;
            high_f = forecastItem.temperature;
            high_c = ConversionMethods.FtoC(forecastItem.temperature);

            if (culture.TwoLetterISOLanguageName.Equals("en", StringComparison.InvariantCultureIgnoreCase) || culture.Equals(CultureInfo.InvariantCulture))
            {
                condition = forecastItem.shortForecast;
            }
            else
            {
                condition = provider.GetWeatherCondition(forecastItem.icon);
            }
            icon = provider.GetWeatherIcon(forecastItem.icon);

            if (forecastItem.windSpeed != null && forecastItem.windDirection != null)
            {
                wind_degrees = WeatherUtils.GetWindDirection(forecastItem.windDirection);

                // windSpeed is reported usually as, for ex., '7 to 10 mph'
                // Format and split text into min and max
                var speeds = forecastItem.windSpeed.Replace(" mph", String.Empty).Split(" to ");
                var maxWindSpeed = speeds.LastOrDefault();
                if (!String.IsNullOrWhiteSpace(maxWindSpeed) && int.TryParse(maxWindSpeed, NumberStyles.Integer, CultureInfo.InvariantCulture, out int windSpeed))
                {
                    wind_mph = windSpeed;
                    wind_kph = ConversionMethods.MphToKph(windSpeed);

                    // Extras
                    extras = new ForecastExtras()
                    {
                        wind_degrees = this.wind_degrees,
                        wind_mph = this.wind_mph,
                        wind_kph = this.wind_kph
                    };
                }
            }
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
                Math.Round(ConversionMethods.KtoF(forecast.temp.morn)),
                SimpleLibrary.ResLoader.GetString("FeelsLike_Label"),
                Math.Round(ConversionMethods.KtoF(forecast.feels_like.morn)));
            sb.AppendLine();
            sb.AppendFormat(CultureInfo.InvariantCulture,
                "{0} - {1}: {2}°; {3}: {4}°", SimpleLibrary.ResLoader.GetString("Label_Day"),
                SimpleLibrary.ResLoader.GetString("Temp_Label"),
                Math.Round(ConversionMethods.KtoF(forecast.temp.day)),
                SimpleLibrary.ResLoader.GetString("FeelsLike_Label"),
                Math.Round(ConversionMethods.KtoF(forecast.temp.day)));
            sb.AppendLine();
            sb.AppendFormat(CultureInfo.InvariantCulture,
                "{0} - {1}: {2}°; {3}: {4}°", SimpleLibrary.ResLoader.GetString("Label_Eve"),
                SimpleLibrary.ResLoader.GetString("Temp_Label"),
                Math.Round(ConversionMethods.KtoF(forecast.temp.eve)),
                SimpleLibrary.ResLoader.GetString("FeelsLike_Label"),
                Math.Round(ConversionMethods.KtoF(forecast.feels_like.eve)));
            sb.AppendLine();
            sb.AppendFormat(CultureInfo.InvariantCulture,
                "{0} - {1}: {2}°; {3}: {4}°", SimpleLibrary.ResLoader.GetString("Label_Night"),
                SimpleLibrary.ResLoader.GetString("Temp_Label"),
                Math.Round(ConversionMethods.KtoF(forecast.temp.night)),
                SimpleLibrary.ResLoader.GetString("FeelsLike_Label"),
                Math.Round(ConversionMethods.KtoF(forecast.feels_like.night)));

            fcttext = sb.ToString();

            var sb_metric = new StringBuilder();
            sb_metric.AppendFormat(CultureInfo.InvariantCulture,
                "{0} - {1}: {2}°; {3}: {4}°", SimpleLibrary.ResLoader.GetString("Label_Morning"),
                SimpleLibrary.ResLoader.GetString("Temp_Label"),
                Math.Round(ConversionMethods.KtoC(forecast.temp.morn)),
                SimpleLibrary.ResLoader.GetString("FeelsLike_Label"),
                Math.Round(ConversionMethods.KtoC(forecast.feels_like.morn)));
            sb_metric.AppendLine();
            sb_metric.AppendFormat(CultureInfo.InvariantCulture,
                "{0} - {1}: {2}°; {3}: {4}°", SimpleLibrary.ResLoader.GetString("Label_Day"),
                SimpleLibrary.ResLoader.GetString("Temp_Label"),
                Math.Round(ConversionMethods.KtoC(forecast.temp.day)),
                SimpleLibrary.ResLoader.GetString("FeelsLike_Label"),
                Math.Round(ConversionMethods.KtoC(forecast.feels_like.day)));
            sb_metric.AppendLine();
            sb_metric.AppendFormat(CultureInfo.InvariantCulture,
                "{0} - {1}: {2}°; {3}: {4}°", SimpleLibrary.ResLoader.GetString("Label_Eve"),
                SimpleLibrary.ResLoader.GetString("Temp_Label"),
                Math.Round(ConversionMethods.KtoC(forecast.temp.eve)),
                SimpleLibrary.ResLoader.GetString("FeelsLike_Label"),
                Math.Round(ConversionMethods.KtoC(forecast.feels_like.eve)));
            sb_metric.AppendLine();
            sb_metric.AppendFormat(CultureInfo.InvariantCulture,
                "{0} - {1}: {2}°; {3}: {4}°", SimpleLibrary.ResLoader.GetString("Label_Night"),
                SimpleLibrary.ResLoader.GetString("Temp_Label"),
                Math.Round(ConversionMethods.KtoC(forecast.temp.night)),
                SimpleLibrary.ResLoader.GetString("FeelsLike_Label"),
                Math.Round(ConversionMethods.KtoC(forecast.feels_like.night)));

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
            var provider = WeatherManager.GetProvider(WeatherAPI.Yahoo);
            var culture = CultureUtils.UserCulture;

            if (culture.TwoLetterISOLanguageName.Equals("en", StringComparison.InvariantCultureIgnoreCase) || culture.Equals(CultureInfo.InvariantCulture))
            {
                weather = observation.condition.text;
            }
            else
            {
                weather = provider.GetWeatherCondition(observation.condition.code.ToInvariantString());
            }
            icon = provider.GetWeatherIcon(observation.condition.code.ToInvariantString());

            temp_f = observation.condition.temperature;
            temp_c = ConversionMethods.FtoC(observation.condition.temperature);
            wind_degrees = observation.wind.direction;
            wind_mph = observation.wind.speed;
            wind_kph = ConversionMethods.MphToKph(observation.wind.speed);
            feelslike_f = observation.wind.chill;
            feelslike_c = ConversionMethods.FtoC(observation.wind.chill);

            beaufort = new Beaufort((int)WeatherUtils.GetBeaufortScale((int)Math.Round(observation.wind.speed)));
        }

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
                windgust_kph = ConversionMethods.MSecToMph(current.wind_gust.Value);
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

        public Condition(Metno.Timesery time)
        {
            // weather
            temp_f = ConversionMethods.CtoF(time.data.instant.details.air_temperature.Value);
            temp_c = (float)time.data.instant.details.air_temperature.Value;
            wind_degrees = (int)Math.Round(time.data.instant.details.wind_from_direction.Value);
            wind_mph = (float)Math.Round(ConversionMethods.MSecToMph(time.data.instant.details.wind_speed.Value));
            wind_kph = (float)Math.Round(ConversionMethods.MSecToKph(time.data.instant.details.wind_speed.Value));
            feelslike_f = WeatherUtils.GetFeelsLikeTemp(temp_f.Value, wind_mph.Value, (int)time.data.instant.details.relative_humidity.Value);
            feelslike_c = ConversionMethods.FtoC(feelslike_f.Value);
            if (time.data.instant.details.wind_speed_of_gust.HasValue)
            {
                windgust_mph = (float) Math.Round(ConversionMethods.MSecToMph(time.data.instant.details.wind_speed_of_gust.Value));
                windgust_kph = (float) Math.Round(ConversionMethods.MSecToKph(time.data.instant.details.wind_speed_of_gust.Value));
            }

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

            beaufort = new Beaufort((int)WeatherUtils.GetBeaufortScale(time.data.instant.details.wind_speed.Value));
            if (time.data.instant.details.ultraviolet_index_clear_sky.HasValue)
            {
                uv = new UV(time.data.instant.details.ultraviolet_index_clear_sky.Value);
            }
        }

        public Condition(HERE.Observation observation, HERE.Forecast forecastItem)
        {
            weather = observation.description.ToPascalCase();
            if (float.TryParse(observation.temperature, NumberStyles.Float, CultureInfo.InvariantCulture, out float tempF))
            {
                temp_f = tempF;
                temp_c = ConversionMethods.FtoC(tempF);
            }

            if (float.TryParse(observation.highTemperature, NumberStyles.Float, CultureInfo.InvariantCulture, out float hiTempF) &&
                float.TryParse(observation.lowTemperature, NumberStyles.Float, CultureInfo.InvariantCulture, out float loTempF))
            {
                high_f = hiTempF;
                high_c = ConversionMethods.FtoC(hiTempF);
                low_f = loTempF;
                low_c = ConversionMethods.FtoC(loTempF);
            }
            else if (float.TryParse(forecastItem.highTemperature, NumberStyles.Float, CultureInfo.InvariantCulture, out hiTempF) &&
                float.TryParse(forecastItem.lowTemperature, NumberStyles.Float, CultureInfo.InvariantCulture, out loTempF))
            {
                high_f = hiTempF;
                high_c = ConversionMethods.FtoC(hiTempF);
                low_f = loTempF;
                low_c = ConversionMethods.FtoC(loTempF);
            }

            if (int.TryParse(observation.windDirection, NumberStyles.Integer, CultureInfo.InvariantCulture, out int windDegrees))
                wind_degrees = windDegrees;
            else
                wind_degrees = 0;

            if (float.TryParse(observation.windSpeed, NumberStyles.Float, CultureInfo.InvariantCulture, out float wind_Speed))
            {
                wind_mph = wind_Speed;
                wind_kph = ConversionMethods.MphToKph(wind_Speed);
            }

            if (float.TryParse(observation.comfort, NumberStyles.Float, CultureInfo.InvariantCulture, out float comfortTempF))
            {
                feelslike_f = comfortTempF;
                feelslike_c = ConversionMethods.FtoC(comfortTempF);
            }

            icon = WeatherManager.GetProvider(WeatherAPI.Here)
                   .GetWeatherIcon(string.Format("{0}_{1}", observation.daylight, observation.iconName));

            if (int.TryParse(forecastItem.beaufortScale, NumberStyles.Integer, CultureInfo.InvariantCulture, out int scale))
                beaufort = new Beaufort(scale, forecastItem.beaufortDescription);

            if (float.TryParse(forecastItem.uvIndex, NumberStyles.Float, CultureInfo.InvariantCulture, out float index))
                uv = new UV(index, forecastItem.uvDesc);

            observation_time = observation.utcTime;
        }

        public Condition(NWS.ObservationsCurrentRootobject obsCurrentRootObject)
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.NWS);
            var culture = CultureUtils.UserCulture;

            if (culture.TwoLetterISOLanguageName.Equals("en", StringComparison.InvariantCultureIgnoreCase) || culture.Equals(CultureInfo.InvariantCulture))
            {
                weather = obsCurrentRootObject.textDescription;
            }
            else
            {
                weather = provider.GetWeatherCondition(obsCurrentRootObject.icon);
            }
            icon = provider.GetWeatherIcon(obsCurrentRootObject.icon);

            if (obsCurrentRootObject.temperature.value.HasValue)
            {
                temp_c = obsCurrentRootObject.temperature.value.GetValueOrDefault(0.00f);
                temp_f = ConversionMethods.CtoF(temp_c.Value);
            }

            if (obsCurrentRootObject.windDirection.value.HasValue)
            {
                wind_degrees = (int)obsCurrentRootObject.windDirection.value.GetValueOrDefault(0);
            }

            if (obsCurrentRootObject.windSpeed.value.HasValue)
            {
                wind_kph = obsCurrentRootObject.windSpeed.value.GetValueOrDefault(0.00f);
                wind_mph = ConversionMethods.KphToMph(wind_kph.Value);
            }

            if (obsCurrentRootObject.windGust.value.HasValue)
            {
                windgust_kph = obsCurrentRootObject.windGust.value;
                windgust_mph = ConversionMethods.KphToMph(windgust_kph.Value);
            }

            if (obsCurrentRootObject.heatIndex.value.HasValue)
            {
                feelslike_c = obsCurrentRootObject.heatIndex.value.GetValueOrDefault(0.00f);
                feelslike_f = ConversionMethods.CtoF(feelslike_c.Value);
            }
            else if (obsCurrentRootObject.windChill.value.HasValue)
            {
                feelslike_c = obsCurrentRootObject.windChill.value.GetValueOrDefault(0.00f);
                feelslike_f = ConversionMethods.CtoF(feelslike_c.Value);
            }
            else if (temp_f.HasValue && !Equals(temp_f, temp_c) && wind_mph.HasValue)
            {
                float humidity = obsCurrentRootObject.relativeHumidity.value.GetValueOrDefault(-1.0f);
                feelslike_f = WeatherUtils.GetFeelsLikeTemp(temp_f.Value, wind_mph.Value, (int)humidity);
                feelslike_c = ConversionMethods.FtoC(feelslike_f.Value);
            }

            if (wind_mph.HasValue)
            {
                beaufort = new Beaufort((int)WeatherUtils.GetBeaufortScale((int)Math.Round(wind_mph.Value)));
            }

            observation_time = obsCurrentRootObject.timestamp;
        }
    }

    public partial class Atmosphere
    {
        public Atmosphere(WeatherYahoo.Atmosphere atmosphere)
        {
            humidity = atmosphere.humidity;
            pressure_in = atmosphere.pressure;
            pressure_mb = ConversionMethods.InHgToMB(atmosphere.pressure);
            pressure_trend = atmosphere.rising.ToInvariantString();
            visibility_mi = atmosphere.visibility;
            visibility_km = ConversionMethods.MiToKm(atmosphere.visibility);
        }

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

        public Atmosphere(Metno.Timesery time)
        {
            humidity = (int?)Math.Round(time.data.instant.details.relative_humidity.Value);
            pressure_mb = time.data.instant.details.air_pressure_at_sea_level.Value;
            pressure_in = ConversionMethods.MBToInHg(time.data.instant.details.air_pressure_at_sea_level.Value);
            pressure_trend = String.Empty;

            if (time.data.instant.details.fog_area_fraction.HasValue)
            {
                float visMi = 10.0f;
                visibility_mi = (visMi - (visMi * time.data.instant.details.fog_area_fraction.Value / 100));
                visibility_km = ConversionMethods.MiToKm(visibility_mi.Value);
            }

            if (time.data.instant.details.dew_point_temperature.HasValue)
            {
                dewpoint_f = ConversionMethods.CtoF(time.data.instant.details.dew_point_temperature.Value);
                dewpoint_c = time.data.instant.details.dew_point_temperature.Value;
            }
        }

        public Atmosphere(HERE.Observation observation)
        {
            if (int.TryParse(observation.humidity, NumberStyles.Integer, CultureInfo.InvariantCulture, out int Humidity))
            {
                humidity = Humidity;
            }

            if (float.TryParse(observation.barometerPressure, NumberStyles.Float, CultureInfo.InvariantCulture, out float pressureIN))
            {
                pressure_in = pressureIN;
                pressure_mb = ConversionMethods.InHgToMB(pressureIN);
            }
            pressure_trend = observation.barometerTrend;

            if (float.TryParse(observation.visibility, NumberStyles.Float, CultureInfo.InvariantCulture, out float visibilityMI))
            {
                visibility_mi = visibilityMI;
                visibility_km = ConversionMethods.MiToKm(visibilityMI);
            }

            if (float.TryParse(observation.dewPoint, NumberStyles.Float, CultureInfo.InvariantCulture, out float dewpointF))
            {
                dewpoint_f = dewpointF;
                dewpoint_c = ConversionMethods.FtoC(dewpointF);
            }
        }

        public Atmosphere(NWS.ObservationsCurrentRootobject obsCurrentRootObject)
        {
            if (obsCurrentRootObject.relativeHumidity.value.HasValue)
            {
                humidity = (int)Math.Round(obsCurrentRootObject.relativeHumidity.value.GetValueOrDefault(0.00f));
            }

            if (obsCurrentRootObject.barometricPressure.value.HasValue)
            {
                var pressure_pa = obsCurrentRootObject.barometricPressure.value.GetValueOrDefault(0.00f);
                pressure_in = ConversionMethods.PaToInHg(pressure_pa);
                pressure_mb = ConversionMethods.PaToMB(pressure_pa);
            }
            pressure_trend = String.Empty;

            if (obsCurrentRootObject.visibility.value.HasValue)
            {
                visibility_km = obsCurrentRootObject.visibility.value.GetValueOrDefault(0.00f) / 1000;
                visibility_mi = ConversionMethods.KmToMi(obsCurrentRootObject.visibility.value.GetValueOrDefault(0.00f) / 1000);
            }

            if (obsCurrentRootObject.dewpoint.value.HasValue)
            {
                dewpoint_c = obsCurrentRootObject.dewpoint.value.GetValueOrDefault(0.00f);
                dewpoint_f = ConversionMethods.CtoF(obsCurrentRootObject.dewpoint.value.GetValueOrDefault(0.00f));
            }
        }
    }

    public partial class Astronomy
    {
        public Astronomy(WeatherYahoo.Astronomy astronomy)
        {
            if (DateTime.TryParse(astronomy.sunrise, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime sunrise))
                this.sunrise = sunrise;
            if (DateTime.TryParse(astronomy.sunset, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime sunset))
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
                    moonPhaseValue = (int)Math.Round(double.Parse(time.moonphase.value, CultureInfo.InvariantCulture));
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

            if (DateTime.TryParse(astroData.sunrise, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime sunrise))
                this.sunrise = sunrise;
            if (DateTime.TryParse(astroData.sunset, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime sunset))
                this.sunset = sunset;
            if (DateTime.TryParse(astroData.moonrise, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime moonrise))
                this.moonrise = moonrise;
            if (DateTime.TryParse(astroData.moonset, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime moonset))
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

        public Precipitation(Metno.Timesery time)
        {
            // Use cloudiness value here
            cloudiness = (int)Math.Round(time.data.instant.details.cloud_area_fraction.Value);
            // Precipitation
            if (time.data.instant.details?.probability_of_precipitation.HasValue == true)
            {
                pop = (int)Math.Round(time.data.instant.details.probability_of_precipitation.Value);
            }
            else if (time.data.next_1_hours?.details?.probability_of_precipitation.HasValue == true)
            {
                pop = (int)Math.Round(time.data.next_1_hours.details.probability_of_precipitation.Value);
            }
            else if (time.data.next_6_hours?.details?.probability_of_precipitation.HasValue == true)
            {
                pop = (int)Math.Round(time.data.next_6_hours.details.probability_of_precipitation.Value);
            }
            else if (time.data.next_12_hours?.details?.probability_of_precipitation.HasValue == true)
            {
                pop = (int)Math.Round(time.data.next_12_hours.details.probability_of_precipitation.Value);
            }
            // The rest DNE
        }

        public Precipitation(HERE.Forecast forecast)
        {
            if (int.TryParse(forecast.precipitationProbability, NumberStyles.Integer, CultureInfo.InvariantCulture, out int PoP))
                pop = PoP;

            if (float.TryParse(forecast.rainFall, NumberStyles.Float, CultureInfo.InvariantCulture, out float rain_in))
            {
                qpf_rain_in = rain_in;
                qpf_rain_mm = ConversionMethods.InToMM(qpf_rain_in.Value);
            }

            if (float.TryParse(forecast.snowFall, NumberStyles.Float, CultureInfo.InvariantCulture, out float snow_in))
            {
                qpf_snow_in = snow_in;
                qpf_snow_cm = ConversionMethods.InToMM(qpf_snow_in.Value) / 10;
            }
        }

        public Precipitation(NWS.ObservationsCurrentRootobject obsCurrentRootObject)
        {
            if (obsCurrentRootObject.precipitationLastHour.value.HasValue)
            {
                // "unit:m"
                qpf_rain_mm = obsCurrentRootObject.precipitationLastHour.value.GetValueOrDefault(0.00f) * 1000;
                qpf_rain_in = ConversionMethods.MMToIn(qpf_rain_mm.Value);
            }
            else if (obsCurrentRootObject.precipitationLast3Hours.value.HasValue)
            {
                // "unit:m"
                qpf_rain_mm = obsCurrentRootObject.precipitationLast3Hours.value.GetValueOrDefault(0.00f) * 1000;
                qpf_rain_in = ConversionMethods.MMToIn(qpf_rain_mm.Value);
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
                    desc = SimpleLibrary.ResLoader.GetString("/Beaufort/Beaufort_0");
                    break;

                case 1:
                    scale = BeaufortScale.B1;
                    desc = SimpleLibrary.ResLoader.GetString("/Beaufort/Beaufort_1");
                    break;

                case 2:
                    scale = BeaufortScale.B2;
                    desc = SimpleLibrary.ResLoader.GetString("/Beaufort/Beaufort_2");
                    break;

                case 3:
                    scale = BeaufortScale.B3;
                    desc = SimpleLibrary.ResLoader.GetString("/Beaufort/Beaufort_3");
                    break;

                case 4:
                    scale = BeaufortScale.B4;
                    desc = SimpleLibrary.ResLoader.GetString("/Beaufort/Beaufort_4");
                    break;

                case 5:
                    scale = BeaufortScale.B5;
                    desc = SimpleLibrary.ResLoader.GetString("/Beaufort/Beaufort_5");
                    break;

                case 6:
                    scale = BeaufortScale.B6;
                    desc = SimpleLibrary.ResLoader.GetString("/Beaufort/Beaufort_6");
                    break;

                case 7:
                    scale = BeaufortScale.B7;
                    desc = SimpleLibrary.ResLoader.GetString("/Beaufort/Beaufort_7");
                    break;

                case 8:
                    scale = BeaufortScale.B8;
                    desc = SimpleLibrary.ResLoader.GetString("/Beaufort/Beaufort_8");
                    break;

                case 9:
                    scale = BeaufortScale.B9;
                    desc = SimpleLibrary.ResLoader.GetString("/Beaufort/Beaufort_9");
                    break;

                case 10:
                    scale = BeaufortScale.B10;
                    desc = SimpleLibrary.ResLoader.GetString("/Beaufort/Beaufort_10");
                    break;

                case 11:
                    scale = BeaufortScale.B11;
                    desc = SimpleLibrary.ResLoader.GetString("/Beaufort/Beaufort_11");
                    break;

                case 12:
                    scale = BeaufortScale.B12;
                    desc = SimpleLibrary.ResLoader.GetString("/Beaufort/Beaufort_12");
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
                    desc = SimpleLibrary.ResLoader.GetString("/MoonPhases/MoonPhase_New");
                    break;

                case MoonPhaseType.WaxingCrescent:
                    desc = SimpleLibrary.ResLoader.GetString("/MoonPhases/MoonPhase_WaxCrescent");
                    break;

                case MoonPhaseType.FirstQtr:
                    desc = SimpleLibrary.ResLoader.GetString("/MoonPhases/MoonPhase_FirstQtr");
                    break;

                case MoonPhaseType.WaxingGibbous:
                    desc = SimpleLibrary.ResLoader.GetString("/MoonPhases/MoonPhase_WaxGibbous");
                    break;

                case MoonPhaseType.FullMoon:
                    desc = SimpleLibrary.ResLoader.GetString("/MoonPhases/MoonPhase_Full");
                    break;

                case MoonPhaseType.WaningGibbous:
                    desc = SimpleLibrary.ResLoader.GetString("/MoonPhases/MoonPhase_WanGibbous");
                    break;

                case MoonPhaseType.LastQtr:
                    desc = SimpleLibrary.ResLoader.GetString("/MoonPhases/MoonPhase_LastQtr");
                    break;

                case MoonPhaseType.WaningCrescent:
                    desc = SimpleLibrary.ResLoader.GetString("/MoonPhases/MoonPhase_WanCrescent");
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
                desc = SimpleLibrary.ResLoader.GetString("/UVIndex/UV_0");
            }
            else if (index >= 3 && index < 6)
            {
                desc = SimpleLibrary.ResLoader.GetString("/UVIndex/UV_3");
            }
            else if (index >= 6 && index < 8)
            {
                desc = SimpleLibrary.ResLoader.GetString("/UVIndex/UV_6");
            }
            else if (index >= 8 && index < 11)
            {
                desc = SimpleLibrary.ResLoader.GetString("/UVIndex/UV_8");
            }
            else if (index >= 11)
            {
                desc = SimpleLibrary.ResLoader.GetString("/UVIndex/UV_11");
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