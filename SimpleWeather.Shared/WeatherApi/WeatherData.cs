using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SimpleWeather.WeatherData
{
    public partial class Weather
    {
        public Weather(WeatherApi.ForecastRootobject root)
        {
            var offset = DateTimeUtils.TzidToOffset(root.location.tz_id);

            location = new Location(root.location);
            update_time = DateTimeOffset.FromUnixTimeSeconds(root.location.localtime_epoch).ToOffset(offset);

            forecast = new List<Forecast>(root.forecast.forecastday.Length);
            hr_forecast = new List<HourlyForecast>(root.forecast.forecastday[0].hour.Length);

            // Forecast
            foreach (var day in root.forecast.forecastday)
            {
                var fcast = new Forecast(day);

                foreach (var hour in day.hour)
                {
                    var date = DateTimeOffset.FromUnixTimeSeconds(hour.time_epoch);

                    if (date.UtcDateTime < update_time.UtcDateTime.Trim(TimeSpan.TicksPerHour))
                    {
                        continue;
                    }

                    hr_forecast.Add(new HourlyForecast(hour, offset));
                }

                forecast.Add(fcast);
            }

            condition = new Condition(root.current, offset);
            atmosphere = new Atmosphere(root.current);
            if (root.forecast.forecastday[0].date == condition.observation_time.Date.ToString("yyyy-MM-dd"))
            {
                astronomy = new Astronomy(root.forecast.forecastday[0].astro);
            }
            precipitation = new Precipitation(root.current);
            ttl = 180;

            if ((!condition.high_f.HasValue || !condition.high_c.HasValue) && forecast.Count > 0)
            {
                condition.high_f = forecast[0].high_f;
                condition.high_c = forecast[0].high_c;
                condition.low_f = forecast[0].low_f;
                condition.low_c = forecast[0].low_c;
            }

            if (root.alerts?.alert?.Length > 0)
            {
                weather_alerts = new List<WeatherAlert>(root.alerts.alert.Length);

                foreach (var alert in root.alerts.alert)
                {
                    weather_alerts.Add(new WeatherAlert(alert));
                }
            }

            source = WeatherAPI.WeatherApi;
        }
    }

    public partial class Location
    {
        public Location(WeatherApi.Location location)
        {
            /* Use name from location provider */
            name = null;
            latitude = location.lat;
            longitude = location.lon;
            tz_long = location.tz_id;
        }
    }

    public partial class Forecast
    {
        public Forecast(WeatherApi.Forecastday day)
        {
            date = DateTime.ParseExact(day.date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

            high_f = day.day.maxtemp_f;
            high_c = day.day.maxtemp_c;
            low_f = day.day.mintemp_f;
            low_c = day.day.mintemp_c;

            condition = day.day.condition.text;
            icon = WeatherManager.GetProvider(WeatherAPI.WeatherApi)
                .GetWeatherIcon(day.day.condition.code?.ToInvariantString());

            // Extras
            extras = new ForecastExtras();
            extras.feelslike_f = WeatherUtils.GetFeelsLikeTemp(day.day.avgtemp_f, day.day.maxwind_mph, (int)MathF.Round(day.day.avghumidity));
            extras.feelslike_c = ConversionMethods.FtoC(extras.feelslike_f.Value);
            extras.humidity = (int)MathF.Round(day.day.avghumidity);
            extras.dewpoint_c = MathF.Round(WeatherUtils.CalculateDewpointC(day.day.avgtemp_c, extras.humidity.Value));
            extras.dewpoint_f = MathF.Round(ConversionMethods.CtoF(extras.dewpoint_c.Value));
            extras.uv_index = day.day.uv;
            if (day.day.daily_chance_of_rain.HasValue)
            {
                extras.pop = day.day.daily_chance_of_rain;
            }
            else
            {
                extras.pop = day.day.daily_chance_of_snow;
            }
            extras.qpf_rain_mm = day.day.totalprecip_mm;
            extras.qpf_rain_in = day.day.totalprecip_in;
            extras.wind_mph = day.day.maxwind_mph;
            extras.wind_kph = day.day.maxwind_kph;
            extras.visibility_mi = day.day.avgvis_miles;
            extras.visibility_km = day.day.avgvis_km;
        }
    }

    public partial class HourlyForecast
    {
        public HourlyForecast(WeatherApi.Hour hour, TimeSpan offset)
        {
            date = DateTimeOffset.FromUnixTimeSeconds(hour.time_epoch).ToOffset(offset);

            high_f = hour.temp_f;
            high_c = hour.temp_c;

            condition = hour.condition.text;
            icon = WeatherManager.GetProvider(WeatherAPI.WeatherApi)
                .GetWeatherIcon(hour.is_day == 0, hour.condition.code?.ToInvariantString());

            wind_mph = hour.wind_mph;
            wind_kph = hour.wind_kph;
            wind_degrees = hour.wind_degree;

            // Extras
            extras = new ForecastExtras();
            extras.feelslike_f = hour.feelslike_f;
            extras.feelslike_c = hour.feelslike_c;
            extras.humidity = hour.humidity;
            extras.dewpoint_f = hour.dewpoint_f;
            extras.dewpoint_c = hour.dewpoint_c;
            extras.uv_index = hour.uv;
            if (hour.chance_of_rain.HasValue)
            {
                extras.pop = hour.chance_of_rain;
            }
            else
            {
                extras.pop = hour.chance_of_snow;
            }
            extras.cloudiness = hour.cloud;
            extras.qpf_rain_in = hour.precip_in;
            extras.qpf_rain_mm = hour.precip_mm;
            extras.pressure_in = hour.pressure_in;
            extras.pressure_mb = hour.pressure_mb;
            extras.wind_degrees = wind_degrees;
            extras.wind_mph = wind_mph;
            extras.wind_kph = wind_kph;
            extras.visibility_mi = hour.vis_miles;
            extras.visibility_km = hour.vis_km;
            extras.windgust_mph = hour.gust_mph;
            extras.windgust_kph = hour.gust_kph;
        }
    }

    public partial class Condition
    {
        public Condition(WeatherApi.Current current, TimeSpan offset)
        {
            weather = current.condition.text;

            temp_f = current.temp_f;
            temp_c = current.temp_c;

            wind_degrees = current.wind_degree;
            wind_mph = current.wind_mph;
            wind_kph = current.wind_kph;

            windgust_mph = current.gust_mph;
            windgust_kph = current.gust_kph;

            feelslike_f = current.feelslike_f;
            feelslike_c = current.feelslike_c;

            icon = WeatherManager.GetProvider(WeatherAPI.WeatherApi)
                .GetWeatherIcon(current.is_day == 0, current.condition.code?.ToInvariantString());

            beaufort = new Beaufort(WeatherUtils.GetBeaufortScale((int)wind_mph));
            uv = new UV(current.uv);

            if (current.air_quality != null)
            {
                airQuality = new AirQuality(current.air_quality);
            }

            observation_time = DateTimeOffset.FromUnixTimeSeconds(current.last_updated_epoch).ToOffset(offset);
        }
    }

    public partial class Atmosphere
    {
        public Atmosphere(WeatherApi.Current current)
        {
            humidity = current.humidity;

            pressure_mb = current.pressure_mb;
            pressure_in = current.pressure_in;
            pressure_trend = string.Empty;

            visibility_mi = current.vis_miles;
            visibility_km = current.vis_km;
        }
    }

    public partial class Astronomy
    {
        public Astronomy(WeatherApi.Astro astro)
        {
            try
            {
                sunrise = DateTime.ParseExact(astro.sunrise, "hh:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            }
            catch (Exception) { }

            try
            {
                sunset = DateTime.ParseExact(astro.sunset, "hh:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            }
            catch (Exception) { }

            try
            {
                moonrise = DateTime.ParseExact(astro.moonrise, "hh:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            }
            catch (Exception) { }

            try
            {
                moonset = DateTime.ParseExact(astro.moonset, "hh:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            }
            catch (Exception) { }

            switch (astro.moon_phase)
            {
                case "New Moon":
                default:
                    moonphase = new MoonPhase(MoonPhase.MoonPhaseType.NewMoon);
                    break;

                case "Waxing Crescent":
                    moonphase = new MoonPhase(MoonPhase.MoonPhaseType.WaxingCrescent);
                    break;

                case "First Quarter":
                    moonphase = new MoonPhase(MoonPhase.MoonPhaseType.FirstQtr);
                    break;

                case "Waxing Gibbous":
                    moonphase = new MoonPhase(MoonPhase.MoonPhaseType.WaxingGibbous);
                    break;

                case "Full Moon":
                    moonphase = new MoonPhase(MoonPhase.MoonPhaseType.FullMoon);
                    break;

                case "Waning Gibbous":
                    moonphase = new MoonPhase(MoonPhase.MoonPhaseType.WaningGibbous);
                    break;

                case "Last Quarter":
                    moonphase = new MoonPhase(MoonPhase.MoonPhaseType.LastQtr);
                    break;

                case "Waning Crescent":
                    moonphase = new MoonPhase(MoonPhase.MoonPhaseType.WaningCrescent);
                    break;
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
        }
    }

    public partial class Precipitation
    {
        public Precipitation(WeatherApi.Current current)
        {
            cloudiness = current.cloud;

            qpf_rain_in = current.precip_in;
            qpf_rain_mm = current.precip_mm;
        }
    }

    public partial class AirQuality
    {
        public AirQuality(WeatherApi.Air_Quality airQuality)
        {
            var aqiData = new List<int>(6);

            // Convert
            if (airQuality.co.HasValue)
            {
                try
                {
                    aqiData.Add(AirQualityUtils.AQICO(AirQualityUtils.CO_ugm3_TO_ppm(airQuality.co.Value)));
                }
                catch (ArgumentOutOfRangeException) { aqiData.Add(-1); }
            }
            else
            {
                aqiData.Add(-1);
            }

            if (airQuality.no2.HasValue)
            {
                try
                {
                    aqiData.Add(AirQualityUtils.AQINO2(AirQualityUtils.NO2_ugm3_to_ppb(airQuality.no2.Value)));
                }
                catch (ArgumentOutOfRangeException) { aqiData.Add(-1); }
            }
            else
            {
                aqiData.Add(-1);
            }

            if (airQuality.o3.HasValue)
            {
                try
                {
                    aqiData.Add(AirQualityUtils.AQIO3(AirQualityUtils.O3_ugm3_to_ppb(airQuality.o3.Value)));
                }
                catch (ArgumentOutOfRangeException) { aqiData.Add(-1); }
            }
            else
            {
                aqiData.Add(-1);
            }

            if (airQuality.so2.HasValue)
            {
                try
                {
                    aqiData.Add(AirQualityUtils.AQISO2(AirQualityUtils.SO2_ugm3_to_ppb(airQuality.so2.Value)));
                }
                catch (ArgumentOutOfRangeException) { aqiData.Add(-1); }
            }
            else
            {
                aqiData.Add(-1);
            }

            if (airQuality.pm2_5.HasValue)
            {
                try
                {
                    aqiData.Add(AirQualityUtils.AQIPM2_5(airQuality.pm2_5.Value));
                }
                catch (ArgumentOutOfRangeException) { aqiData.Add(-1); }
            }
            else
            {
                aqiData.Add(-1);
            }

            if (airQuality.pm10.HasValue)
            {
                try
                {
                    aqiData.Add(AirQualityUtils.AQIPM10(airQuality.pm10.Value));
                }
                catch (ArgumentOutOfRangeException) { aqiData.Add(-1); }
            }
            else
            {
                aqiData.Add(-1);
            }

            var idx = aqiData.Max();

            if (idx >= 0)
            {
                index = idx;
            }
        }
    }
}