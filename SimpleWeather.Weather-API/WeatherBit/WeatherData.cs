using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SimpleWeather.Weather_API.WeatherBit
{
    public static partial class WeatherBitIOProviderExtensions
    {
        public static Weather CreateWeatherData(this WeatherBitIOProvider _, WeatherBit.CurrentRootobject currRoot, WeatherBit.ForecastRootobject foreRoot, WeatherBit.HourlyRootobject hourlyRoot)
        {
            var weather = new Weather();

            var currData = currRoot.data.First();
            var tzLong = currData.timezone;
            var tzOffset = DateTimeUtils.TzidToOffset(tzLong);

            weather.location = _.CreateLocation(currData);
            weather.update_time = DateTimeOffset.FromUnixTimeSeconds(currData.ts).ToOffset(tzOffset);

            // 16-day forecast
            weather.forecast = new List<Forecast>(16);

            foreach (var day in foreRoot.data)
            {
                var fcast = _.CreateForecast(day);
                weather.forecast.Add(fcast);
            }

            hourlyRoot?.data?.Let(it =>
            {
                weather.hr_forecast = new List<HourlyForecast>(it.Length);

                it.ForEach(hourly =>
                {
                    weather.hr_forecast.Add(_.CreateHourlyForecast(hourly).Apply(hrfcast =>
                    {
                        hrfcast.date = hrfcast.date.ToOffset(tzOffset);
                    }));
                });
            });

            currRoot.minutely?.Let(it =>
            {
                weather.min_forecast = new List<MinutelyForecast>(it.Length);

                it.ForEach(minute =>
                {
                    weather.min_forecast.Add(_.CreateMinutelyForecast(minute).Apply(minfcast =>
                    {
                        minfcast.date = minfcast.date.ToOffset(tzOffset);
                    }));
                });
            });

            weather.condition = _.CreateCondition(currData);
            weather.atmosphere = _.CreateAtmosphere(currData);
            weather.astronomy = _.CreateAstronomy(currData, foreRoot);
            weather.precipitation = _.CreatePrecipitation(currData);
            weather.ttl = 120;

            weather.query = String.Format(CultureInfo.InvariantCulture, "lat={0:0.####}&lon={1:0.####}", weather.location.latitude, weather.location.longitude);

            if ((!weather.condition.high_f.HasValue || !weather.condition.low_f.HasValue) && weather.forecast.Count > 0)
            {
                weather.condition.high_f = weather.forecast[0].high_f.Value;
                weather.condition.high_c = weather.forecast[0].high_c.Value;
                weather.condition.low_f = weather.forecast[0].low_f.Value;
                weather.condition.low_c = weather.forecast[0].low_c.Value;
            }

            weather.weather_alerts = _.CreateWeatherAlerts(currRoot.alerts, tzOffset);

            weather.source = WeatherAPI.WeatherBitIo;

            return weather;
        }

        public static Location CreateLocation(this WeatherBitIOProvider _, WeatherBit.CurrentDatum currData)
        {
            return new Location()
            {
                // Use location name from location provider
                name = null,
                latitude = currData.lat,
                longitude = currData.lon,
                tz_long = currData.timezone,
            };
        }

        public static Forecast CreateForecast(this WeatherBitIOProvider _, WeatherBit.ForecastDatum forecast)
        {
            return new Forecast
            {
                date = DateTime.ParseExact(forecast.valid_date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                high_c = forecast.high_temp,
                high_f = ConversionMethods.CtoF(forecast.high_temp),
                low_c = forecast.low_temp,
                low_f = ConversionMethods.CtoF(forecast.low_temp),
                condition = forecast.weather?.description?.ToUpperCase(),
                icon = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.WeatherBitIo)
                   .GetWeatherIcon(forecast.weather?.icon),

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
                }
            };
        }

        public static HourlyForecast CreateHourlyForecast(this WeatherBitIOProvider _, WeatherBit.HourlyDatum forecast)
        {
            return new HourlyForecast
            {
                date = DateTimeOffset.FromUnixTimeSeconds(forecast.ts),
                high_c = forecast.temp,
                high_f = ConversionMethods.CtoF(forecast.temp),
                condition = forecast.weather?.description?.ToUpperCase(),
                icon = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.WeatherBitIo)
                   .GetWeatherIcon(forecast.weather?.icon),

                wind_degrees = forecast.wind_dir,
                wind_kph = ConversionMethods.MSecToKph(forecast.wind_spd),
                wind_mph = ConversionMethods.MSecToMph(forecast.wind_spd),

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
                    feelslike_c = forecast.app_temp,
                    feelslike_f = ConversionMethods.CtoF(forecast.app_temp),
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
                }
            };
        }

        public static MinutelyForecast CreateMinutelyForecast(this WeatherBitIOProvider _, WeatherBit.Minutely item)
        {
            return new MinutelyForecast
            {
                date = DateTimeOffset.FromUnixTimeSeconds(item.ts),
                rain_mm = item.precip
            };
        }

        public static Condition CreateCondition(this WeatherBitIOProvider _, WeatherBit.CurrentDatum current)
        {
            var condition = new Condition
            {
                weather = current.weather?.description?.ToUpperCase(),
                temp_c = current.temp,
                temp_f = ConversionMethods.CtoF(current.temp),
                wind_degrees = current.wind_dir,
                wind_mph = ConversionMethods.MSecToMph(current.wind_spd),
                wind_kph = ConversionMethods.MSecToKph(current.wind_spd),
                beaufort = new Beaufort(WeatherUtils.GetBeaufortScale(current.wind_spd)),
                feelslike_c = current.app_temp,
                feelslike_f = ConversionMethods.CtoF(current.app_temp),

                icon = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.WeatherBitIo)
                   .GetWeatherIcon(current.weather?.icon),

                uv = new UV(current.uv),

                airQuality = new AirQuality()
                {
                    index = current.aqi
                },

                observation_time = DateTimeOffset.FromUnixTimeSeconds(current.ts).ToOffset(DateTimeUtils.TzidToOffset(current.timezone))
            };

            return condition;
        }

        public static Atmosphere CreateAtmosphere(this WeatherBitIOProvider _, WeatherBit.CurrentDatum current)
        {
            return new Atmosphere()
            {
                humidity = current.rh.RoundToInt(),
                pressure_mb = current.slp,
                pressure_in = ConversionMethods.MBToInHg(current.slp),
                pressure_trend = String.Empty,
                visibility_km = current.vis,
                visibility_mi = ConversionMethods.KmToMi(current.vis),
                dewpoint_c = current.dewpt,
                dewpoint_f = ConversionMethods.CtoF(current.dewpt),
            };
        }

        public static Astronomy CreateAstronomy(this WeatherBitIOProvider _, WeatherBit.CurrentDatum current, WeatherBit.ForecastRootobject foreRoot)
        {
            var astronomy = new Astronomy();

            var tzOffset = DateTimeUtils.TzidToOffset(current.timezone);
            var obsTime = DateTimeOffset.FromUnixTimeSeconds(current.ts).ToOffset(tzOffset);
            var obsDateStr = obsTime.ToInvariantString("yyyy-MM-dd");

            var currentDayFcast = foreRoot.data.FirstOrDefault(it => Equals(it.valid_date, obsDateStr));

            if (currentDayFcast != null)
            {
                try
                {
                    astronomy.sunrise = DateTimeOffset.FromUnixTimeSeconds(currentDayFcast.sunrise_ts).ToOffset(tzOffset).DateTime;
                }
                catch { }
                try
                {
                    astronomy.sunset = DateTimeOffset.FromUnixTimeSeconds(currentDayFcast.sunset_ts).ToOffset(tzOffset).DateTime;
                }
                catch { }
                try
                {
                    astronomy.moonrise = DateTimeOffset.FromUnixTimeSeconds(currentDayFcast.moonrise_ts).ToOffset(tzOffset).DateTime;
                }
                catch { }
                try
                {
                    astronomy.moonset = DateTimeOffset.FromUnixTimeSeconds(currentDayFcast.moonset_ts).ToOffset(tzOffset).DateTime;
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

                    astronomy.moonphase = new MoonPhase(moonPhaseType);
                });
            }

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

        public static Precipitation CreatePrecipitation(this WeatherBitIOProvider _, WeatherBit.CurrentDatum current)
        {
            return new Precipitation()
            {
                // Use cloudiness value here
                cloudiness = current.clouds,
                qpf_rain_mm = current.precip,
                qpf_rain_in = ConversionMethods.MMToIn(current.precip),
                qpf_snow_cm = current.snow / 10,
                qpf_snow_in = ConversionMethods.MMToIn(current.snow / 10),
            };
        }
    }
}