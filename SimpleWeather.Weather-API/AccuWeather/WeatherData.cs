using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleWeather.Weather_API.AccuWeather
{
    internal static partial class AccuWeatherExtensions
    {
        public static Weather CreateWeatherData(this AccuWeatherProvider _, DailyForecastRootobject dailyRoot, HourlyForecastRootobject hourlyRoot, CurrentRootobject currentRoot)
        {
            var weather = new Weather();

            var currentItem = currentRoot.Items[0];
            var observationTime = currentItem.LocalObservationDateTime;
            var now = DateTimeOffset.UtcNow.ToOffset(observationTime.Offset);

            weather.update_time = now;

            weather.location = _.CreateLocation(currentRoot);

            weather.forecast = new List<Forecast>(dailyRoot.DailyForecasts.Length);
            weather.txt_forecast = new List<TextForecast>(dailyRoot.DailyForecasts.Length);
            var haveTodaysForecast = false;
            foreach (var fcast in dailyRoot.DailyForecasts)
            {
                if (!haveTodaysForecast && fcast.Date.Date.Equals(now.Date))
                {
                    weather.astronomy = _.CreateAstronomy(fcast);
                    weather.condition = _.CreateCondition(currentItem, fcast);
                    haveTodaysForecast = true;
                }

                weather.forecast.Add(_.CreateForecast(fcast));
                weather.txt_forecast.Add(_.CreateTextForecast(fcast));
            }

            weather.hr_forecast = new List<HourlyForecast>(hourlyRoot.Items.Length);
            foreach (var fcast in hourlyRoot.Items)
            {
                weather.hr_forecast.Add(_.CreateHourlyForecast(fcast));
            }

            if (weather.condition == null)
            {
                weather.condition = _.CreateCondition(currentItem);
            }
            weather.atmosphere = _.CreateAtmosphere(currentItem);
            weather.precipitation = _.CreatePrecipitation(currentItem);

            // Weather summary
            if (dailyRoot.Headline?.EffectiveEpochDate != null && dailyRoot.Headline?.EndEpochDate != null)
            {
                var effectiveDate = DateTimeOffset.FromUnixTimeSeconds(dailyRoot.Headline.EffectiveEpochDate.Value).ToOffset(observationTime.Offset);
                var endDate = DateTimeOffset.FromUnixTimeSeconds(dailyRoot.Headline.EndEpochDate.Value).ToOffset(observationTime.Offset);

                if (observationTime >= effectiveDate && observationTime <= endDate)
                {
                    weather.condition.summary = dailyRoot.Headline?.Text;
                }
            }

            weather.ttl = 180;
            weather.source = WeatherAPI.AccuWeather;

            return weather;
        }

        public static Location CreateLocation(this AccuWeatherProvider _, CurrentRootobject current)
        {
            /* Use name from location provider */
            return new Location()
            {
                name = null,
                latitude = null,
                longitude = null,
                tz_long = null,
            };
        }

        public static Forecast CreateForecast(this AccuWeatherProvider _, Dailyforecast daily)
        {
            var forecast = new Forecast();

            forecast.date = daily.Date.DateTime;

            if (daily.Temperature.Maximum.Value.HasValue)
            {
                forecast.high_f = ConversionMethods.CtoF(daily.Temperature.Maximum.Value.Value);
                forecast.high_c = daily.Temperature.Maximum.Value;
            }
            if (daily.Temperature.Minimum.Value.HasValue)
            {
                forecast.low_f = ConversionMethods.CtoF(daily.Temperature.Minimum.Value.Value);
                forecast.low_c = daily.Temperature.Minimum.Value;
            }

            forecast.condition = daily.Day?.IconPhrase ?? daily.Night?.IconPhrase;
            forecast.icon = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.AccuWeather)
                .GetWeatherIcon(false, daily.Day?.Icon?.ToString() ?? daily.Night?.Icon?.ToString());

            // Extras
            forecast.extras = new ForecastExtras();

            forecast.extras.feelslike_c = daily.RealFeelTemperature?.Maximum?.Value ?? daily.RealFeelTemperature?.Minimum?.Value;
            if (forecast.extras.feelslike_c.HasValue)
            {
                forecast.extras.feelslike_f = ConversionMethods.CtoF(forecast.extras.feelslike_c.Value);
            }

            forecast.extras.uv_index = daily.AirAndPollen?.FirstOrDefault(it => it?.Name == "UVIndex")?.Value;
            forecast.extras.pop = daily.Day?.PrecipitationProbability ?? daily.Night?.PrecipitationProbability;
            forecast.extras.cloudiness = daily.Day?.CloudCover?.RoundToInt() ?? daily.Night?.CloudCover?.RoundToInt();

            forecast.extras.qpf_rain_mm = daily.Day?.Rain?.Value ?? daily.Night?.Rain?.Value;
            if (forecast.extras.qpf_rain_mm.HasValue)
            {
                forecast.extras.qpf_rain_in = ConversionMethods.MMToIn(forecast.extras.qpf_rain_mm.Value);
            }

            forecast.extras.qpf_snow_cm = daily.Day?.Snow?.Value ?? daily.Night?.Snow?.Value;
            if (forecast.extras.qpf_snow_in.HasValue)
            {
                forecast.extras.qpf_snow_in = ConversionMethods.MMToIn(forecast.extras.qpf_snow_cm.Value * 10);
            }

            if (daily.Day?.Wind != null)
            {
                forecast.extras.wind_degrees = daily.Day?.Wind?.Direction?.Degrees?.RoundToInt();
                forecast.extras.wind_kph = daily.Day?.Wind?.Speed.Value;
                if (forecast.extras.wind_kph.HasValue)
                {
                    forecast.extras.wind_mph = ConversionMethods.KphToMph(forecast.extras.wind_kph.Value);
                }
            }
            else if (daily.Night?.Wind != null)
            {
                forecast.extras.wind_degrees = daily.Night?.Wind?.Direction?.Degrees?.RoundToInt();
                forecast.extras.wind_kph = daily.Night?.Wind?.Speed.Value;
                if (forecast.extras.wind_kph.HasValue)
                {
                    forecast.extras.wind_mph = ConversionMethods.KphToMph(forecast.extras.wind_kph.Value);
                }
            }

            if (daily.Day?.WindGust != null)
            {
                forecast.extras.windgust_kph = daily.Day?.WindGust?.Speed.Value;
                if (forecast.extras.windgust_kph.HasValue)
                {
                    forecast.extras.windgust_mph = ConversionMethods.KphToMph(forecast.extras.windgust_kph.Value);
                }
            }
            else if (daily.Night?.WindGust != null)
            {
                forecast.extras.windgust_kph = daily.Night?.WindGust?.Speed.Value;
                if (forecast.extras.windgust_kph.HasValue)
                {
                    forecast.extras.windgust_mph = ConversionMethods.KphToMph(forecast.extras.windgust_kph.Value);
                }
            }

            return forecast;
        }

        public static TextForecast CreateTextForecast(this AccuWeatherProvider _, Dailyforecast daily)
        {
            var forecast = new TextForecast();

            forecast.date = daily.Date;

            var ResLoader = SharedModule.Instance.ResLoader;
            var fctStr = new StringBuilder();
            if (daily.Day != null)
            {
                fctStr.Append($"{ResLoader.GetString("label_day")} - {daily.Day.LongPhrase}");
            }
            if (daily.Night != null)
            {
                if (fctStr.Length > 0) fctStr.AppendLine();
                fctStr.Append($"{ResLoader.GetString("label_night")} - {daily.Night.LongPhrase}");
            }

            forecast.fcttext = fctStr.ToString();
            forecast.fcttext_metric = forecast.fcttext;

            return forecast;
        }

        public static HourlyForecast CreateHourlyForecast(this AccuWeatherProvider _, HourlyItem hourly)
        {
            var hrf = new HourlyForecast();

            hrf.date = hourly.DateTime;

            hrf.high_c = hourly.Temperature?.Value;
            hrf.high_f = hourly.Temperature?.Value?.Let((i) =>
            {
                return ConversionMethods.CtoF(i);
            });

            hrf.condition = hourly.IconPhrase;
            hrf.icon = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.AccuWeather)
                .GetWeatherIcon(!hourly.IsDaylight.GetValueOrDefault(), hourly.WeatherIcon?.ToString());

            // Extras
            hrf.extras = new ForecastExtras();
            hourly.RealFeelTemperature?.Value?.Let((it) =>
            {
                hrf.extras.feelslike_c = it;
                hrf.extras.feelslike_f = ConversionMethods.CtoF(it);
            });
            hrf.extras.humidity = hourly.RelativeHumidity;
            hourly.DewPoint?.Value?.Let((it) =>
            {
                hrf.extras.dewpoint_c = it;
                hrf.extras.dewpoint_f = ConversionMethods.CtoF(it);
            });

            hrf.extras.uv_index = hourly.UVIndex;
            hrf.extras.pop = hourly.PrecipitationProbability;
            hrf.extras.cloudiness = hourly.CloudCover;

            hourly.Rain?.Value?.Let((it) =>
            {
                hrf.extras.qpf_rain_mm = it;
                hrf.extras.qpf_rain_in = ConversionMethods.MMToIn(it);
            });
            hourly.Snow?.Value?.Let((it) =>
            {
                hrf.extras.qpf_snow_cm = it;
                hrf.extras.qpf_snow_in = ConversionMethods.MMToIn(it * 10);
            });

            hourly.Wind?.Speed?.Value?.Let((it) =>
            {
                hrf.extras.wind_kph = it;
                hrf.wind_kph = it;

                hrf.extras.wind_mph = ConversionMethods.KphToMph(it);
                hrf.wind_mph = hrf.extras.wind_mph;
            });
            hrf.extras.wind_degrees = hourly.Wind?.Direction?.Degrees?.RoundToInt();

            hourly.Visibility?.Value?.Let((it) =>
            {
                hrf.extras.visibility_km = it;
                hrf.extras.visibility_mi = ConversionMethods.KmToMi(it);
            });

            hourly.WindGust?.Speed?.Value?.Let((it) =>
            {
                hrf.extras.windgust_kph = it;
                hrf.extras.windgust_mph = ConversionMethods.KphToMph(it);
            });

            return hrf;
        }

        public static Astronomy CreateAstronomy(this AccuWeatherProvider _, Dailyforecast daily)
        {
            var astro = new Astronomy();

            astro.sunrise = daily.Sun?.Rise?.DateTime ?? DateTime.Now.Date.AddYears(1).AddTicks(-1);
            astro.sunset = daily.Sun?.Set?.DateTime ?? DateTime.Now.Date.AddYears(1).AddTicks(-1);

            astro.moonrise = daily.Moon?.Rise?.DateTime ?? DateTime.MinValue;
            astro.moonset = daily.Moon?.Set?.DateTime ?? DateTime.MinValue;

            daily.Moon?.Phase?.Let((it) =>
            {
                astro.moonphase = it.ToLowerInvariant() switch
                {
                    "new" or "newmoon" => new MoonPhase(MoonPhase.MoonPhaseType.NewMoon),
                    "waxingcrescent" => new MoonPhase(MoonPhase.MoonPhaseType.WaxingCrescent),
                    "first" or "firstquarter" => new MoonPhase(MoonPhase.MoonPhaseType.FirstQtr),
                    "waxinggibbous" => new MoonPhase(MoonPhase.MoonPhaseType.WaxingGibbous),
                    "full" or "fullmoon" => new MoonPhase(MoonPhase.MoonPhaseType.FullMoon),
                    "waninggibbous" => new MoonPhase(MoonPhase.MoonPhaseType.WaningGibbous),
                    "third" or "last" => new MoonPhase(MoonPhase.MoonPhaseType.LastQtr),
                    "waningcrescent" => new MoonPhase(MoonPhase.MoonPhaseType.WaningCrescent),
                    _ => new MoonPhase(MoonPhase.MoonPhaseType.NewMoon),
                };
            });

            return astro;
        }

        public static Condition CreateCondition(this AccuWeatherProvider _, CurrentsItem current, Dailyforecast daily = null)
        {
            var condition = new Condition();

            condition.weather = current.WeatherText;
            condition.icon = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.AccuWeather)
                .GetWeatherIcon(!current.IsDayTime.GetValueOrDefault(), current.WeatherIcon?.ToString());

            condition.temp_f = current.Temperature?.Imperial?.Value;
            condition.temp_c = current.Temperature?.Metric?.Value;

            condition.wind_degrees = current.Wind?.Direction?.Degrees;
            condition.wind_mph = current.Wind?.Speed?.Imperial?.Value;
            condition.wind_kph = current.Wind?.Speed?.Metric?.Value;
            condition.windgust_mph = current.WindGust?.Speed?.Imperial?.Value;
            condition.windgust_kph = current.WindGust?.Speed?.Metric?.Value;

            condition.feelslike_f = current.RealFeelTemperature?.Imperial?.Value;
            condition.feelslike_c = current.RealFeelTemperature?.Metric?.Value;

            current.Wind?.Speed?.Imperial?.Value?.Let((it) =>
            {
                condition.beaufort = new Beaufort(WeatherUtils.GetBeaufortScale(it.RoundToInt()));
            });

            condition.uv = new UV()
            {
                index = current.UVIndex
            };

            if (current.TemperatureSummary?.Past6HourRange?.Maximum?.Imperial?.Value != null &&
                current.TemperatureSummary?.Past6HourRange?.Maximum?.Metric?.Value != null)
            {
                condition.high_f = current.TemperatureSummary?.Past6HourRange?.Maximum?.Imperial?.Value;
                condition.high_c = current.TemperatureSummary?.Past6HourRange?.Maximum?.Metric?.Value;
            }
            else if (current.TemperatureSummary?.Past12HourRange?.Maximum?.Imperial?.Value != null &&
                  current.TemperatureSummary?.Past12HourRange?.Maximum?.Metric?.Value != null)
            {
                condition.high_f = current.TemperatureSummary?.Past12HourRange?.Maximum?.Imperial?.Value;
                condition.high_c = current.TemperatureSummary?.Past12HourRange?.Maximum?.Metric?.Value;
            }
            else if (current.TemperatureSummary?.Past24HourRange?.Maximum?.Imperial?.Value != null &&
                  current.TemperatureSummary?.Past24HourRange?.Maximum?.Metric?.Value != null)
            {
                condition.high_f = current.TemperatureSummary?.Past24HourRange?.Maximum?.Imperial?.Value;
                condition.high_c = current.TemperatureSummary?.Past24HourRange?.Maximum?.Metric?.Value;
            }

            if (current.TemperatureSummary?.Past6HourRange?.Minimum?.Imperial?.Value != null &&
                    current.TemperatureSummary?.Past6HourRange?.Minimum?.Metric?.Value != null)
            {
                condition.low_f = current.TemperatureSummary?.Past6HourRange?.Minimum?.Imperial?.Value;
                condition.low_c = current.TemperatureSummary?.Past6HourRange?.Minimum?.Metric?.Value;
            }
            else if (current.TemperatureSummary?.Past12HourRange?.Minimum?.Imperial?.Value != null &&
                  current.TemperatureSummary?.Past12HourRange?.Minimum?.Metric?.Value != null)
            {
                condition.low_f = current.TemperatureSummary?.Past12HourRange?.Minimum?.Imperial?.Value;
                condition.low_c = current.TemperatureSummary?.Past12HourRange?.Minimum?.Metric?.Value;
            }
            else if (current.TemperatureSummary?.Past24HourRange?.Minimum?.Imperial?.Value != null &&
                  current.TemperatureSummary?.Past24HourRange?.Minimum?.Metric?.Value != null)
            {
                condition.low_f = current.TemperatureSummary?.Past24HourRange?.Minimum?.Imperial?.Value;
                condition.low_c = current.TemperatureSummary?.Past24HourRange?.Minimum?.Metric?.Value;
            }

            daily?.AirAndPollen?.FirstOrDefault(it => it?.Name == "AirQuality")?.Value?.Let((it) =>
            {
                condition.airQuality = new AirQuality()
                {
                    index = it
                };
            });

            if (daily?.AirAndPollen?.Length > 0)
            {
                int? treePollenValue = null;
                int? grassPollenValue = null;
                int? ragweedPollenValue = null;

                foreach (var item in daily.AirAndPollen)
                {
                    if (item?.Name == "Grass")
                    {
                        grassPollenValue = item.Value;
                    }
                    if (item?.Name == "Tree")
                    {
                        treePollenValue = item.Value;
                    }
                    if (item?.Name == "Ragweed")
                    {
                        ragweedPollenValue = item.Value;
                    }
                }

                if (grassPollenValue != null || treePollenValue != null || ragweedPollenValue != null)
                {
                    condition.pollen = new Pollen()
                    {
                        treePollenCount = treePollenValue switch
                        {
                            >= 0 and <= 14 => Pollen.PollenCount.Low,
                            >= 15 and <= 89 => Pollen.PollenCount.Moderate,
                            >= 90 and <= 1499 => Pollen.PollenCount.High,
                            >= 1500 => Pollen.PollenCount.VeryHigh,
                            _ => Pollen.PollenCount.Unknown
                        },
                        grassPollenCount = grassPollenValue switch
                        {
                            >= 0 and <= 4 => Pollen.PollenCount.Low,
                            >= 5 and <= 19 => Pollen.PollenCount.Moderate,
                            >= 20 and <= 199 => Pollen.PollenCount.High,
                            >= 200 => Pollen.PollenCount.VeryHigh,
                            _ => Pollen.PollenCount.Unknown
                        },
                        ragweedPollenCount = ragweedPollenValue switch
                        {
                            >= 0 and <= 9 => Pollen.PollenCount.Low,
                            >= 10 and <= 49 => Pollen.PollenCount.Moderate,
                            >= 50 and <= 499 => Pollen.PollenCount.High,
                            >= 500 => Pollen.PollenCount.VeryHigh,
                            _ => Pollen.PollenCount.Unknown
                        }
                    };
                }
            }

            condition.observation_time = current.LocalObservationDateTime;

            condition.summary = daily?.Let((it) =>
            {
                var ResLoader = SharedModule.Instance.ResLoader;
                var labelDay = ResLoader.GetString("label_day");
                var labelNite = ResLoader.GetString("label_night");

                var strBuilder = new StringBuilder();
                if (!String.IsNullOrWhiteSpace(it.Day?.LongPhrase))
                {
                    strBuilder.Append($"{labelDay} - {it.Day.LongPhrase}");
                }
                if (!String.IsNullOrWhiteSpace(it.Night?.LongPhrase))
                {
                    if (strBuilder.Length > 0) strBuilder.AppendLine();
                    strBuilder.Append($"{labelNite} - {it.Night.LongPhrase}");
                }

                return strBuilder.ToString();
            });

            return condition;
        }

        public static Atmosphere CreateAtmosphere(this AccuWeatherProvider _, CurrentsItem current)
        {
            return new Atmosphere()
            {
                humidity = current.RelativeHumidity,

                pressure_mb = current.Pressure?.Metric?.Value,
                pressure_in = current.Pressure?.Imperial?.Value,
                pressure_trend = current.PressureTendency?.Code switch
                {
                    "F" => "Falling",
                    "R" => "Rising",
                    _ => string.Empty
                },

                visibility_mi = current.Visibility?.Imperial?.Value,
                visibility_km = current.Visibility?.Metric?.Value,

                dewpoint_f = current.DewPoint?.Imperial?.Value,
                dewpoint_c = current.DewPoint?.Metric?.Value,
            };
        }

        public static SimpleWeather.WeatherData.Precipitation CreatePrecipitation(this AccuWeatherProvider _, CurrentsItem current)
        {
            var precip = new SimpleWeather.WeatherData.Precipitation();

            precip.cloudiness = current.CloudCover;

            if (current.PrecipitationType == "Rain" || current.PrecipitationType == "Mixed")
            {
                precip.qpf_rain_in = current.Precip1hr?.Imperial?.Value ?? current.PrecipitationSummary?.PastHour?.Imperial?.Value;

                if ((current.Precip1hr?.Metric?.Unit ?? current.PrecipitationSummary?.PastHour?.Metric?.Unit) == "mm")
                {
                    precip.qpf_rain_mm = current.Precip1hr?.Metric?.Value ?? current.PrecipitationSummary?.PastHour?.Metric?.Value;
                }
                else if ((current.Precip1hr?.Metric?.Unit ?? current.PrecipitationSummary?.PastHour?.Metric?.Unit) == "cm")
                {
                    precip.qpf_rain_mm = current.Precip1hr?.Metric?.Value?.Times(10) ?? current.PrecipitationSummary?.PastHour?.Metric?.Value?.Times(10);
                }
            }
            else if (current.PrecipitationType == "Snow" || current.PrecipitationType == "Ice")
            {
                precip.qpf_snow_in = current.Precip1hr?.Imperial?.Value ?? current.PrecipitationSummary?.PastHour?.Imperial?.Value;

                if ((current.Precip1hr?.Metric?.Unit ?? current.PrecipitationSummary?.PastHour?.Metric?.Unit) == "mm")
                {
                    precip.qpf_snow_cm = current.Precip1hr?.Metric?.Value?.Div(10) ?? current.PrecipitationSummary?.PastHour?.Metric?.Value;
                }
                else if ((current.Precip1hr?.Metric?.Unit ?? current.PrecipitationSummary?.PastHour?.Metric?.Unit) == "cm")
                {
                    precip.qpf_snow_cm = current.Precip1hr?.Metric?.Value ?? current.PrecipitationSummary?.PastHour?.Metric?.Value;
                }
            }

            return precip;
        }
    }
}