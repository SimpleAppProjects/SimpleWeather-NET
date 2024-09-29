using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Linq;
using LocData = SimpleWeather.LocationData.LocationData;

namespace SimpleWeather.Weather_API.BrightSky
{
    public static partial class BrightSkyProviderExtensions
    {
        public static Weather CreateWeatherData(this BrightSkyProvider _, CurrentRootobject currRoot,
            ForecastRootobject foreRoot, LocData locationData)
        {
            var weather = new Weather();

            DateTimeOffset now;

            if (locationData?.tz_offset != null)
            {
                now = DateTimeOffset.UtcNow.ToOffset(locationData.tz_offset);
            }
            else
            {
                now = DateTimeOffset.UtcNow;
            }

            weather.location = _.CreateLocation(locationData);
            weather.update_time = now;

            foreRoot.weather?.Let(items =>
            {
                weather.forecast = new List<Forecast>(5);
                weather.hr_forecast = new List<HourlyForecast>(foreRoot.weather?.Length ?? 0);

                // Store potential min/max values
                var dayMax = float.NaN;
                var dayMin = float.NaN;

                var minTime = DateTimeOffset.MinValue;
                var currentDate = minTime;
                Forecast fcast = null;

                var iconCountMap = new Dictionary<string, int>();

                // No daily data for DWD
                for (int i = 0; i < items.Length; i++)
                {
                    var item = items[i];
                    var date = item.timestamp;

                    if (locationData?.tz_offset != null)
                    {
                        date = date.ToOffset(locationData.tz_offset);
                    }

                    if (currentDate == minTime)
                    {
                        currentDate = date;
                    }

                    // Add a new hour
                    if (date.Trim(TimeSpan.TicksPerHour) >= now.Trim(TimeSpan.TicksPerHour))
                    {
                        weather.hr_forecast.Add(_.CreateHourlyForecast(item));
                    }

                    // Create new forecast
                    if (i == 0 || currentDate.UtcDateTime.Date != date.UtcDateTime.Date)
                    {
                        // Last forecast for day; create forecast
                        if (fcast != null)
                        {
                            // condition (set in provider GetWeather method)
                            // date
                            fcast.date = currentDate.LocalDateTime;
                            // high
                            fcast.high_f = ConversionMethods.CtoF(dayMax);
                            fcast.high_c = dayMax;
                            // low
                            fcast.low_f = ConversionMethods.CtoF(dayMin);
                            fcast.low_c = dayMin;
                            // icon
                            fcast.icon = iconCountMap
                                .WhereNot(it => it.Key.EndsWith("-night"))
                                .MaxBy(it => it.Value)
                                .Key ?? fcast.icon;

                            weather.forecast.Add(fcast);
                        }

                        currentDate = date;
                        fcast = _.CreateForecast(item);
                        fcast.date = date.LocalDateTime;

                        // Reset
                        dayMax = float.NaN;
                        dayMin = float.NaN;
                        iconCountMap.Clear();
                    }

                    // Find max/min for each hour
                    var temp = item.temperature ?? float.NaN;

                    if (!float.IsNaN(temp) && (float.IsNaN(dayMax) || temp > dayMax))
                    {
                        dayMax = temp;
                    }
                    if (!float.IsNaN(temp) && (float.IsNaN(dayMin) || temp < dayMin))
                    {
                        dayMin = temp;
                    }

                    // Keep track of conditions
                    item.icon?.Let(it =>
                    {
                        iconCountMap[it] = iconCountMap.GetValueOrDefault(it, 0) + 1;
                    });
                }

                fcast = weather.forecast.LastOrDefault();
                if (fcast != null && fcast.condition == null && fcast.icon == null)
                {
                    weather.forecast.RemoveAt(weather.forecast.Count - 1);
                }

                var hrfcast = weather.hr_forecast.LastOrDefault();
                if (hrfcast != null && hrfcast.condition == null && hrfcast.icon == null)
                {
                    weather.hr_forecast.RemoveAt(weather.hr_forecast.Count - 1);
                }
            });

            weather.condition = _.CreateCondition(currRoot);
            weather.atmosphere = _.CreateAtmosphere(currRoot);
            weather.precipitation = _.CreatePrecipitation(currRoot);

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

            weather.source = WeatherAPI.DWD;

            return weather;
        }

        public static SimpleWeather.WeatherData.Location CreateLocation(this BrightSkyProvider _, LocData locationData)
        {
            // Use location name from location provider
            return new SimpleWeather.WeatherData.Location()
            {
                name = locationData?.name,
                latitude = ((float?)locationData?.latitude),
                longitude = ((float?)locationData?.longitude),
                tz_long = locationData?.tz_long,
            };
        }

        public static Condition CreateCondition(this BrightSkyProvider _, CurrentRootobject currRoot)
        {
            var condition = new Condition();

            currRoot?.weather?.temperature?.Let(it =>
            {
                condition.temp_f = ConversionMethods.CtoF(it);
                condition.temp_c = it;
            });

            condition.wind_degrees = currRoot.weather?.wind_direction_10;

            currRoot.weather?.wind_speed_10?.Let(it =>
            {
                condition.wind_mph = ConversionMethods.KphToMph(it);
                condition.wind_kph = it;
            });

            currRoot.weather?.wind_gust_speed_10?.Let(it =>
            {
                condition.windgust_mph = ConversionMethods.KphToMph(it);
                condition.windgust_kph = it;
            });

            condition.icon = currRoot.weather?.icon;

            condition.wind_mph?.Let(it =>
            {
                condition.beaufort = new(WeatherUtils.GetBeaufortScale(it.RoundToInt()));
            });

            condition.observation_time = currRoot.weather.timestamp;

            return condition;
        }

        public static Atmosphere CreateAtmosphere(this BrightSkyProvider _, CurrentRootobject currRoot)
        {
            var atmosphere = new Atmosphere();

            atmosphere.humidity = currRoot.weather?.relative_humidity;

            currRoot.weather?.pressure_msl?.Let(it =>
            {
                atmosphere.pressure_mb = it;
                atmosphere.pressure_in = ConversionMethods.MBToInHg(it);
                atmosphere.pressure_trend = "";
            });

            currRoot.weather?.visibility?.Let(it =>
            {
                atmosphere.visibility_mi = ConversionMethods.KmToMi(it / 1000f);
                atmosphere.visibility_km = it / 1000f;
            });

            currRoot.weather?.dew_point?.Let(it =>
            {
                atmosphere.dewpoint_f = ConversionMethods.CtoF(it);
                atmosphere.dewpoint_c = it;
            });

            return atmosphere;
        }

        public static Precipitation CreatePrecipitation(this BrightSkyProvider _, CurrentRootobject currRoot)
        {
            var precip = new Precipitation();

            // Use cloudiness value here
            precip.cloudiness = currRoot.weather?.cloud_cover?.RoundToInt();

            // Precipitation
            (currRoot.weather?.precipitation_10 ?? currRoot.weather?.precipitation_30 ?? currRoot.weather?.precipitation_10)?.Let(it =>
            {
                precip.qpf_rain_mm = it;
                precip.qpf_rain_in = ConversionMethods.MMToIn(it);
            });

            return precip;
        }

        public static HourlyForecast CreateHourlyForecast(this BrightSkyProvider _, ForecastWeather forecast)
        {
            var hrf = new HourlyForecast();

            hrf.date = forecast.timestamp;
            forecast.temperature?.Let(it =>
            {
                hrf.high_f = ConversionMethods.CtoF(it);
                hrf.high_c = it;
            });
            hrf.wind_degrees = forecast.wind_direction?.RoundToInt();
            forecast.wind_speed?.Let(it =>
            {
                hrf.wind_mph = ConversionMethods.KphToMph(it);
                hrf.wind_kph = it;
            });

            hrf.icon = forecast.icon;

            // Extras
            int? humidity = forecast.relative_humidity?.RoundToInt();
            hrf.extras = new ForecastExtras();
            if (hrf.high_f.HasValue && hrf.wind_mph.HasValue && humidity.HasValue)
            {
                var feelsLikeF = WeatherUtils.GetFeelsLikeTemp(hrf.high_f.Value, hrf.wind_mph.Value, humidity.Value);
                hrf.extras.feelslike_f = feelsLikeF;
                hrf.extras.feelslike_c = ConversionMethods.FtoC(feelsLikeF);
            }
            hrf.extras.humidity = humidity;
            forecast.dew_point?.Let(it =>
            {
                hrf.extras.dewpoint_f = ConversionMethods.CtoF(it);
                hrf.extras.dewpoint_c = it;
            });
            hrf.extras.cloudiness = forecast.cloud_cover?.RoundToInt();
            // Precipitation
            hrf.extras.pop = forecast.precipitation_probability ?? forecast.precipitation_probability_6h;
            forecast.pressure_msl?.Let(it =>
            {
                hrf.extras.pressure_in = ConversionMethods.MBToInHg(it);
                hrf.extras.pressure_mb = it;
            });
            hrf.extras.wind_degrees = hrf.wind_degrees;
            hrf.extras.wind_mph = hrf.wind_mph;
            hrf.extras.wind_kph = hrf.wind_kph;
            forecast.wind_gust_speed?.Let(it =>
            {
                hrf.extras.windgust_mph = ConversionMethods.KphToMph(it);
                hrf.extras.windgust_kph = it;
            });
            forecast.visibility?.Let(it =>
            {
                hrf.extras.visibility_mi = ConversionMethods.KmToMi(it / 1000f);
                hrf.extras.visibility_km = it / 1000f;
            });

            return hrf;
        }

        public static Forecast CreateForecast(this BrightSkyProvider _, ForecastWeather forecast)
        {
            return new Forecast
            {
                date = forecast.timestamp.UtcDateTime,
                icon = forecast.icon
            };
        }
    }
}