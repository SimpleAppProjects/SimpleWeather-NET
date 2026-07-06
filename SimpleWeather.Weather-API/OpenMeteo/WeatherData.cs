#nullable enable
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Linq;
using Condition = SimpleWeather.WeatherData.Condition;
using Location = SimpleWeather.WeatherData.Location;

namespace SimpleWeather.Weather_API.OpenMeteo
{
    public static partial class OpenMeteoWeatherProviderExtensions
    {
        public static Weather CreateWeatherData(this OpenMeteoWeatherProvider _, ForecastResponse forecast, AQIResponse aqiResponse)
        {
            var weather = new Weather();

            weather.location = _.CreateLocation(forecast);
            weather.update_time = DateTimeOffset.UtcNow;

            weather.forecast = forecast.daily?.Let(_.CreateForecasts);
            weather.hr_forecast = forecast.hourly?.Let(_.CreateHourlyForecasts);
            weather.min_forecast = forecast.minutely_15?.Let(_.CreateMinutelyForecasts);

            weather.condition = forecast.current?.Let(it => _.CreateCondition(it, aqiResponse.current));
            weather.atmosphere = forecast.current?.Let(it => _.CreateAtmosphere(it));
            weather.precipitation = forecast.current?.Let(it => _.CreatePrecipitation(it));

            if ((weather.condition?.high_f == null || weather.condition?.high_c == null) && weather.forecast?.Any() == true)
            {
                weather.condition?.high_f = weather.forecast[0].high_f;
                weather.condition?.high_c = weather.forecast[0].high_c;
                weather.condition?.low_f = weather.forecast[0].low_f;
                weather.condition?.low_c = weather.forecast[0].low_c;
            }

            weather.ttl = 120;
            weather.source = WeatherAPI.OpenMeteo;

            return weather;
        }

        public static Location CreateLocation(this OpenMeteoWeatherProvider _, ForecastResponse forecast)
        {
            return new Location()
            {
                /* Use name from location provider */
                name = null,
                latitude = forecast.latitude,
                longitude = forecast.longitude,
                tz_long = forecast.timezone,
            };
        }

        public static List<Forecast>? CreateForecasts(this OpenMeteoWeatherProvider _, Daily daily)
        {
            return daily.time?.Select((ts, index) =>
            {
                var forecast = new Forecast();

                forecast.date = DateTimeOffset.FromUnixTimeSeconds(ts).UtcDateTime;

                daily.temperature_2m_max?.ElementAtOrNull(index)?.Let(it =>
                {
                    forecast.high_f = ConversionMethods.CtoF(it);
                    forecast.high_c = it;
                });

                daily.temperature_2m_min?.ElementAtOrNull(index)?.Let(it =>
                {
                    forecast.low_f = ConversionMethods.CtoF(it);
                    forecast.low_c = it;
                });

                forecast.icon = forecast.condition = (daily.weather_code?.ElementAtOrNull(index) ?? -1).ToInvariantString();

                // Extras
                forecast.extras = new ForecastExtras();

                forecast.extras.uv_index = daily.uv_index_max?.ElementAtOrNull(index);
                forecast.extras.pop = daily.precipitation_probability_max?.ElementAtOrNull(index);

                daily.wind_speed_10m_max?.ElementAtOrNull(index)?.Let(it =>
                {
                    forecast.extras.wind_kph = it;
                    forecast.extras.wind_mph = ConversionMethods.KphToMph(it);
                });

                daily.wind_gusts_10m_max?.ElementAtOrNull(index)?.Let(it =>
                {
                    forecast.extras.windgust_kph = it;
                    forecast.extras.windgust_mph = ConversionMethods.KphToMph(it);
                });

                forecast.extras.wind_degrees = daily.wind_direction_10m_dominant?.ElementAtOrNull(index);

                forecast.extras.cloudiness = daily.cloud_cover_mean?.ElementAtOrNull(index);

                daily.apparent_temperature_mean?.ElementAtOrNull(index)?.Let(it =>
                {
                    forecast.extras.feelslike_c = it;
                    forecast.extras.feelslike_f = ConversionMethods.CtoF(it);
                });

                daily.dew_point_2m_mean?.ElementAtOrNull(index)?.Let(it =>
                {
                    forecast.extras.dewpoint_c = it;
                    forecast.extras.dewpoint_f = ConversionMethods.CtoF(it);
                });

                // 1hPA = 1mbar
                daily.pressure_msl_mean?.ElementAtOrNull(index)?.Let(it =>
                {
                    forecast.extras.pressure_mb = it;
                    forecast.extras.pressure_in = ConversionMethods.MBToInHg(it);
                });

                forecast.extras.humidity = daily.relative_humidity_2m_mean?.ElementAtOrNull(index);

                daily.visibility_mean?.ElementAtOrNull(index)?.Let(it =>
                {
                    forecast.extras.visibility_km = it / 1000;
                    forecast.extras.visibility_mi = ConversionMethods.KmToMi(it / 1000);
                });

                return forecast;
            })?.ToList();
        }

        public static List<HourlyForecast>? CreateHourlyForecasts(this OpenMeteoWeatherProvider _, Hourly hourly)
        {
            return hourly.time?.Select((ts, index) =>
            {
                var forecast = new HourlyForecast();

                forecast.date = DateTimeOffset.FromUnixTimeSeconds(ts);

                hourly.temperature_2m?.ElementAtOrNull(index)?.Let(it =>
                {
                    forecast.high_f = ConversionMethods.CtoF(it);
                    forecast.high_c = it;
                });

                var isDay = hourly.is_day?.ElementAtOrNull(index) ?? 1;
                forecast.icon = forecast.condition = $"{(hourly.weather_code?.ElementAtOrNull(index) ?? -1).ToInvariantString()}_{isDay}";

                // Extras
                forecast.extras = new ForecastExtras();

                forecast.extras.humidity = hourly.relative_humidity_2m?.ElementAtOrNull(index);

                hourly.dew_point_2m?.ElementAtOrNull(index)?.Let(it =>
                {
                    forecast.extras.dewpoint_c = it;
                    forecast.extras.dewpoint_f = ConversionMethods.CtoF(it);
                });

                hourly.apparent_temperature?.ElementAtOrNull(index)?.Let(it =>
                {
                    forecast.extras.feelslike_c = it;
                    forecast.extras.feelslike_f = ConversionMethods.CtoF(it);
                });

                forecast.extras.pop = hourly.precipitation_probability?.ElementAtOrNull(index);

                // 1hPA = 1mbar
                hourly.pressure_msl?.ElementAtOrNull(index)?.Let(it =>
                {
                    forecast.extras.pressure_mb = it;
                    forecast.extras.pressure_in = ConversionMethods.MBToInHg(it);
                });

                forecast.extras.cloudiness = hourly.cloud_cover?.ElementAtOrNull(index);

                hourly.visibility?.ElementAtOrNull(index)?.Let(it =>
                {
                    forecast.extras.visibility_km = it / 1000;
                    forecast.extras.visibility_mi = ConversionMethods.KmToMi(it / 1000);
                });

                hourly.wind_speed_10m?.ElementAtOrNull(index)?.Let(it =>
                {
                    forecast.extras.wind_kph = it;
                    forecast.extras.wind_mph = ConversionMethods.KphToMph(it);
                });

                hourly.wind_gusts_10m?.ElementAtOrNull(index)?.Let(it =>
                {
                    forecast.extras.windgust_kph = it;
                    forecast.extras.windgust_mph = ConversionMethods.KphToMph(it);
                });

                forecast.extras.wind_degrees = hourly.wind_direction_10m?.ElementAtOrNull(index);

                hourly.rain?.ElementAtOrNull(index)?.Let(it =>
                {
                    forecast.extras.qpf_rain_mm = it;
                    forecast.extras.qpf_rain_in = ConversionMethods.MMToIn(it);
                });

                hourly.snowfall?.ElementAtOrNull(index)?.Let(it =>
                {
                    forecast.extras.qpf_snow_cm = it;
                    forecast.extras.qpf_snow_in = ConversionMethods.MMToIn(it * 10);
                });

                return forecast;
            })?.ToList();
        }

        public static List<MinutelyForecast>? CreateMinutelyForecasts(this OpenMeteoWeatherProvider _, Minutely_15 minutely15)
        {
            return minutely15.time?.Select((ts, index) =>
            {
                return new MinutelyForecast()
                {
                    date = DateTimeOffset.FromUnixTimeSeconds(ts),
                    rain_mm = minutely15.rain?.ElementAtOrNull(index) ?? 0,
                    snow_mm = minutely15.snowfall?.ElementAtOrNull(index)?.Let(it => it * 10) ?? 0
                };
            })?.ToList();
        }

        public static Condition CreateCondition(this OpenMeteoWeatherProvider _, Current current, CurrentAQI? aqi)
        {
            var condition = new Condition();

            condition.observation_time = current.time?.Let(DateTimeOffset.FromUnixTimeSeconds) ?? DateTimeOffset.UtcNow;

            var isDay = current.is_day ?? 1;
            condition.icon = condition.weather = $"{current.weather_code?.ToInvariantString()}_{isDay}";

            current.temperature_2m?.Let(it =>
            {
                condition.temp_c = it;
                condition.temp_f = ConversionMethods.CtoF(it);
            });

            current.apparent_temperature?.Let(it =>
            {
                condition.feelslike_c = it;
                condition.feelslike_f = ConversionMethods.CtoF(it);
            });

            current.wind_speed_10m?.Let(it =>
            {
                condition.wind_kph = it;
                float windMph = ConversionMethods.KphToMph(it);
                condition.wind_mph = windMph;
                condition.beaufort = new Beaufort(WeatherUtils.GetBeaufortScale(mph: windMph.RoundToInt()));
            });

            current.wind_gusts_10m?.Let(it =>
            {
                condition.windgust_kph = it;
                condition.windgust_mph = ConversionMethods.KphToMph(it);
            });

            condition.wind_degrees = current.wind_direction_10m;

            condition.uv = current.uv_index?.Let(it => new UV(it));

            condition.airQuality = new AirQuality()
            {
                index = aqi?.us_aqi,
                pm25 = aqi?.us_aqi_pm2_5,
                pm10 = aqi?.us_aqi_pm10,
                o3 = aqi?.us_aqi_ozone,
                no2 = aqi?.us_aqi_nitrogen_dioxide,
                co = aqi?.us_aqi_carbon_monoxide,
                so2 = aqi?.us_aqi_sulphur_dioxide,
            };

            var treePollenMeasure = (aqi?.birch_pollen).HasValue && (aqi?.alder_pollen).HasValue ?
                MathF.Max(aqi!.birch_pollen!.Value, aqi!.alder_pollen!.Value) : (aqi?.birch_pollen ?? aqi?.alder_pollen);
            var grassPollenMeasure = aqi?.grass_pollen;
            var ragweedPollenMeasure = aqi?.ragweed_pollen;

            condition.pollen = new Pollen()
            {
                treePollenCount = treePollenMeasure switch
                {
                    null => Pollen.PollenCount.Unknown,
                    < 15 => Pollen.PollenCount.Low,
                    < 90 => Pollen.PollenCount.Moderate,
                    < 1500 => Pollen.PollenCount.High,
                    >= 1500 => Pollen.PollenCount.VeryHigh,
                    _ => Pollen.PollenCount.Unknown
                },
                grassPollenCount = grassPollenMeasure switch
                {
                    null => Pollen.PollenCount.Unknown,
                    < 5 => Pollen.PollenCount.Low,
                    < 20 => Pollen.PollenCount.Moderate,
                    < 200 => Pollen.PollenCount.High,
                    >= 200 => Pollen.PollenCount.VeryHigh,
                    _ => Pollen.PollenCount.Unknown
                },
                ragweedPollenCount = ragweedPollenMeasure switch
                {
                    null => Pollen.PollenCount.Unknown,
                    < 10 => Pollen.PollenCount.Low,
                    < 50 => Pollen.PollenCount.Moderate,
                    < 500 => Pollen.PollenCount.High,
                    >= 500 => Pollen.PollenCount.VeryHigh,
                    _ => Pollen.PollenCount.Unknown
                }
            };

            return condition;
        }

        public static Atmosphere CreateAtmosphere(this OpenMeteoWeatherProvider _, Current current)
        {
            var atmosphere = new Atmosphere();

            atmosphere.humidity = current.relative_humidity_2m;

            // 1hPa = 1mb
            current.pressure_msl?.Let(it =>
            {
                atmosphere.pressure_mb = it;
                atmosphere.pressure_in = ConversionMethods.MBToInHg(it);
            });

            atmosphere.pressure_trend = string.Empty;

            current.dew_point_2m?.Let(it =>
            {
                atmosphere.dewpoint_c = it;
                atmosphere.dewpoint_f = ConversionMethods.CtoF(it);
            });

            current.visibility?.Let(it =>
            {
                atmosphere.visibility_km = it / 1000;
                atmosphere.visibility_mi = ConversionMethods.KmToMi(it / 1000);
            });

            return atmosphere;
        }

        public static Precipitation CreatePrecipitation(this OpenMeteoWeatherProvider _, Current current)
        {
            var precip = new Precipitation();

            precip.pop = current.precipitation_probability;
            precip.cloudiness = current.cloud_cover;

            current.rain?.Let(it =>
            {
                precip.qpf_rain_mm = it;
                precip.qpf_rain_in = ConversionMethods.MMToIn(it);
            });

            current.snowfall?.Let(it =>
            {
                precip.qpf_snow_cm = it;
                precip.qpf_snow_in = ConversionMethods.MMToIn(it * 10);
            });

            return precip;
        }
    }
}
#nullable restore