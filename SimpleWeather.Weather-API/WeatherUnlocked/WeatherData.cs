using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace SimpleWeather.Weather_API.WeatherUnlocked
{
    public static partial class WeatherUnlockedProviderExtensions
    {
        public static Weather CreateWeatherData(this WeatherUnlockedProvider _, CurrentRootobject currRoot, WeatherUnlocked.ForecastRootobject foreRoot)
        {
            var weather = new Weather();

            weather.location = _.CreateLocation(currRoot);
            weather.update_time = DateTimeOffset.UtcNow;

            // 8-day forecast / 3-hr forecast
            // 24hr / 3hr = 8items for each day
            weather.forecast = new List<Forecast>(8);
            weather.hr_forecast = new List<HourlyForecast>(64);

            // Forecast
            foreach (var day in foreRoot.Days)
            {
                Forecast fcast = _.CreateForecast(day);

                int midDayIdx = day.Timeframes.Length / 2;
                for (int i = 0; i < day.Timeframes.Length; i++)
                {
                    var hrfcast = _.CreateHourlyForecast(day.Timeframes[i]);

                    if (i == midDayIdx)
                    {
                        fcast.icon = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.WeatherUnlocked)
                            .GetWeatherIcon(hrfcast.icon);
                        fcast.condition = hrfcast.condition;
                    }

                    weather.hr_forecast.Add(hrfcast);
                }

                weather.forecast.Add(fcast);
            }

            weather.condition = _.CreateCondition(currRoot);
            weather.atmosphere = _.CreateAtmosphere(currRoot);
            //weather.astronomy = _.CreateAstronomy(currRoot);
            //weather.precipitation = _.CreatePrecipitation(currRoot);
            weather.ttl = 180;

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

            weather.condition.observation_time = weather.update_time;

            weather.source = WeatherAPI.WeatherUnlocked;

            return weather;
        }

        public static Location CreateLocation(this WeatherUnlockedProvider _, CurrentRootobject currRoot)
        {
            return new Location()
            {
                // Use location name from location provider
                name = null,
                latitude = currRoot.lat,
                longitude = currRoot.lon,
                tz_long = null,
            };
        }

        public static Forecast CreateForecast(this WeatherUnlockedProvider _, WeatherUnlocked.Day day)
        {
            var forecast = new Forecast();

            forecast.date = DateTime.ParseExact(day.date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            forecast.high_f = day.temp_max_f;
            forecast.high_c = day.temp_max_c;
            forecast.low_f = day.temp_min_f;
            forecast.low_c = day.temp_min_c;

            //forecast.condition = null;
            //forecast.icon = null;

            // Extras
            forecast.extras = new ForecastExtras();
            forecast.extras.humidity = (int)MathF.Round((day.humid_min_pct + day.humid_max_pct) / 2);
            forecast.extras.pressure_mb = (day.slp_min_mb + day.slp_max_mb) / 2f;
            forecast.extras.pressure_in = (day.slp_min_in + day.slp_max_in) / 2f;
            if (day.windspd_max_mph > 0 && day.humid_max_pct > 0)
            {
                forecast.extras.feelslike_f = WeatherUtils.GetFeelsLikeTemp(forecast.high_f.Value, day.windspd_max_mph, (int)Math.Round(day.humid_max_pct));
                forecast.extras.feelslike_c = ConversionMethods.FtoC(forecast.extras.feelslike_f.Value);
            }
            if (forecast.high_c > 0 && forecast.high_c < 60 && day.humid_max_pct > 1)
            {
                forecast.extras.dewpoint_c = WeatherUtils.CalculateDewpointC(forecast.high_c.Value, (int)Math.Round(day.humid_max_pct));
                forecast.extras.dewpoint_f = ConversionMethods.CtoF(forecast.extras.dewpoint_c.Value);
            }
            forecast.extras.wind_mph = day.windspd_max_mph;
            forecast.extras.wind_kph = day.windspd_max_kmh;
            forecast.extras.pop = (int)MathF.Round(day.prob_precip_pct);
            forecast.extras.windgust_mph = day.windgst_max_mph;
            forecast.extras.windgust_kph = day.windgst_max_kmh;
            forecast.extras.qpf_rain_mm = day.rain_total_mm;
            forecast.extras.qpf_rain_in = day.rain_total_in;
            forecast.extras.qpf_snow_cm = day.snow_total_mm / 10;
            forecast.extras.qpf_snow_in = day.snow_total_in;

            return forecast;
        }

        public static HourlyForecast CreateHourlyForecast(this WeatherUnlockedProvider _, WeatherUnlocked.Timeframe timeframe)
        {
            var hrf = new HourlyForecast();

            string date = timeframe.utcdate;
            int time = timeframe.utctime;
            DateTime dateObj = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            TimeSpan timeObj;
            if (time == 0)
            {
                timeObj = TimeSpan.Zero;
            }
            else
            {
                timeObj = TimeSpan.ParseExact(time.ToInvariantString("0000"), "%hmm", CultureInfo.InvariantCulture);
            }
            hrf.date = new DateTimeOffset(dateObj, TimeSpan.Zero).Add(timeObj);

            hrf.high_f = timeframe.temp_f;
            hrf.high_c = timeframe.temp_c;
            hrf.condition = timeframe.wx_desc;
            hrf.icon = timeframe.wx_code.ToInvariantString();

            hrf.wind_degrees = (int)MathF.Round(timeframe.winddir_deg);
            hrf.wind_mph = timeframe.windspd_mph;
            hrf.wind_kph = timeframe.windspd_kmh;

            // Extras
            hrf.extras = new ForecastExtras();
            hrf.extras.humidity = (int)MathF.Round(timeframe.humid_pct);
            hrf.extras.cloudiness = (int)MathF.Round(timeframe.cloudtotal_pct);
            hrf.extras.pressure_mb = timeframe.slp_mb;
            hrf.extras.pressure_in = timeframe.slp_in;
            hrf.extras.wind_mph = hrf.wind_mph;
            hrf.extras.wind_kph = hrf.wind_kph;
            hrf.extras.dewpoint_f = timeframe.dewpoint_f;
            hrf.extras.dewpoint_c = timeframe.dewpoint_c;
            hrf.extras.feelslike_f = timeframe.feelslike_f;
            hrf.extras.feelslike_c = timeframe.feelslike_c;
            if (int.TryParse(timeframe.prob_precip_pct, out int pop))
            {
                hrf.extras.pop = pop;
            }
            else
            {
                hrf.extras.pop = 0;
            }
            hrf.extras.windgust_mph = timeframe.windgst_mph;
            hrf.extras.windgust_kph = timeframe.windgst_kmh;
            hrf.extras.visibility_mi = timeframe.vis_mi;
            hrf.extras.visibility_km = timeframe.vis_km;
            hrf.extras.qpf_rain_mm = timeframe.rain_mm;
            hrf.extras.qpf_rain_in = timeframe.rain_in;
            hrf.extras.qpf_snow_cm = timeframe.snow_mm / 10;
            hrf.extras.qpf_snow_in = timeframe.snow_in;

            return hrf;
        }

        public static Condition CreateCondition(this WeatherUnlockedProvider _, CurrentRootobject currRoot)
        {
            return new Condition()
            {
                temp_f = currRoot.temp_f,
                temp_c = currRoot.temp_c,

                weather = currRoot.wx_desc,
                icon = currRoot.wx_code.ToInvariantString(),

                wind_degrees = (int)MathF.Round(currRoot.winddir_deg),
                wind_mph = currRoot.windspd_mph,
                wind_kph = currRoot.windspd_kmh,
                feelslike_f = currRoot.feelslike_f,
                feelslike_c = currRoot.feelslike_c,

                beaufort = new Beaufort(WeatherUtils.GetBeaufortScale(currRoot.windspd_ms)),
            };
        }

        public static Atmosphere CreateAtmosphere(this WeatherUnlockedProvider _, CurrentRootobject currRoot)
        {
            return new Atmosphere()
            {
                humidity = (int)MathF.Round(currRoot.humid_pct),
                pressure_mb = currRoot.slp_mb,
                pressure_in = currRoot.slp_in,
                pressure_trend = String.Empty,
                visibility_mi = currRoot.vis_mi,
                visibility_km = currRoot.vis_km,
            };
        }
    }
}