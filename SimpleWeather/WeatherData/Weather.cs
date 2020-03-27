using SimpleWeather.Utils;
using SimpleWeather.UWP;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;

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

            source = WeatherAPI.Yahoo;
        }

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
            int lastDay = 0;

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

                // Get every 8th item for daily forecast
                if (i % 8 == 0)
                {
                    lastDay = i / 8;

                    forecast.Insert(i / 8, new Forecast(foreRoot.list[i]));
                }

                // This is possibly the last forecast for the day (3-hrly forecast)
                // Set the min / max temp here and reset
                if (hr_forecast[i].date.Hour >= 21)
                {
                    if (!float.IsNaN(dayMax))
                    {
                        forecast[lastDay].high_f = ConversionMethods.KtoF(dayMax.ToString(CultureInfo.InvariantCulture));
                        forecast[lastDay].high_c = ConversionMethods.KtoC(dayMax.ToString(CultureInfo.InvariantCulture));
                    }
                    if (!float.IsNaN(dayMin))
                    {
                        forecast[lastDay].low_f = ConversionMethods.KtoF(dayMin.ToString(CultureInfo.InvariantCulture));
                        forecast[lastDay].low_c = ConversionMethods.KtoC(dayMin.ToString(CultureInfo.InvariantCulture));
                    }

                    dayMax = float.NaN;
                    dayMin = float.NaN;
                }
            }
            condition = new Condition(currRoot);
            atmosphere = new Atmosphere(currRoot);
            astronomy = new Astronomy(currRoot);
            precipitation = new Precipitation(currRoot);
            ttl = "120";

            query = currRoot.id.ToString();

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

            source = WeatherAPI.OpenWeatherMap;
        }

        public Weather(Metno.weatherdata foreRoot, Metno.astrodata astroRoot)
        {
            location = new Location(foreRoot);
            update_time = foreRoot.created;

            // 9-day forecast / hrly -> 6hrly forecast
            forecast = new List<Forecast>(10);
            hr_forecast = new List<HourlyForecast>(90);

            // Store potential min/max values
            float dayMax = float.NaN;
            float dayMin = float.NaN;

            // Flag values
            bool end = false;
            bool conditionSet = false;
            int fcastCount = 0;

            DateTime startDate = foreRoot.meta.model.from;
            DateTime endDate = foreRoot.meta.model.to;
            Forecast fcast = null;

            // Metno data is troublesome to parse thru
            for (int i = 0; i < foreRoot.product.time.Length; i++)
            {
                var time = foreRoot.product.time[i];
                DateTime date = time.from;

                // Create condition for next 2hrs from data
                if (i == 0 && date.Equals(startDate))
                {
                    condition = new Condition(time);
                    atmosphere = new Atmosphere(time);
                    precipitation = new Precipitation(time);
                }

                // This contains all weather details
                if (!end && time.to.Subtract(time.from).Ticks == 0)
                {
                    // Find max/min for each hour
                    float temp = (float)time.location.temperature.value;
                    if (!float.IsNaN(temp) && (float.IsNaN(dayMax) || temp > dayMax))
                    {
                        dayMax = temp;
                    }
                    if (!float.IsNaN(temp) && (float.IsNaN(dayMin) || temp < dayMin))
                    {
                        dayMin = temp;
                    }

                    // Add a new hour
                    hr_forecast.Add(new HourlyForecast(time));

                    // Create new forecast
                    if (date.Hour == 0 || date.Equals(startDate))
                    {
                        fcastCount++;

                        // Oops, we missed one
                        if (fcast != null && fcastCount != forecast.Count)
                        {
                            // Set forecast properties here:
                            // condition (set in provider GetWeather method)
                            // date
                            fcast.date = date;
                            // high
                            fcast.high_f = ConversionMethods.CtoF(dayMax.ToString(CultureInfo.InvariantCulture));
                            fcast.high_c = Math.Round(dayMax).ToString();
                            // low
                            fcast.low_f = ConversionMethods.CtoF(dayMin.ToString(CultureInfo.InvariantCulture));
                            fcast.low_c = Math.Round(dayMin).ToString();
                            // icon
                            forecast.Add(fcast);

                            // Reset
                            dayMax = float.NaN;
                            dayMin = float.NaN;
                        }

                        fcast = new Forecast(time);
                    }
                    // Last forecast for day; create forecast
                    if (date.Hour == 23 || date.Equals(endDate))
                    {
                        // condition (set in provider GetWeather method)
                        // date
                        fcast.date = date;
                        // high
                        fcast.high_f = ConversionMethods.CtoF(dayMax.ToString(CultureInfo.InvariantCulture));
                        fcast.high_c = Math.Round(dayMax).ToString();
                        // low
                        fcast.low_f = ConversionMethods.CtoF(dayMin.ToString(CultureInfo.InvariantCulture));
                        fcast.low_c = Math.Round(dayMin).ToString();
                        // icon
                        forecast.Add(fcast);

                        if (date.Equals(endDate))
                            end = true;

                        // Reset
                        dayMax = float.NaN;
                        dayMin = float.NaN;
                        fcast = null;
                    }
                }

                // Get conditions for hour if available
                if (hr_forecast.Count > 1 &&
                    hr_forecast[hr_forecast.Count - 2].date.Equals(time.from))
                {
                    // Set condition from id
                    var hr = hr_forecast[hr_forecast.Count - 2];
                    if (String.IsNullOrEmpty(hr.icon))
                    {
                        if (time.location.symbol != null)
                        {
                            hr.condition = time.location.symbol.id;
                            hr.icon = time.location.symbol.number.ToString();
                        }
                    }
                }
                else if (end && hr_forecast.Last().date.Equals(time.from))
                {
                    // Set condition from id
                    var hr = hr_forecast.Last();
                    if (String.IsNullOrEmpty(hr.icon))
                    {
                        if (time.location.symbol != null)
                        {
                            hr.condition = time.location.symbol.id;
                            hr.icon = time.location.symbol.number.ToString();
                        }
                    }
                }

                if (fcast != null && fcast.date.Equals(time.from) && time.to.Subtract(time.from).TotalHours >= 1)
                {
                    if (time.location.symbol != null)
                    {
                        fcast.condition = time.location.symbol.id;
                        fcast.icon = time.location.symbol.number.ToString();
                    }
                }
                else if (forecast.Count > 0 && forecast.Last().date.Equals(time.from) && time.to.Subtract(time.from).TotalHours >= 1)
                {
                    if (String.IsNullOrEmpty(forecast.Last().icon))
                    {
                        if (time.location.symbol != null)
                        {
                            forecast.Last().condition = time.location.symbol.id;
                            forecast.Last().icon = time.location.symbol.number.ToString();
                        }
                    }
                }

                if (!conditionSet && condition != null && date.Equals(startDate) && time.to.Subtract(time.from).TotalHours >= 2)
                {
                    // Set condition from id
                    if (time.location.symbol != null)
                    {
                        condition.icon = time.location.symbol.number.ToString();
                        condition.weather = time.location.symbol.id;
                        if (time.location.maxTemperature?.value != null && time.location.minTemperature?.value != null)
                        {
                            condition.high_f = float.Parse(ConversionMethods.CtoF(time.location.maxTemperature.value.ToString(CultureInfo.InvariantCulture)));
                            condition.high_c = (float) Math.Round(time.location.maxTemperature.value);
                            condition.low_f = float.Parse(ConversionMethods.CtoF(time.location.minTemperature.value.ToString(CultureInfo.InvariantCulture)));
                            condition.low_c = (float) Math.Round(time.location.minTemperature.value);
                        }
                    }

                    conditionSet = true;
                }
            }

            fcast = forecast.Last();
            if (fcast?.condition == null && fcast?.icon == null)
            {
                forecast.RemoveAt(forecast.Count - 1);
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

            source = WeatherAPI.MetNo;
        }

        public Weather(HERE.Rootobject root)
        {
            var now = root.feedCreation;

            location = new Location(root.observations.location[0]);
            update_time = root.feedCreation;
            forecast = new List<Forecast>(root.dailyForecasts.forecastLocation.forecast.Length);
            for (int i = 0; i < root.dailyForecasts.forecastLocation.forecast.Length; i++)
            {
                forecast.Add(new Forecast(root.dailyForecasts.forecastLocation.forecast[i]));
            }
            hr_forecast = new List<HourlyForecast>(root.hourlyForecasts.forecastLocation.forecast.Length);
            foreach (HERE.Forecast1 forecast1 in root.hourlyForecasts.forecastLocation.forecast)
            {
                if (forecast1.utcTime.UtcDateTime < now.UtcDateTime)
                    continue;

                hr_forecast.Add(new HourlyForecast(forecast1));
            }
            txt_forecast = new List<TextForecast>(root.dailyForecasts.forecastLocation.forecast.Length);
            for (int i = 0; i < root.dailyForecasts.forecastLocation.forecast.Length; i++)
            {
                txt_forecast.Add(new TextForecast(root.dailyForecasts.forecastLocation.forecast[i]));
            }
            condition = new Condition(root.observations.location[0].observation[0], root.dailyForecasts.forecastLocation.forecast[0]);
            atmosphere = new Atmosphere(root.observations.location[0].observation[0]);
            astronomy = new Astronomy(root.astronomy.astronomy);
            precipitation = new Precipitation(root.dailyForecasts.forecastLocation.forecast[0]);
            ttl = "180";

            source = WeatherAPI.Here;
        }

        public Weather(NWS.PointsRootobject pointsRootobject, NWS.ForecastRootobject forecastRootobject,
            NWS.ForecastRootobject hourlyForecastRootobject, NWS.ObservationsCurrentRootobject obsCurrentRootObject)
        {
            location = new Location(pointsRootobject);
            update_time = DateTimeOffset.UtcNow;

            // ~8-day forecast
            forecast = new List<Forecast>(8);
            txt_forecast = new List<TextForecast>(16);

            for (int i = 0; i < forecastRootobject.periods.Length; i++)
            {
                NWS.Period forecastItem = forecastRootobject.periods[i];

                if (forecast.Count == 0 && !forecastItem.isDaytime)
                    continue;

                if (forecastItem.isDaytime && (i + 1) < forecastRootobject.periods.Length)
                {
                    NWS.Period ntForecastItem = forecastRootobject.periods[i + 1];
                    forecast.Add(new Forecast(forecastItem, ntForecastItem));

                    txt_forecast.Add(new TextForecast(forecastItem));
                    txt_forecast.Add(new TextForecast(ntForecastItem));

                    i++;
                }
            }
            if (hourlyForecastRootobject != null)
            {
                hr_forecast = new List<HourlyForecast>(hourlyForecastRootobject.periods.Length);
                for (int i = 0; i < hourlyForecastRootobject.periods.Length; i++)
                {
                    hr_forecast.Add(new HourlyForecast(hourlyForecastRootobject.periods[i]));
                }
            }
            condition = new Condition(obsCurrentRootObject);
            atmosphere = new Atmosphere(obsCurrentRootObject);
            //astronomy = new Astronomy(obsCurrentRootObject);
            //precipitation = new Precipitation(obsCurrentRootObject);
            ttl = "180";

            if (condition.high_f == condition.high_c && forecast.Count > 0)
            {
                condition.high_f = float.Parse(forecast[0].high_f);
                condition.high_c = float.Parse(forecast[0].high_c);
                condition.low_f = float.Parse(forecast[0].low_f);
                condition.low_c = float.Parse(forecast[0].low_c);
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
            latitude = location.lat.ToString(CultureInfo.InvariantCulture);
            longitude = location._long.ToString(CultureInfo.InvariantCulture);
            tz_offset = TimeSpan.Zero;
            tz_short = "UTC";
            tz_long = location.timezone_id;
        }

        public Location(OpenWeather.ForecastRootobject root)
        {
            // Use location name from location provider
            name = null;
            latitude = root.city.coord.lat.ToString(CultureInfo.InvariantCulture);
            longitude = root.city.coord.lon.ToString(CultureInfo.InvariantCulture);
            tz_offset = TimeSpan.Zero;
            tz_short = "UTC";
        }

        public Location(Metno.weatherdata foreRoot)
        {
            // API doesn't provide location name (at all)
            name = null;
            latitude = foreRoot.product.time.First().location.latitude.ToString(CultureInfo.InvariantCulture);
            longitude = foreRoot.product.time.First().location.longitude.ToString(CultureInfo.InvariantCulture);
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

        public Forecast(OpenWeather.List forecast)
        {
            date = DateTimeOffset.FromUnixTimeSeconds(forecast.dt).DateTime;
            high_f = ConversionMethods.KtoF(forecast.main.temp_max.ToString(CultureInfo.InvariantCulture));
            high_c = ConversionMethods.KtoC(forecast.main.temp_max.ToString(CultureInfo.InvariantCulture));
            low_f = ConversionMethods.KtoF(forecast.main.temp_min.ToString(CultureInfo.InvariantCulture));
            low_c = ConversionMethods.KtoC(forecast.main.temp_min.ToString(CultureInfo.InvariantCulture));
            condition = forecast.weather[0].description.ToUpperCase();
            icon = WeatherManager.GetProvider(WeatherAPI.OpenWeatherMap)
                   .GetWeatherIcon(forecast.weather[0].id.ToString());
        }

        public Forecast(Metno.weatherdataProductTime time)
        {
            date = time.from;
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
        public HourlyForecast(OpenWeather.List hr_forecast)
        {
            date = DateTimeOffset.FromUnixTimeSeconds(hr_forecast.dt);
            high_f = ConversionMethods.KtoF(hr_forecast.main.temp.ToString(CultureInfo.InvariantCulture));
            high_c = ConversionMethods.KtoC(hr_forecast.main.temp.ToString(CultureInfo.InvariantCulture));
            condition = hr_forecast.weather[0].description.ToUpperCase();

            // Use icon to determine if day or night
            string ico = hr_forecast.weather[0].icon;
            string dn = ico.Last().ToString();

            if (int.TryParse(dn, out int x))
                dn = String.Empty;

            icon = WeatherManager.GetProvider(WeatherAPI.OpenWeatherMap)
                   .GetWeatherIcon(hr_forecast.weather[0].id.ToString() + dn);

            // Use cloudiness value here
            pop = hr_forecast.clouds.all.ToString();
            wind_degrees = (int)hr_forecast.wind.deg;
            wind_mph = (float)Math.Round(double.Parse(ConversionMethods.MSecToMph(hr_forecast.wind.speed.ToString())));
            wind_kph = (float)Math.Round(double.Parse(ConversionMethods.MSecToKph(hr_forecast.wind.speed.ToString())));

            // Extras
            extras = new ForecastExtras();
            extras.feelslike_f = float.Parse(WeatherUtils.GetFeelsLikeTemp(high_f, wind_mph.ToString(CultureInfo.InvariantCulture), hr_forecast.main.humidity));
            extras.feelslike_c = float.Parse(ConversionMethods.FtoC(extras.feelslike_f.ToString(CultureInfo.InvariantCulture)));
            extras.humidity = hr_forecast.main.humidity;
            extras.pop = pop;
            if (hr_forecast.rain != null)
            {
                extras.qpf_rain_in = float.Parse(ConversionMethods.MMToIn(hr_forecast.rain._3h.ToString(CultureInfo.InvariantCulture)));
                extras.qpf_rain_mm = hr_forecast.rain._3h;
            }
            if (hr_forecast.snow != null)
            {
                extras.qpf_snow_in = float.Parse(ConversionMethods.MMToIn(hr_forecast.snow._3h.ToString(CultureInfo.InvariantCulture)));
                extras.qpf_snow_cm = hr_forecast.snow._3h / 10;
            }
            extras.pressure_in = ConversionMethods.MBToInHg(hr_forecast.main.pressure.ToString(CultureInfo.InvariantCulture));
            extras.pressure_mb = hr_forecast.main.pressure.ToString(CultureInfo.InvariantCulture);
            extras.wind_degrees = wind_degrees;
            extras.wind_mph = wind_mph;
            extras.wind_kph = wind_kph;
        }

        public HourlyForecast(Metno.weatherdataProductTime hr_forecast)
        {
            date = new DateTimeOffset(hr_forecast.from, TimeSpan.Zero);
            high_f = ConversionMethods.CtoF(hr_forecast.location.temperature.value.ToString(CultureInfo.InvariantCulture));
            high_c = hr_forecast.location.temperature.value.ToString();
            //condition = hr_forecast.weather[0].main;
            //icon = hr_forecast.weather[0].id.ToString();
            // Use cloudiness value here
            pop = ((int)Math.Round(hr_forecast.location.cloudiness.percent)).ToString();
            wind_degrees = (int)Math.Round(hr_forecast.location.windDirection.deg);
            wind_mph = (float)Math.Round(double.Parse(ConversionMethods.MSecToMph(hr_forecast.location.windSpeed.mps.ToString())));
            wind_kph = (float)Math.Round(double.Parse(ConversionMethods.MSecToKph(hr_forecast.location.windSpeed.mps.ToString())));

            // Extras
            extras = new ForecastExtras()
            {
                feelslike_f = float.Parse(WeatherUtils.GetFeelsLikeTemp(high_f, wind_mph.ToString(CultureInfo.InvariantCulture), Math.Round(hr_forecast.location.humidity.value).ToString(CultureInfo.InvariantCulture))),
                feelslike_c = float.Parse(ConversionMethods.FtoC(WeatherUtils.GetFeelsLikeTemp(high_f, wind_mph.ToString(CultureInfo.InvariantCulture), Math.Round(hr_forecast.location.humidity.value).ToString(CultureInfo.InvariantCulture)))),
                humidity = Math.Round(hr_forecast.location.humidity.value).ToString(CultureInfo.InvariantCulture),
                dewpoint_f = ConversionMethods.CtoF(hr_forecast.location.dewpointTemperature.value.ToString(CultureInfo.InvariantCulture)),
                dewpoint_c = hr_forecast.location.dewpointTemperature.value.ToString(CultureInfo.InvariantCulture),
                pop = pop,
                pressure_in = ConversionMethods.MBToInHg(hr_forecast.location.pressure.value.ToString(CultureInfo.InvariantCulture)),
                pressure_mb = hr_forecast.location.pressure.value.ToString(CultureInfo.InvariantCulture),
                wind_degrees = wind_degrees,
                wind_mph = wind_mph,
                wind_kph = wind_kph
            };
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
        public TextForecast(HERE.Forecast forecast)
        {
            title = forecast.weekday;

            String fctxt = String.Format("{0} {1} {2}: {3}",
                forecast.description.ToPascalCase(), forecast.beaufortDescription.ToPascalCase(),
                SimpleLibrary.ResLoader.GetString("Label_Humidity/Text"),
                forecast.humidity + "%");

            fcttext = String.Format("{0} {1} {2}F. {3} {4}F. {5} {6} {7}mph",
                fctxt,
                SimpleLibrary.ResLoader.GetString("Label_High"),
                Math.Round(double.Parse(forecast.highTemperature)),
                SimpleLibrary.ResLoader.GetString("Label_Low"),
                Math.Round(double.Parse(forecast.lowTemperature)),
                SimpleLibrary.ResLoader.GetString("Label_Wind/Text"),
                forecast.windDesc, Math.Round(double.Parse(forecast.windSpeed)));

            fcttext_metric = String.Format("{0} {1} {2}C. {3} {4}C. {5} {6} {7}kph",
                fctxt,
                SimpleLibrary.ResLoader.GetString("Label_High"),
                ConversionMethods.FtoC(forecast.highTemperature),
                SimpleLibrary.ResLoader.GetString("Label_Low"),
                ConversionMethods.FtoC(forecast.lowTemperature),
                SimpleLibrary.ResLoader.GetString("Label_Wind/Text"),
                forecast.windDesc, Math.Round(double.Parse(ConversionMethods.MphToKph(forecast.windSpeed))));

            icon = WeatherManager.GetProvider(WeatherAPI.Here)
                   .GetWeatherIcon(string.Format("{0}_{1}", forecast.daylight, forecast.iconName));

            pop = forecast.precipitationProbability;
        }

        public TextForecast(NWS.Period forecastItem)
        {
            title = forecastItem.name;
            fcttext = forecastItem.detailedForecast;
            fcttext_metric = forecastItem.detailedForecast;
            icon = WeatherManager.GetProvider(WeatherAPI.NWS)
                        .GetWeatherIcon(forecastItem.icon);
            pop = null;
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

        public Condition(OpenWeather.CurrentRootobject root)
        {
            weather = root.weather[0].description.ToUpperCase();
            temp_f = float.Parse(ConversionMethods.KtoF(root.main.temp.ToString(CultureInfo.InvariantCulture)));
            temp_c = float.Parse(ConversionMethods.KtoC(root.main.temp.ToString(CultureInfo.InvariantCulture)));
            high_f = float.Parse(ConversionMethods.KtoF(root.main.temp_max.ToString(CultureInfo.InvariantCulture)));
            high_c = float.Parse(ConversionMethods.KtoC(root.main.temp_max.ToString(CultureInfo.InvariantCulture)));
            low_f = float.Parse(ConversionMethods.KtoF(root.main.temp_min.ToString(CultureInfo.InvariantCulture)));
            low_c = float.Parse(ConversionMethods.KtoC(root.main.temp_min.ToString(CultureInfo.InvariantCulture)));
            wind_degrees = (int)root.wind.deg;
            wind_mph = float.Parse(ConversionMethods.MSecToMph(root.wind.speed.ToString(CultureInfo.InvariantCulture)));
            wind_kph = float.Parse(ConversionMethods.MSecToKph(root.wind.speed.ToString(CultureInfo.InvariantCulture)));
            // This will be calculated after with formula
            feelslike_f = temp_f;
            feelslike_c = temp_c;

            string ico = root.weather[0].icon;
            string dn = ico.Last().ToString();

            if (int.TryParse(dn, out int x))
                dn = String.Empty;

            icon = WeatherManager.GetProvider(WeatherAPI.OpenWeatherMap)
                   .GetWeatherIcon(root.weather[0].id.ToString() + dn);
        }

        public Condition(Metno.weatherdataProductTime time)
        {
            // weather
            temp_f = float.Parse(ConversionMethods.CtoF(time.location.temperature.value.ToString()));
            temp_c = (float)time.location.temperature.value;
            wind_degrees = (int)Math.Round(time.location.windDirection.deg);
            wind_mph = (float)Math.Round(double.Parse(ConversionMethods.MSecToMph(time.location.windSpeed.mps.ToString())));
            wind_kph = (float)Math.Round(double.Parse(ConversionMethods.MSecToKph(time.location.windSpeed.mps.ToString())));
            // This will be calculated after with formula
            feelslike_f = temp_f;
            feelslike_c = temp_c;
            // icon
            beaufort = new Beaufort(time.location.windSpeed.beaufort);
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
            wind_degrees = (int)obsCurrentRootObject.windDirection.value.GetValueOrDefault(0);

            if (obsCurrentRootObject.windSpeed.value.HasValue)
            {
                wind_mph = float.Parse(ConversionMethods.MSecToMph(obsCurrentRootObject.windSpeed.value.GetValueOrDefault(0.00f).ToString()));
                wind_kph = float.Parse(ConversionMethods.MSecToKph(obsCurrentRootObject.windSpeed.value.GetValueOrDefault(0.00f).ToString()));
            }
            else
            {
                wind_mph = -1.00f;
                wind_kph = -1.00f;
            }

            float humidity = obsCurrentRootObject.relativeHumidity.value.GetValueOrDefault(-1.0f);
            if (temp_f != temp_c)
            {
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

        public Atmosphere(OpenWeather.CurrentRootobject root)
        {
            humidity = root.main.humidity;
            pressure_mb = root.main.pressure.ToString(CultureInfo.InvariantCulture);
            pressure_in = ConversionMethods.MBToInHg(root.main.pressure.ToString(CultureInfo.InvariantCulture));
            pressure_trend = String.Empty;
            visibility_mi = ConversionMethods.KmToMi((root.visibility / 1000).ToString());
            visibility_km = (root.visibility / 1000).ToString();
        }

        public Atmosphere(Metno.weatherdataProductTime time)
        {
            humidity = Math.Round(time.location.humidity.value).ToString();
            pressure_mb = time.location.pressure.value.ToString(CultureInfo.InvariantCulture);
            pressure_in = ConversionMethods.MBToInHg(time.location.pressure.value.ToString(CultureInfo.InvariantCulture));
            pressure_trend = String.Empty;

            try
            {
                float visMi = 10.0f;
                visibility_mi = (visMi - (visMi * (float)time.location.fog.percent / 100)).ToString(CultureInfo.InvariantCulture);
                visibility_km = ConversionMethods.MiToKm(visibility_mi);
            }
            catch (FormatException)
            {
                visibility_mi = Weather.NA;
                visibility_km = Weather.NA;
            }

            try
            {
                dewpoint_f = ConversionMethods.CtoF(time.location.dewpointTemperature.value.ToString(CultureInfo.InvariantCulture));
                dewpoint_c = ((float)time.location.dewpointTemperature.value).ToString(CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                dewpoint_f = null;
                dewpoint_c = null;
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

        public Astronomy(OpenWeather.CurrentRootobject root)
        {
            try
            {
                sunrise = DateTimeOffset.FromUnixTimeSeconds(root.sys.sunrise).UtcDateTime;
            }
            catch (Exception) { }
            try
            {
                sunset = DateTimeOffset.FromUnixTimeSeconds(root.sys.sunset).UtcDateTime;
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

        public Astronomy(Metno.astrodata astroRoot)
        {
            int moonPhaseValue = -1;

            foreach (Metno.astrodataLocationTime time in astroRoot.location.time)
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
                    moonPhaseValue = (int)Math.Round(time.moonphase.value);
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
        public Precipitation(OpenWeather.CurrentRootobject root)
        {
            // Use cloudiness value here
            pop = root.clouds.all.ToString();
            if (root.rain != null)
            {
                qpf_rain_in = float.Parse(ConversionMethods.MMToIn(root.rain._3h.ToString(CultureInfo.InvariantCulture)));
                qpf_rain_mm = root.rain._3h;
            }
            if (root.snow != null)
            {
                qpf_snow_in = float.Parse(ConversionMethods.MMToIn(root.snow._3h.ToString(CultureInfo.InvariantCulture)));
                qpf_snow_cm = root.snow._3h;
            }
        }

        public Precipitation(Metno.weatherdataProductTime time)
        {
            // Use cloudiness value here
            pop = Math.Round(time.location.cloudiness.percent).ToString();
            // The rest DNE
        }

        public Precipitation(HERE.Forecast forecast)
        {
            pop = forecast.precipitationProbability;

            if (float.TryParse(forecast.rainFall, NumberStyles.Float, CultureInfo.InvariantCulture, out float rain_in))
                qpf_rain_in = rain_in;
            else
                qpf_rain_in = 0.00f;

            qpf_rain_mm = float.Parse(ConversionMethods.InToMM(qpf_rain_in.ToString(CultureInfo.InvariantCulture)));

            if (float.TryParse(forecast.snowFall, NumberStyles.Float, CultureInfo.InvariantCulture, out float snow_in))
                qpf_snow_in = snow_in;
            else
                qpf_snow_in = 0.00f;

            qpf_snow_cm = float.Parse(ConversionMethods.InToMM(qpf_snow_in.ToString(CultureInfo.InvariantCulture))) / 10;
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