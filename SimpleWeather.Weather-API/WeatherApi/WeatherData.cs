using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace SimpleWeather.Weather_API.WeatherApi
{
    public static partial class WeatherApiProviderExtensions
    {
        public static Weather CreateWeatherData(this WeatherApiProvider _, ForecastRootobject root)
        {
            var weather = new Weather();

            var offset = DateTimeUtils.TzidToOffset(root.location.tz_id);

            weather.location = _.CreateLocation(root.location);
            weather.update_time = DateTimeOffset.FromUnixTimeSeconds(root.location.localtime_epoch).ToOffset(offset);

            weather.forecast = new List<SimpleWeather.WeatherData.Forecast>(root.forecast.forecastday.Length);
            weather.hr_forecast = new List<HourlyForecast>(root.forecast.forecastday[0].hour.Length);

            // Forecast
            foreach (var day in root.forecast.forecastday)
            {
                var fcast = _.CreateForecast(day);

                foreach (var hour in day.hour)
                {
                    var date = DateTimeOffset.FromUnixTimeSeconds(hour.time_epoch);

                    if (date.UtcDateTime < weather.update_time.UtcDateTime.Trim(TimeSpan.TicksPerHour))
                    {
                        continue;
                    }

                    weather.hr_forecast.Add(_.CreateHourlyForecast(hour, offset));
                }

                weather.forecast.Add(fcast);
            }

            weather.condition = _.CreateCondition(root.current, offset);
            weather.atmosphere = _.CreateAtmosphere(root.current);
            if (root.forecast.forecastday[0].date == weather.condition.observation_time.Date.ToString("yyyy-MM-dd"))
            {
                weather.astronomy = _.CreateAstronomy(root.forecast.forecastday[0].astro);
            }
            weather.precipitation = _.CreatePrecipitation(root.current);
            weather.ttl = 180;

            if ((!weather.condition.high_f.HasValue || !weather.condition.high_c.HasValue) && weather.forecast.Count > 0)
            {
                weather.condition.high_f = weather.forecast[0].high_f;
                weather.condition.high_c = weather.forecast[0].high_c;
                weather.condition.low_f = weather.forecast[0].low_f;
                weather.condition.low_c = weather.forecast[0].low_c;
            }

            weather.weather_alerts = _.CreateWeatherAlerts(root.alerts);

            weather.source = WeatherAPI.WeatherApi;

            return weather;
        }

        public static SimpleWeather.WeatherData.Location CreateLocation(this WeatherApiProvider _, Weather_API.WeatherApi.Location location)
        {
            return new SimpleWeather.WeatherData.Location()
            {
                /* Use name from location provider */
                name = null,
                latitude = location.lat,
                longitude = location.lon,
                tz_long = location.tz_id,
            };
        }

        public static SimpleWeather.WeatherData.Forecast CreateForecast(this WeatherApiProvider _, Forecastday day)
        {
            var forecast = new SimpleWeather.WeatherData.Forecast();

            forecast.date = DateTime.ParseExact(day.date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

            forecast.high_f = day.day.maxtemp_f;
            forecast.high_c = day.day.maxtemp_c;
            forecast.low_f = day.day.mintemp_f;
            forecast.low_c = day.day.mintemp_c;

            forecast.condition = day.day.condition.text;
            forecast.icon = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.WeatherApi)
                .GetWeatherIcon(day.day.condition.code?.ToInvariantString());

            // Extras
            forecast.extras = new ForecastExtras();
            forecast.extras.feelslike_f = WeatherUtils.GetFeelsLikeTemp(day.day.avgtemp_f, day.day.maxwind_mph, (int)MathF.Round(day.day.avghumidity));
            forecast.extras.feelslike_c = ConversionMethods.FtoC(forecast.extras.feelslike_f.Value);
            forecast.extras.humidity = (int)MathF.Round(day.day.avghumidity);
            forecast.extras.dewpoint_c = WeatherUtils.CalculateDewpointC(day.day.avgtemp_c, forecast.extras.humidity.Value);
            forecast.extras.dewpoint_f = ConversionMethods.CtoF(forecast.extras.dewpoint_c.Value);
            forecast.extras.uv_index = day.day.uv;
            forecast.extras.pop = day.day.daily_chance_of_rain ?? day.day.daily_chance_of_snow;
            forecast.extras.qpf_rain_mm = day.day.totalprecip_mm;
            forecast.extras.qpf_rain_in = day.day.totalprecip_in;
            forecast.extras.qpf_snow_cm = day.day.totalsnow_cm;
            forecast.extras.qpf_snow_in = day.day.totalsnow_cm?.Let(it => ConversionMethods.MMToIn(it * 10));
            forecast.extras.wind_mph = day.day.maxwind_mph;
            forecast.extras.wind_kph = day.day.maxwind_kph;
            forecast.extras.visibility_mi = day.day.avgvis_miles;
            forecast.extras.visibility_km = day.day.avgvis_km;

            return forecast;
        }

        public static HourlyForecast CreateHourlyForecast(this WeatherApiProvider _, Hour hour, TimeSpan offset)
        {
            return new HourlyForecast
            {
                date = DateTimeOffset.FromUnixTimeSeconds(hour.time_epoch).ToOffset(offset),

                high_f = hour.temp_f,
                high_c = hour.temp_c,

                condition = hour.condition.text,
                icon = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.WeatherApi)
                    .GetWeatherIcon(hour.is_day == 0, hour.condition.code?.ToInvariantString()),

                wind_mph = hour.wind_mph,
                wind_kph = hour.wind_kph,
                wind_degrees = hour.wind_degree,

                // Extras
                extras = new ForecastExtras
                {
                    feelslike_f = hour.feelslike_f,
                    feelslike_c = hour.feelslike_c,
                    humidity = hour.humidity,
                    dewpoint_f = hour.dewpoint_f,
                    dewpoint_c = hour.dewpoint_c,
                    uv_index = hour.uv,
                    pop = hour.chance_of_rain ?? hour.chance_of_snow,
                    cloudiness = hour.cloud,
                    qpf_rain_in = hour.precip_in,
                    qpf_rain_mm = hour.precip_mm,
                    qpf_snow_cm = hour.snow_cm,
                    qpf_snow_in = hour.snow_cm?.Let(it => ConversionMethods.MMToIn(it * 10)),
                    pressure_in = hour.pressure_in,
                    pressure_mb = hour.pressure_mb,
                    wind_degrees = hour.wind_degree,
                    wind_mph = hour.wind_mph,
                    wind_kph = hour.wind_kph,
                    visibility_mi = hour.vis_miles,
                    visibility_km = hour.vis_km,
                    windgust_mph = hour.gust_mph,
                    windgust_kph = hour.gust_kph
                }
            };
        }

        public static SimpleWeather.WeatherData.Condition CreateCondition(this WeatherApiProvider _, Current current, TimeSpan offset)
        {
            return new SimpleWeather.WeatherData.Condition()
            {
                weather = current.condition.text,

                temp_f = current.temp_f,
                temp_c = current.temp_c,

                wind_degrees = current.wind_degree,
                wind_mph = current.wind_mph,
                wind_kph = current.wind_kph,

                windgust_mph = current.gust_mph,
                windgust_kph = current.gust_kph,

                feelslike_f = current.feelslike_f,
                feelslike_c = current.feelslike_c,

                icon = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.WeatherApi)
                    .GetWeatherIcon(current.is_day == 0, current.condition.code?.ToInvariantString()),

                beaufort = new Beaufort(WeatherUtils.GetBeaufortScale((int)current.wind_mph)),
                uv = new UV(current.uv),
                airQuality = _.CreateAirQuality(current.air_quality),
                observation_time = DateTimeOffset.FromUnixTimeSeconds(current.last_updated_epoch).ToOffset(offset),
            };
        }

        public static Atmosphere CreateAtmosphere(this WeatherApiProvider _, Current current)
        {
            return new Atmosphere()
            {
                humidity = current.humidity,

                pressure_mb = current.pressure_mb,
                pressure_in = current.pressure_in,
                pressure_trend = string.Empty,

                visibility_mi = current.vis_miles,
                visibility_km = current.vis_km,
            };
        }

        public static Astronomy CreateAstronomy(this WeatherApiProvider _, Astro astro)
        {
            var astronomy = new Astronomy();

            try
            {
                astronomy.sunrise = DateTime.ParseExact(astro.sunrise, "hh:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            }
            catch (Exception) { }

            try
            {
                astronomy.sunset = DateTime.ParseExact(astro.sunset, "hh:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

                if (astronomy.sunrise != null && astronomy.sunrise != DateTime.MinValue && astronomy.sunset.CompareTo(astronomy.sunrise) < 0)
                {
                    // Is next day
                    astronomy.sunset = astronomy.sunset.AddDays(1);
                }
            }
            catch (Exception) { }

            try
            {
                astronomy.moonrise = DateTime.ParseExact(astro.moonrise, "hh:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            }
            catch (Exception) { }

            try
            {
                astronomy.moonset = DateTime.ParseExact(astro.moonset, "hh:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            }
            catch (Exception) { }

            astronomy.moonphase = astro.moon_phase switch
            {
                "New Moon" => new MoonPhase(MoonPhase.MoonPhaseType.NewMoon),
                "Waxing Crescent" => new MoonPhase(MoonPhase.MoonPhaseType.WaxingCrescent),
                "First Quarter" => new MoonPhase(MoonPhase.MoonPhaseType.FirstQtr),
                "Waxing Gibbous" => new MoonPhase(MoonPhase.MoonPhaseType.WaxingGibbous),
                "Full Moon" => new MoonPhase(MoonPhase.MoonPhaseType.FullMoon),
                "Waning Gibbous" => new MoonPhase(MoonPhase.MoonPhaseType.WaningGibbous),
                "Last Quarter" => new MoonPhase(MoonPhase.MoonPhaseType.LastQtr),
                "Waning Crescent" => new MoonPhase(MoonPhase.MoonPhaseType.WaningCrescent),
                _ => new MoonPhase(MoonPhase.MoonPhaseType.NewMoon),
            };

            // If the sun won't set/rise, set time to the future
            if (astronomy.sunrise == null)
            {
                astronomy.sunrise = DateTime.Now.Date.AddYears(1).AddTicks(-1);
            }
            if (astronomy.sunset == null)
            {
                astronomy.sunset = DateTime.Now.Date.AddYears(1).AddTicks(-1);
            }
            if (astronomy.moonrise == null)
            {
                astronomy.moonrise = DateTime.MinValue;
            }
            if (astronomy.moonset == null)
            {
                astronomy.moonset = DateTime.MinValue;
            }

            return astronomy;
        }

        public static Precipitation CreatePrecipitation(this WeatherApiProvider _, Current current)
        {
            return new Precipitation()
            {
                cloudiness = current.cloud,

                qpf_rain_in = current.precip_in,
                qpf_rain_mm = current.precip_mm,
            };
        }

        public static AirQuality CreateAirQuality(this WeatherApiProvider _, Air_Quality airQuality)
        {
            if (airQuality == null) return null;

            return new AirQuality()
            {
                co = airQuality.co?.Let(it => _.RunCatching(() => AirQualityUtils.AQICO(AirQualityUtils.CO_ugm3_TO_ppm(it))).GetOrNull()),
                no2 = airQuality.no2?.Let(it => _.RunCatching(() => AirQualityUtils.AQINO2(AirQualityUtils.NO2_ugm3_to_ppb(it))).GetOrNull()),
                o3 = airQuality.o3?.Let(it => _.RunCatching(() => AirQualityUtils.AQIO3(AirQualityUtils.O3_ugm3_to_ppb(it))).GetOrNull()),
                so2 = airQuality.so2?.Let(it => _.RunCatching(() => AirQualityUtils.AQISO2(AirQualityUtils.SO2_ugm3_to_ppb(it))).GetOrNull()),
                pm25 = airQuality.pm2_5?.Let(it => _.RunCatching(() => AirQualityUtils.AQIPM2_5(it)).GetOrNull()),
                pm10 = airQuality.pm10?.Let(it => _.RunCatching(() => AirQualityUtils.AQIPM10(it)).GetOrNull()),
            }.Apply(aqi => aqi.index = aqi.GetIndexFromData());
        }
    }
}