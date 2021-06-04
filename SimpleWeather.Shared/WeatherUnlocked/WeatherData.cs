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
        public Weather(WeatherUnlocked.CurrentRootobject currRoot, WeatherUnlocked.ForecastRootobject foreRoot)
        {
            location = new Location(currRoot);
            update_time = DateTimeOffset.UtcNow;

            // 8-day forecast / 3-hr forecast
            // 24hr / 3hr = 8items for each day
            forecast = new List<Forecast>(8);
            hr_forecast = new List<HourlyForecast>(64);

            // Forecast
            foreach (var day in foreRoot.Days)
            {
                Forecast fcast = new Forecast(day);

                int midDayIdx = day.Timeframes.Length / 2;
                for (int i = 0; i < day.Timeframes.Length; i++)
                {
                    var hrfcast = new HourlyForecast(day.Timeframes[i]);

                    if (i == midDayIdx)
                    {
                        fcast.icon = WeatherManager.GetProvider(WeatherAPI.WeatherUnlocked)
                            .GetWeatherIcon(hrfcast.icon);
                        fcast.condition = hrfcast.condition;
                    }

                    hr_forecast.Add(hrfcast);
                }

                forecast.Add(fcast);
            }

            condition = new Condition(currRoot);
            atmosphere = new Atmosphere(currRoot);
            //astronomy = new Astronomy(currRoot);
            //precipitation = new Precipitation(currRoot);
            ttl = 180;

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

            condition.observation_time = update_time;

            source = WeatherAPI.WeatherUnlocked;
        }
    }

    public partial class Location
    {
        public Location(WeatherUnlocked.CurrentRootobject currRoot)
        {
            // Use location name from location provider
            name = null;
            latitude = currRoot.lat;
            longitude = currRoot.lon;
            tz_long = null;
        }
    }

    public partial class Forecast
    {
        public Forecast(WeatherUnlocked.Day day)
        {
            date = DateTime.ParseExact(day.date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            high_f = day.temp_max_f;
            high_c = day.temp_max_c;
            low_f = day.temp_min_f;
            low_c = day.temp_min_c;

            //condition = null;
            //icon = null;

            // Extras
            extras = new ForecastExtras();
            extras.humidity = (int)MathF.Round((day.humid_min_pct + day.humid_max_pct) / 2);
            extras.pressure_mb = MathF.Round((day.slp_min_mb + day.slp_max_mb) / 2);
            extras.pressure_in = MathF.Round((day.slp_min_in + day.slp_max_in) / 2);
            if (day.windspd_max_mph > 0 && day.humid_max_pct > 0)
            {
                extras.feelslike_f = WeatherUtils.GetFeelsLikeTemp(high_f.Value, day.windspd_max_mph, (int)Math.Round(day.humid_max_pct));
                extras.feelslike_c = ConversionMethods.FtoC(extras.feelslike_f.Value);
            }
            if (high_c > 0 && high_c < 60 && day.humid_max_pct > 1)
            {
                extras.dewpoint_c = MathF.Round(WeatherUtils.CalculateDewpointC(high_c.Value, (int)Math.Round(day.humid_max_pct)));
                extras.dewpoint_f = MathF.Round(ConversionMethods.CtoF(extras.dewpoint_c.Value));
            }
            extras.wind_mph = MathF.Round(day.windspd_max_mph);
            extras.wind_kph = MathF.Round(day.windspd_max_kmh);
            extras.pop = (int)MathF.Round(day.prob_precip_pct);
            extras.windgust_mph = MathF.Round(day.windgst_max_mph);
            extras.windgust_kph = MathF.Round(day.windgst_max_kmh);
            extras.qpf_rain_mm = day.rain_total_mm;
            extras.qpf_rain_in = day.rain_total_in;
            extras.qpf_snow_cm = day.snow_total_mm / 10;
            extras.qpf_snow_in = day.snow_total_in;
        }
    }

    public partial class HourlyForecast
    {
        public HourlyForecast(WeatherUnlocked.Timeframe timeframe)
        {
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
            this.date = new DateTimeOffset(dateObj, TimeSpan.Zero).Add(timeObj);

            high_f = timeframe.temp_f;
            high_c = timeframe.temp_c;
            condition = timeframe.wx_desc;
            icon = timeframe.wx_code.ToInvariantString();

            wind_degrees = (int)MathF.Round(timeframe.winddir_deg);
            wind_mph = MathF.Round(timeframe.windspd_mph);
            wind_kph = MathF.Round(timeframe.windspd_kmh);

            // Extras
            extras = new ForecastExtras();
            extras.humidity = (int)MathF.Round(timeframe.humid_pct);
            extras.cloudiness = (int)MathF.Round(timeframe.cloudtotal_pct);
            extras.pressure_mb = timeframe.slp_mb;
            extras.pressure_in = timeframe.slp_in;
            extras.wind_mph = wind_mph;
            extras.wind_kph = wind_kph;
            extras.dewpoint_f = MathF.Round(timeframe.dewpoint_f);
            extras.dewpoint_c = MathF.Round(timeframe.dewpoint_c);
            extras.feelslike_f = MathF.Round(timeframe.feelslike_f);
            extras.feelslike_c = MathF.Round(timeframe.feelslike_c);
            if (int.TryParse(timeframe.prob_precip_pct, out int pop))
            {
                extras.pop = pop;
            }
            else
            {
                extras.pop = 0;
            }
            extras.windgust_mph = MathF.Round(timeframe.windgst_mph);
            extras.windgust_kph = MathF.Round(timeframe.windgst_kmh);
            extras.visibility_mi = timeframe.vis_mi;
            extras.visibility_km = timeframe.vis_km;
            extras.qpf_rain_mm = timeframe.rain_mm;
            extras.qpf_rain_in = timeframe.rain_in;
            extras.qpf_snow_cm = timeframe.snow_mm / 10;
            extras.qpf_snow_in = timeframe.snow_in;
        }
    }

    public partial class Condition
    {
        public Condition(WeatherUnlocked.CurrentRootobject currRoot)
        {
            temp_f = currRoot.temp_f;
            temp_c = currRoot.temp_c;

            weather = currRoot.wx_desc;
            icon = currRoot.wx_code.ToInvariantString();

            wind_degrees = (int)MathF.Round(currRoot.winddir_deg);
            wind_mph = currRoot.windspd_mph;
            wind_kph = currRoot.windspd_kmh;
            feelslike_f = currRoot.feelslike_f;
            feelslike_c = currRoot.feelslike_c;

            beaufort = new Beaufort((int)WeatherUtils.GetBeaufortScale(currRoot.windspd_ms));
        }
    }

    public partial class Atmosphere
    {
        public Atmosphere(WeatherUnlocked.CurrentRootobject currRoot)
        {
            humidity = (int)MathF.Round(currRoot.humid_pct);
            pressure_mb = currRoot.slp_mb;
            pressure_in = currRoot.slp_in;
            pressure_trend = String.Empty;
            visibility_mi = currRoot.vis_mi;
            visibility_km = currRoot.vis_km;
        }
    }
}