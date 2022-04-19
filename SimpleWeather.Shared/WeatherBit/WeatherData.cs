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
        public Weather(WeatherBit.CurrentRootobject currRoot, WeatherBit.ForecastRootobject foreRoot)
        {
            var currData = currRoot.data.First();
            var tzLong = currData.timezone;
            var tzOffset = DateTimeUtils.TzidToOffset(tzLong);

            location = new Location(currData);
            update_time = DateTimeOffset.FromUnixTimeSeconds(currData.ts).ToOffset(tzOffset);

            // 16-day forecast
            forecast = new List<Forecast>(16);

            foreach (var day in foreRoot.data)
            {
                var fcast = new Forecast(day);
                forecast.Add(fcast);
            }

            currRoot.minutely?.Let(it =>
            {
                min_forecast = new List<MinutelyForecast>(it.Length);

                it.ForEach(minute =>
                {
                    min_forecast.Add(new MinutelyForecast(minute).Apply(minfcast =>
                    {
                        minfcast.date = minfcast.date.ToOffset(tzOffset);
                    }));
                });
            });
            
            condition = new Condition(currData);
            atmosphere = new Atmosphere(currData);
            astronomy = new Astronomy(currData, foreRoot);
            precipitation = new Precipitation(currData);
            ttl = 120;

            query = String.Format(CultureInfo.InvariantCulture, "lat={0:0.####}&lon={1:0.####}", location.latitude, location.longitude);

            if ((!condition.high_f.HasValue || !condition.low_f.HasValue) && forecast.Count > 0)
            {
                condition.high_f = forecast[0].high_f.Value;
                condition.high_c = forecast[0].high_c.Value;
                condition.low_f = forecast[0].low_f.Value;
                condition.low_c = forecast[0].low_c.Value;
            }

            if (currRoot.alerts?.Length > 0)
            {
                weather_alerts = new HashSet<WeatherAlert>(currRoot.alerts.Length);

                foreach (var alert in currRoot.alerts)
                {
                    weather_alerts.Add(new WeatherAlert(alert).Apply(it =>
                    {
                        it.Date = it.Date.ToOffset(tzOffset);
                        it.ExpiresDate = it.ExpiresDate.ToOffset(tzOffset);
                    }));
                }
            }

            source = WeatherAPI.WeatherBitIo;
        }
    }

    public partial class Location
    {
        public Location(WeatherBit.CurrentDatum currData)
        {
            // Use location name from location provider
            name = null;
            latitude = currData.lat;
            longitude = currData.lon;
            tz_long = currData.timezone;
        }
    }

    public partial class Forecast
    {
        public Forecast(WeatherBit.ForecastDatum forecast)
        {
            date = DateTime.ParseExact(forecast.valid_date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            high_c = forecast.high_temp;
            high_f = ConversionMethods.CtoF(forecast.high_temp);
            low_c = forecast.low_temp;
            low_f = ConversionMethods.CtoF(forecast.low_temp);
            condition = forecast.weather?.description?.ToUpperCase();
            icon = WeatherManager.GetProvider(WeatherAPI.WeatherBitIo)
                   .GetWeatherIcon(forecast.weather?.icon);

            // Extras
            extras = new ForecastExtras()
            {
                humidity = forecast.rh,
                cloudiness = forecast.clouds,
                // 1hPA = 1mbar
                pressure_mb = forecast.slp,
                pressure_in = ConversionMethods.MBToInHg(forecast.slp),
                wind_degrees = forecast.wind_dir,
                wind_mph = ConversionMethods.MSecToMph(forecast.wind_spd),
                wind_kph = ConversionMethods.MSecToKph(forecast.wind_spd),
                dewpoint_c = forecast.dewpt,
                dewpoint_f = ConversionMethods.CtoF(forecast.dewpt),
                feelslike_c = forecast.app_max_temp,
                feelslike_f = ConversionMethods.CtoF(forecast.app_max_temp),
                pop = forecast.pop,
                visibility_km = forecast.vis,
                visibility_mi = ConversionMethods.KmToMi(forecast.vis),
                windgust_mph = ConversionMethods.MSecToMph(forecast.wind_gust_spd),
                windgust_kph = ConversionMethods.MSecToKph(forecast.wind_gust_spd),
                qpf_rain_mm = forecast.precip,
                qpf_rain_in = ConversionMethods.MMToIn(forecast.precip),
                qpf_snow_cm = forecast.snow / 10,
                qpf_snow_in = ConversionMethods.MMToIn(forecast.snow),
                uv_index = forecast.uv
            };
        }
    }

    public partial class MinutelyForecast
    {
        public MinutelyForecast(WeatherBit.Minutely item)
        {
            date = DateTimeOffset.FromUnixTimeSeconds(item.ts);
            rain_mm = item.precip;
        }
    }

    public partial class Condition
    {
        public Condition(WeatherBit.CurrentDatum current)
        {
            weather = current.weather?.description?.ToUpperCase();
            temp_c = current.temp;
            temp_f = ConversionMethods.CtoF(current.temp);
            wind_degrees = current.wind_dir;
            wind_mph = ConversionMethods.MSecToMph(current.wind_spd);
            wind_kph = ConversionMethods.MSecToKph(current.wind_spd);
            beaufort = new Beaufort(WeatherUtils.GetBeaufortScale(current.wind_spd));
            feelslike_c = current.app_temp;
            feelslike_f = ConversionMethods.CtoF(current.app_temp);

            icon = WeatherManager.GetProvider(WeatherAPI.WeatherBitIo)
                   .GetWeatherIcon(current.weather?.icon);

            uv = new UV(current.uv);

            airQuality = new AirQuality()
            {
                index = current.aqi
            };

            observation_time = DateTimeOffset.FromUnixTimeSeconds(current.ts).ToOffset(DateTimeUtils.TzidToOffset(current.timezone));
        }
    }

    public partial class Atmosphere
    {
        public Atmosphere(WeatherBit.CurrentDatum current)
        {
            humidity = current.rh.RoundToInt();
            pressure_mb = current.slp;
            pressure_in = ConversionMethods.MBToInHg(current.slp);
            pressure_trend = String.Empty;
            visibility_km = current.vis;
            visibility_mi = ConversionMethods.KmToMi(current.vis);
            dewpoint_c = current.dewpt;
            dewpoint_f = ConversionMethods.CtoF(current.dewpt);
        }
    }

    public partial class Astronomy
    {
        public Astronomy(WeatherBit.CurrentDatum current, WeatherBit.ForecastRootobject foreRoot)
        {
            var tzOffset = DateTimeUtils.TzidToOffset(current.timezone);
            var obsTime = DateTimeOffset.FromUnixTimeSeconds(current.ts).ToOffset(tzOffset);
            var obsDateStr = obsTime.ToInvariantString("yyyy-MM-dd");

            var currentDayFcast = foreRoot.data.FirstOrDefault(it => Equals(it.valid_date, obsDateStr));

            if (currentDayFcast != null)
            {
                try
                {
                    sunrise = DateTimeOffset.FromUnixTimeSeconds(currentDayFcast.sunrise_ts).ToOffset(tzOffset).DateTime;
                }
                catch { }
                try
                {
                    sunset = DateTimeOffset.FromUnixTimeSeconds(currentDayFcast.sunset_ts).ToOffset(tzOffset).DateTime;
                }
                catch { }
                try
                {
                    moonrise = DateTimeOffset.FromUnixTimeSeconds(currentDayFcast.moonrise_ts).ToOffset(tzOffset).DateTime;
                }
                catch { }
                try
                {
                    moonset = DateTimeOffset.FromUnixTimeSeconds(currentDayFcast.moonset_ts).ToOffset(tzOffset).DateTime;
                }
                catch { }

                currentDayFcast.moon_phase_lunation.Times(100).Let(it =>
                {
                    var moonPhaseType = it switch
                    {
                        >= 2 and < 23 => MoonPhase.MoonPhaseType.WaxingCrescent,
                        >= 23 and < 26 => MoonPhase.MoonPhaseType.FirstQtr,
                        >= 26 and < 48 => MoonPhase.MoonPhaseType.WaxingGibbous,
                        >= 48 and < 52 => MoonPhase.MoonPhaseType.FullMoon,
                        >= 52 and < 73 => MoonPhase.MoonPhaseType.WaningGibbous,
                        >= 73 and < 76 => MoonPhase.MoonPhaseType.LastQtr,
                        >= 76 and < 98 => MoonPhase.MoonPhaseType.WaningCrescent,
                        _ => MoonPhase.MoonPhaseType.NewMoon // 0, 1, 98, 99, 100
                    };

                    moonphase = new MoonPhase(moonPhaseType);
                });
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
        public Precipitation(WeatherBit.CurrentDatum current)
        {
            // Use cloudiness value here
            cloudiness = current.clouds;
            qpf_rain_mm = current.precip;
            qpf_rain_in = ConversionMethods.MMToIn(current.precip);
            qpf_snow_cm = current.snow / 10;
            qpf_snow_in = ConversionMethods.MMToIn(current.snow / 10);
        }
    }
}